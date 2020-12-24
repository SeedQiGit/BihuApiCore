using BihuApiCore.Events.Event;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Infrastructure.Helper.RabbitMq;
using Newtonsoft.Json;
using System;
using System.Threading;
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

        public override async Task Handle(NormalEvent message,CancellationToken cancellationToken)
        {
            try
            {
                LogHelper.Info("收到信息："+JsonConvert.SerializeObject(message)+"时间："+DateTime.Now);

                Thread.Sleep(10000);
           
                //处理完成，手动确认 这个我封装在RabbitMqClient 的_doAsync 里面了
                //channel.BasicAck(ea.DeliveryTag, false);

                LogHelper.Info("处理完成");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"NormalEvent异常：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
            }
           

        }

    }
}
