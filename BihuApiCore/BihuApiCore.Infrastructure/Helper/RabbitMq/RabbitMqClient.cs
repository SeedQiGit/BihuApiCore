using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BihuApiCore.Infrastructure.Helper.RabbitMq
{
     /// <summary>
    /// RabbitMQ客户端辅助类
    /// </summary>
    public class RabbitMqClient:IDisposable
    {
        #region 构造函数及字段

        private  IConnection _conn;

        private static readonly object LockObj = new object();

        /// <summary>
        ///  Common AMQP model,usually call it channel
        /// </summary>
        private  readonly ConcurrentDictionary<string, IModel> _modelDic =new ConcurrentDictionary<string, IModel>();

        private readonly ILogger<RabbitMqClient> _logger;

        /// <summary>
        /// 单例创建ConnectionFactory
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        private void Open(string host, string userName, string pwd)
        {
            if (_conn != null) return;
            lock (LockObj)
            {
                if (_conn != null) return;
                var factory = new ConnectionFactory
                {
                    HostName = host,
                    UserName = userName,
                    Password = pwd,
                    AutomaticRecoveryEnabled = true,
                    RequestedHeartbeat = 15
                };
                _conn = _conn ?? factory.CreateConnection();
            }
        }

        /// <summary>
        /// 构造函数，单例启动
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public RabbitMqClient(IConfiguration configuration,ILogger<RabbitMqClient> logger)
        {
            string host = configuration.GetSection("EventBusConnection").Value;
            string userName = configuration.GetSection("EventBusUserName").Value;
            string pwd = configuration.GetSection("EventBusPassword").Value;

            _logger = logger;
            Open(host, userName, pwd);
        }

        public void Dispose()
        {
            foreach (var item in _modelDic) item.Value.Dispose();
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
            var result = result1!=null ? result1 as RabbitMqQueueAttribute : new RabbitMqQueueAttribute();
            string rolekey= typeof(T).Name;
            result.RouteKey = rolekey;
            result.DelayRouteKey = rolekey;
            result.DeadRouteKey = rolekey;
            result.QueueName = string.Concat(result.QueueName,"_", rolekey);
            result.DelayQueueName = string.Concat(result.DelayQueueName, "_", rolekey);
            result.DeadQueueName = string.Concat(result.DeadQueueName, "_", rolekey);
            return result;
        }

        /// <summary>
        ///  获取或新增Model 也叫channel
        /// </summary>
        /// <param name="exchange">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey">匹配规则</param>
        /// <param name="exchangeType">交换机类型 direct</param>
        /// <param name="durable">是否持久化</param>
        /// <param name="autoDelete"></param>
        /// <param name="exchangeArgs"></param>
        /// <param name="queueArgs"></param>
        /// <returns></returns>
        private IModel GetOrAddModel(string exchange, string queue, string routingKey, string exchangeType,
            bool durable = true,
            bool autoDelete = false, IDictionary<string, object> exchangeArgs = null,
            IDictionary<string, object> queueArgs = null)
        {
            return _modelDic.GetOrAdd(queue, key =>
            {
                var model = _conn.CreateModel();
                //声明订阅通道
                ExchangeDeclare(model, exchange, exchangeType, durable, autoDelete, exchangeArgs);
               
                QueueDeclare(model, queue, durable, autoDelete, queueArgs);
                //绑定队列
                model.QueueBind(queue, exchange, routingKey);
                _modelDic[queue] = model;
                return model;
            });
        }

        private void ExchangeDeclare(IModel iModel, string exchange, string type, bool durable = true,
            bool autoDelete = false, IDictionary<string, object> arguments = null)
        {
            exchange = string.IsNullOrWhiteSpace(exchange) ? "" : exchange.Trim();
            iModel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        private void QueueDeclare(IModel channel, string queue, bool durable = true, bool autoDelete = false,
            IDictionary<string, object> arguments = null, bool exclusive = false)
        {
            queue = string.IsNullOrWhiteSpace(queue) ? "UndefinedQueueName" : queue.Trim();
            channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }


        #endregion

        #region 普通收发消息

        public void SendMessage<T>(T messageObj) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null|| queueInfo.MessageKind != RabbitMsgKind.Nomal)
                throw new ArgumentException("消息上不具有任何特性");
            var channel = GetOrAddModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey,
                queueInfo.ExchangeType, queueInfo.Durable);
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channel.BasicPublish(queueInfo.ExchangeName, queueInfo.RouteKey, properties, body);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="queueInfo"></param>
        public void SendMessage<T>(T messageObj, RabbitMqQueueModel queueInfo) where T : class
        {
            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            var channel = GetOrAddModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey,
                queueInfo.ExchangeType, queueInfo.Durable);
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
            long delay = (long) (publishTime - DateTime.Now).TotalSeconds;
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
            var channel = GetOrAddModel(queueInfo.DelayExchangeName, queueInfo.DelayQueueName, queueInfo.DelayRouteKey,
                queueInfo.DelayExchangeType, queueInfo.Durable, false, args);
            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>
            {
                {"x-delay", (seconds*1000)}
            };
            properties.DeliveryMode = 2;
      
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            channel.BasicPublish(queueInfo.DelayExchangeName, queueInfo.DelayRouteKey, properties, body);
        }

        #endregion

        #region 延迟消息，死信

        /// <summary>
        ///     发送延迟消息 通过死信实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="publishTime">消息推送时间，即服务接收到消息的时间</param>
        public void SendMessageDead<T>(T messageObj, DateTime publishTime) where T : class
        {
            long delay = (long) (publishTime - DateTime.Now).TotalSeconds;
            SendMessageDead(messageObj, delay);
        }

        /// <summary>
        ///     发送延迟消息 通过死信实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageObj"></param>
        /// <param name="seconds">延时的秒数</param>
        public void SendMessageDead<T>(T messageObj, long seconds) where T : class
        {
            var queueInfo = GetRabbitMqAttribute<T>();
            if (queueInfo == null||queueInfo.MessageKind!=RabbitMsgKind.Dead)
                throw new ArgumentException("消息上不具有任何特性");

            //延迟队列参数，必须
            IDictionary<string, object> queueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", queueInfo.DeadExchangeName},
                {"x-dead-letter-routing-key", queueInfo.DeadRouteKey}
            };

            var channel = _modelDic.GetOrAdd(queueInfo.QueueName, key =>
            {
                //声明原交换机，队列，并绑定
                var model = _conn.CreateModel();
                ExchangeDeclare(model, queueInfo.ExchangeName, queueInfo.ExchangeType, queueInfo.Durable);
                QueueDeclare(model, queueInfo.QueueName, queueInfo.Durable, false, queueArgs);
                model.QueueBind(queueInfo.QueueName, queueInfo.ExchangeName, queueInfo.RouteKey, null);

                //声明死信交换机，死信队列，并绑定
                ExchangeDeclare(model, queueInfo.DeadExchangeName, queueInfo.DeadExchangeType, queueInfo.Durable);
                QueueDeclare(model, queueInfo.DeadQueueName, queueInfo.Durable);
                model.QueueBind(queueInfo.DeadQueueName, queueInfo.DeadExchangeName, queueInfo.DeadRouteKey, null);

                _modelDic[queueInfo.QueueName] = model;
                return model;
            });

            var properties = channel.CreateBasicProperties();
            properties.Expiration = (seconds*1000).ToString(); //设置消息过期时间
            properties.DeliveryMode = 2;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageObj));
            //发送消息到原队列
            channel.BasicPublish(queueInfo.ExchangeName, queueInfo.RouteKey, properties, body);
        }

        #endregion

        #region 消息接收

        public void ReceiveMessage<T>(Func<T, CancellationToken, Task> handler)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Nomal)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            var channel = GetOrAddModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey,
                queueInfo.ExchangeType, queueInfo.Durable);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.QueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler); };
        }

        public void ReceiveMessage<T>(Func<T, Task> handler)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Nomal)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            var channel = GetOrAddModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey,
                queueInfo.ExchangeType, queueInfo.Durable);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.QueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler); };
        }

        /// <summary>
        ///     接收消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueInfo"></param>
        /// <param name="handler"></param>
        public void ReceiveMessage<T>(RabbitMqQueueModel queueInfo, Func<T, CancellationToken, Task> handler)
        {
            if (queueInfo == null)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            var channel = GetOrAddModel(queueInfo.ExchangeName, queueInfo.QueueName, queueInfo.RouteKey,
                queueInfo.ExchangeType, queueInfo.Durable);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.QueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler); };
        }

        #region 延时消息

        /// <summary>
        ///     接收延迟消息，通过 通过插件rabbitmq-delayed-message-exchange实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void ReceiveMessageDelay<T>(Func<T, Task> handler)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null|| queueInfo.MessageKind!=RabbitMsgKind.Delay)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            //延迟队列参数，必须

            IDictionary<string, object> args = new Dictionary<string, object>
            {
                {"x-delayed-type", "direct"}
            };
            var channel = GetOrAddModel(queueInfo.DelayExchangeName, queueInfo.DelayQueueName, queueInfo.DelayRouteKey,
                queueInfo.DelayExchangeType, queueInfo.Durable, false, args);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.DelayQueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler); };
        }

        /// <summary>
        ///     接收延迟消息 通过死信方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void ReceiveMessageDead<T>(Func<T, Task> handler)
        {
            var queueInfo = GetRabbitMqAttribute<T>();

            if (queueInfo == null || queueInfo.MessageKind != RabbitMsgKind.Dead)
                throw new ArgumentException("消息上不具有任何特性");
            if (handler == null) throw new NullReferenceException("处理事件为null");
            var channel = GetOrAddModel(queueInfo.DeadExchangeName, queueInfo.DeadQueueName, queueInfo.DeadRouteKey,
                queueInfo.DeadExchangeType, queueInfo.Durable);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queueInfo.DeadQueueName, false, consumer);
            consumer.Received += async (model, ea) => { await _doAsync(channel, ea, handler); };
        }

        #endregion
        

        private async Task _doAsync<T>(IModel channel, BasicDeliverEventArgs ea,
            Func<T, CancellationToken, Task> normalHandler)
        {
            var body = ea.Body;
            try
            {
                var msgBody =JsonConvert.DeserializeObject<T>( Encoding.UTF8.GetString(body));
                await normalHandler(msgBody, default(CancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                channel.BasicAck(ea.DeliveryTag, false);
            }
        }


        private async Task _doAsync<T>(IModel channel, BasicDeliverEventArgs ea,
            Func<T, Task> normalHandler)
        {
            try
            {
                var msgBody =JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(ea.Body));
                await normalHandler(msgBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                channel.BasicAck(ea.DeliveryTag, false);
            }
        }

        #endregion
    }
}
