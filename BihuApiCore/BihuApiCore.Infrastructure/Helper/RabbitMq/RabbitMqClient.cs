using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Helper.RabbitMq
{
    /// <summary>
    /// RabbitMQ客户端辅助类   还可以再精简，参照我在壁虎signal3项目中的改造
    /// </summary>
    public class RabbitMqClient : IDisposable
    {
        #region 构造函数及字段

        private readonly IConnection _conn;

        /// <summary>
        ///  Common AMQP model,usually call it channel
        /// </summary>
        protected readonly ConcurrentDictionary<string, IModel> ModelDicQueue = new ConcurrentDictionary<string, IModel>();

        protected readonly ConcurrentDictionary<string, IModel> ModelDicExchange = new ConcurrentDictionary<string, IModel>();

        private readonly ILogger<RabbitMqClient> _logger;
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// 构造函数，单例启动
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionFactory"></param>
        /// <param name="env"></param>
        public RabbitMqClient(ILogger<RabbitMqClient> logger, ConnectionFactory connectionFactory, IHostingEnvironment env)
        {
            _conn = _conn ?? connectionFactory.CreateConnection();
            _logger = logger;
            _env = env;
        }

        public void Dispose()
        {
            foreach (var item in ModelDicQueue) item.Value.Dispose();
            foreach (var item in ModelDicExchange) item.Value.Dispose();
            _conn.Dispose();
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// 获取特性拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private RabbitMqQueueAttribute GetRabbitMqAttribute<T>()
        {
            var result1 = Attribute.GetCustomAttribute(typeof(T), typeof(RabbitMqQueueAttribute));
            var result = result1 != null ? result1 as RabbitMqQueueAttribute : new RabbitMqQueueAttribute();
            if (result.Customize)
            {
                //暂时先只定义直接发送模式
                result.RouteKey = result.RouteKey;
                result.QueueName = result.QueueName;
                result.ExchangeName = result.ExchangeName;
                return result;
            }
            string rolekey = typeof(T).Name + $"_{_env.EnvironmentName}_";
            result.RouteKey = rolekey;
            result.DelayRouteKey = rolekey;
            result.DeadRouteKey = rolekey;
            result.QueueName = string.Concat(result.QueueName, "_", rolekey);
            result.DelayQueueName = string.Concat(result.DelayQueueName, "_", rolekey);
            result.DeadQueueName = string.Concat(result.DeadQueueName, $"_", rolekey);

            return result;
        }

        /// <summary>
        ///  获取或新增Model 也叫channel   
        /// </summary>
        /// <param name="exchange">交换机名称</param>
        /// <param name="exchangeType">交换机类型 direct</param>
        /// <param name="durable">是否持久化</param>
        /// <param name="autoDelete">当所有绑定队列都不在使用时，是否自动删除交换器 true：删除false：不删除</param>
        /// <param name="exchangeArgs"></param>
        /// <returns></returns>
        private IModel GetOrAddModelPublish(string exchange, string exchangeType, bool durable = false, bool autoDelete = false, IDictionary<string, object> exchangeArgs = null)
        {
            exchange = exchange.Trim();
            return ModelDicExchange.GetOrAdd(exchange, key =>
            {
                var model = _conn.CreateModel();
                //声明订阅通道  这里不涉及发布，所以不声明了，直接使用壁虎的交换机
                model.ExchangeDeclare(exchange, exchangeType, durable, autoDelete, exchangeArgs);
                ModelDicExchange[exchange] = model;
                return model;
            });
        }

        /// <summary>
        ///  获取或新增Model 也叫channel   这里发送和接收应该分开。
        /// </summary>
        /// <param name="exchange">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey">匹配规则</param>
        /// <param name="durable">是否持久化</param>
        /// <param name="autoDelete">当所有消费客户端连接断开后，是否自动删除队列 true：删除false：不删除</param>
        /// <param name="queueArgs"></param>
        /// <returns></returns>
        private IModel GetOrAddModelConsumer(string exchange, string queue, string routingKey, bool durable = false, bool autoDelete = false, IDictionary<string, object> queueArgs = null)
        {
            queue = queue.Trim();
            if (_env.IsProduction())
            {
                autoDelete = false;
                durable = true;
            }
            return ModelDicQueue.GetOrAdd(queue, key =>
            {
                var model = _conn.CreateModel();
                model.QueueDeclare(queue, durable, false, autoDelete, queueArgs);
                //绑定队列
                model.QueueBind(queue, exchange, routingKey);
                ModelDicQueue[queue] = model;
                return model;
            });
        }

        #endregion

        #region 普通发送消息

        /// <summary>
        /// 使用模型推导queueInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        public void SendMessage<T>(T messageObj) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Normal)
                throw new ArgumentException("消息上不具有任何特性");
            var channel = GetOrAddModelPublish(queueInfo.ExchangeName, queueInfo.ExchangeType, queueInfo.ExchangeDurable);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;//数据持久化
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channel.BasicPublish(queueInfo.ExchangeName, queueInfo.RouteKey, properties, body);
        }

        /// <summary>
        /// 发送消息  自定义的queueInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="queueInfo"></param>
        public void SendMessage<T>(T messageObj, RabbitMqQueueModel queueInfo) where T : class
        {
            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            var channel = GetOrAddModelPublish(queueInfo.ExchangeName, queueInfo.ExchangeType, queueInfo.Durable);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;
            channel.ConfirmSelect();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channel.BasicPublish(queueInfo.ExchangeName, queueInfo.RouteKey, properties, body);
            bool isOk = channel.WaitForConfirms();
            if (!isOk) _logger.LogError("发布消息至对列确认失败。队列内容：{0}", JsonConvert.SerializeObject(messageObj));
        }

        #endregion

        #region 延时消息，插件

        /// <summary>
        /// 发送延迟消息 通过插件rabbitmq-delayed-message-exchange实现 (服务器要安装这个插件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="publishTime">消息推送时间，即服务接收到消息的时间</param>
        public void SendMessageDelay<T>(T messageObj, DateTime publishTime) where T : class
        {
            long delay = (long)(publishTime - DateTime.Now).TotalSeconds;
            SendMessageDelay(messageObj, delay);
        }

        /// <summary>
        /// 发送延迟消息 通过插件rabbitmq-delayed-message-exchange实现 (服务器要安装这个插件)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="seconds">延时的秒数</param>
        public void SendMessageDelay<T>(T messageObj, long seconds) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Delay)
                throw new ArgumentException("消息上不具有任何特性");

            //延迟队列参数，必须
            IDictionary<string, object> args = new Dictionary<string, object>
            {
                {"x-delayed-type", "direct"}
            };
            var channelPublish = GetOrAddModelPublish(queueInfo.DelayExchangeName, queueInfo.DelayExchangeType, queueInfo.ExchangeDurable, false, args);

            var properties = channelPublish.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>
            {
                {"x-delay", seconds*1000}
            };
            properties.DeliveryMode = 2;

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channelPublish.BasicPublish(queueInfo.DelayExchangeName, queueInfo.DelayRouteKey, properties, body);
        }

        #endregion

        #region 延迟消息，死信

        /// <summary>
        ///  发送延迟消息 通过死信实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="publishTime">消息推送时间，即服务接收到消息的时间</param>
        public void SendMessageDead<T>(T messageObj, DateTime publishTime) where T : class
        {
            long delay = (long)(publishTime - DateTime.Now).TotalMilliseconds;
            SendMessageDead(messageObj, delay);
        }

        /// <summary>
        /// 发送延迟消息 通过死信实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="milliseconds">延时的秒数</param>
        public void SendMessageDead<T>(T messageObj, long milliseconds) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Dead)
                throw new ArgumentException("消息上不具有任何特性");

            //延迟队列参数，必须
            IDictionary<string, object> queueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", queueInfo.DeadExchangeName},
                {"x-dead-letter-routing-key", queueInfo.DeadRouteKey},
                {"x-message-ttl", milliseconds}, // 这个时间要一致 //单位毫秒！！
                {"x-expires", milliseconds+5000},//单位毫秒！！
                //Auto Expire(x-expires):当队列在指定的时间没有被访问(consume, basicGet, queueDeclare…)就会被删除
            };

            //创建一个名叫"wait_dead_queuexxxxxx"的固定等死消息队列
            string queueName = "wait_dead_queue" + Guid.NewGuid().ToString();
            var model = _conn.CreateModel();
            //这里如果自动删除，可能导致延时消息出现问题  durable为true 那么队列肯定会存活到消息发送，而且我们设置了
            model.QueueDeclare(queueName, true, false, false, queueArgs);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            //发送消息到等死队列
            model.BasicPublish("", queueName, null, body);
        }

        #endregion

        #region 消息接收

        public IModel ReceiveMessage<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Normal)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            var channelPublish = GetOrAddModelPublish(queueInfo.ExchangeName, queueInfo.ExchangeType, queueInfo.ExchangeDurable);
            var channel = GetOrAddModelConsumer(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey, queueInfo.QueueDurable);

            //同一时间不处理超过10条消息  
            channel.BasicQos(0, 10, false);
            var consumer = new EventingBasicConsumer(channel);
            //autoAck =true 自动应答，一旦我们完成任务，消费者会自动发送应答。通知RabbitMQ消息已被处理，可以从内存删除
            channel.BasicConsume(queueInfo.QueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler, cancellationToken); };
            return channel;
        }


        #region 延时消息

        /// <summary>
        ///  接收延迟消息 通过死信方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public IModel ReceiveMessageDead<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Dead)
                throw new ArgumentException("消息上不具有任何特性");

            if (handler == null) throw new NullReferenceException("处理事件为null");
            var channelPublish = GetOrAddModelPublish(queueInfo.DeadExchangeName, queueInfo.DeadExchangeType, queueInfo.ExchangeDurable);
            var channel = GetOrAddModelConsumer(queueInfo.DeadExchangeName, queueInfo.DeadQueueName, queueInfo.DeadRouteKey, queueInfo.QueueDurable);

            //同一时间不处理超过20条消息
            channel.BasicQos(0, 20, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.DeadQueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler, cancellationToken); };
            return channel;
        }

        /// <summary>
        ///     接收延迟消息，通过 通过插件rabbitmq-delayed-message-exchange实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public IModel ReceiveMessageDelay<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Delay)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            //延迟队列参数，必须

            IDictionary<string, object> args = new Dictionary<string, object>
            {
                {"x-delayed-type", "direct"}
            };

            var channelPublish = GetOrAddModelPublish(queueInfo.DelayExchangeName, queueInfo.DelayExchangeType, queueInfo.ExchangeDurable, false, args);
            var channel = GetOrAddModelConsumer(queueInfo.DelayExchangeName, queueInfo.DelayQueueName, queueInfo.DelayRouteKey, queueInfo.QueueDurable, false);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.DelayQueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler, cancellationToken); };
            return channel;
        }

        #endregion

        #region 消费委托封装

        private async Task _doAsync<T>(IModel channel, BasicDeliverEventArgs ea,
           Func<T, CancellationToken, Task> normalHandler, CancellationToken cancellationToken)
        {
            var body = ea.Body;
            try
            {
                var msgBody = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
                await normalHandler(msgBody, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"_doAsync处理异常：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
            }
            finally
            {
                //如果autoAck =true 可以不用再次确认收到   所以这里我再统一应答，各个方法使用自己的写法
                channel.BasicAck(ea.DeliveryTag, false);
            }
        }

        #endregion

        #endregion
    }
}
