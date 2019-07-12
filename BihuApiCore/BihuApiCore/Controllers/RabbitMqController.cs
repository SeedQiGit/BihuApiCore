using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class RabbitMqController:BaseController
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMqController( ConnectionFactory connectionFactory  )
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Send hello world
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> Send()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明队列  hello为队列名
                //声明队列是一次性的，只有当它不存在时才会被创建
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,//exclusive 排他的
                    autoDelete: false,
                    arguments: null);
                string message = "Hello World!";
                //Bytes流发送过去,可以使用任意编码模式，对应的解码模式相同即可
                var body = Encoding.UTF8.GetBytes(message);

                //发送消息  通过hello这个管道    
                channel.BasicPublish(exchange: "",
                    routingKey: "hello",
                    basicProperties: null,
                    body: body);
                LogHelper.Info($"已发送:{message}");
            }
            return BaseResponse.Ok();
        }

        /// <summary>
        /// SendDirect
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> SendDirect(string routingKey)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                    type: "direct");

                var body = Encoding.UTF8.GetBytes("direct Message!");
                channel.BasicPublish(exchange: "direct_logs",
                    routingKey: routingKey??"info",
                    basicProperties: null,
                    body: body);
                LogHelper.Info($"已发送:{routingKey??"info"},direct Message!");
            }
            return BaseResponse.Ok();
        }
        
    }
}
