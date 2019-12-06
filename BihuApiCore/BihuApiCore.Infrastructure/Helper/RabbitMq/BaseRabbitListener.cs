using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Helper.RabbitMq
{
    /// <summary>
    ///     RabbitMQ消息监听基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRabbitListener<T> : IHostedService where T : class
    {
        #region 构造函数及字段

        protected RabbitMqClient MqClient;
        protected RabbitMsgKind MessageKind;

        public BaseRabbitListener(RabbitMqClient mqClient)
        {
            this.MqClient = mqClient;
            GetMessageKind();
        }

        private void GetMessageKind()
        {
            var result1 = Attribute.GetCustomAttribute(typeof(T), typeof(RabbitMqQueueAttribute));
            var result = result1 != null ? result1 as RabbitMqQueueAttribute : new RabbitMqQueueAttribute();
            MessageKind = result.MessageKind;
        }

        #endregion

        #region 服务启动或停止

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //    channel.close();
            //connection.close();


            //channel
            //this.connection.Close();
            return Task.CompletedTask;
        }

        #endregion

        #region 消息监听接口

        /// <summary>
        ///     消费者处理事件
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual Task Handle(T message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     注册消费者监听
        /// </summary>
        /// <param name="cancellationToken"></param>
        protected void Register(CancellationToken cancellationToken)
        {
            Func<T, Task> handler = Handle;
            switch (MessageKind)
            {
                case RabbitMsgKind.Normal:
                    MqClient.ReceiveMessage(handler);
                    break;
                case RabbitMsgKind.Delay:
                    MqClient.ReceiveMessageDelay(handler);
                    break;
                case RabbitMsgKind.Dead:
                    MqClient.ReceiveMessageDead(handler);
                    break;
            }
        }

        #endregion
    }
}
