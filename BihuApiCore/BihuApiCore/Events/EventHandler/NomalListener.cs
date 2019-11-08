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
    public class NomalListener : BaseRabbitListener<NomalEvent>
    {
        
       
        public NomalListener(RabbitMqClient mqClient) : base(mqClient)
        {
          
        }

        public override async Task Handle(NomalEvent message)
        {
            LogHelper.Info("收到信息："+JsonConvert.SerializeObject(message)+"时间："+DateTime.Now);
        }

    }
}
