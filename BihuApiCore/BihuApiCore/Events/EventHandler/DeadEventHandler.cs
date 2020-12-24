using BihuApiCore.Events.Event;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Infrastructure.Helper.RabbitMq;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Events.EventHandler
{
    public class DeadEventHandler : BaseRabbitListener<DeadEvent>
    {
        public DeadEventHandler(RabbitMqClient mqClient) : base(mqClient)
        {
          
        }

        public override async Task Handle(DeadEvent message, CancellationToken cancellationToken)
        {
            LogHelper.Info("收到信息："+JsonConvert.SerializeObject(message)+"时间："+DateTime.Now);
        }
    }
}
