using BihuApiCore.Infrastructure.Helper.RabbitMq;

namespace BihuApiCore.Events.Event
{
    /// <summary>
    /// 普通事件
    /// </summary>
    [RabbitMqQueue(MessageKind = RabbitMsgKind.Normal)]
    public class NormalEvent
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
