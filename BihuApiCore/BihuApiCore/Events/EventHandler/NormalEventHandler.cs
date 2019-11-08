using BihuApiCore.Events.Event;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Infrastructure.Helper.RabbitMq;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BihuApiCore.Events.EventHandler
{
    /// <summary>
    /// 
    /// </summary>
    public class NormalEventHandler : BaseRabbitListener<NormalEvent>
    {
        
       
        public NormalEventHandler(RabbitMqClient mqClient) : base(mqClient)
        {
          
        }

        public override async Task Handle(NormalEvent message)
        {
            LogHelper.Info("收到信息："+JsonConvert.SerializeObject(message)+"时间："+DateTime.Now);
        }

    }
}
