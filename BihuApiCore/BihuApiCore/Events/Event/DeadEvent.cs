using BihuApiCore.Infrastructure.Helper.RabbitMq;

namespace BihuApiCore.Events.Event
{
    /// <summary>
    /// 死信事件
    /// </summary>
    [RabbitMqQueue(MessageKind = RabbitMsgKind.Dead)]
    public class DeadEvent
    {

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
