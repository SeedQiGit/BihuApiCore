using BihuApiCore.Infrastructure.Helper.RabbitMq;

namespace BihuApiCore.Events.Event
{
    /// <summary>
    /// 普通事件
    /// </summary>
    [RabbitMqQueue(MessageKind = RabbitMsgKind.Nomal)]
    public class NomalEvent
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
