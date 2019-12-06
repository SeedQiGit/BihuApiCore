using System;
using BihuApiCore.Infrastructure.Helper.Http;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Helper;

namespace BihuApiCore.Service.Implementations
{
    public class HttpService : IHttpService
    {
        private readonly DefaultClient _defaultClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpService(DefaultClient defaultClient, IHttpClientFactory httpClientFactory)
        {
            _defaultClient = defaultClient;
            _httpClientFactory = httpClientFactory;
        }

        #region 各种请求

        public async Task<BaseResponse> FormRequest()
        {
            FormRequest request = new FormRequest
            {
                grant_type = "password",
                username = "maxiaocui",
                password = "102",
                scope = "employee_center",
                client_id = "wechat",
                client_secret = "secret"
            };
            var response = await _defaultClient.Client.FormDataRequest("http://identity.91bihu.me/connect/token", request);

            return BaseResponse.Ok(response);
        }

        public async Task<BaseResponse> FormFileRequest()
        {
            //复用在Startup中定义的client_1的httpclient
            var client = _httpClientFactory.CreateClient("client_1");
            return BaseResponse.Ok();
        }

        #endregion

        #region 压力测试

        public async Task<BaseResponse> PressureTest()
        {
            var client =_httpClientFactory.CreateClient();
            string url= "http://localhost:5111/api/RabbitMq/MqClientSendNomal";
            for (int i = 0; i < 10; i++)
            {
                try
                {
#pragma warning disable 4014
                     client.HelperGetAsync(url);
#pragma warning restore 4014
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"PressureTest HelperGetAsync异常：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
                }
            }
            return BaseResponse.Ok();
        }

        #endregion


    }
}
