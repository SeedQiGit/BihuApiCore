using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Helper.RabbitMq;

namespace BihuApiCore.Events.Event
{
    /// <summary>
    /// 死信事件
    /// </summary>
    [RabbitMqQueue(MessageKind = RabbitMsgKind.Delay)]
    public class DelayEvent
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
