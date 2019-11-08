using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Infrastructure.Helper.Http;
using BihuApiCore.Model.Request;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace BihuApiCore.Service.Implementations
{
    public class HttpService:IHttpService
    {
        private readonly DefaultClient _defaultClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpService(DefaultClient defaultClient,IHttpClientFactory httpClientFactory)
        {
            _defaultClient = defaultClient;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BaseResponse> FormRequest()
        {
            FormRequest request = new FormRequest
            {
                grant_type="password",
                username="maxiaocui",
                password = "102",
                scope="employee_center",
                client_id="wechat",
                client_secret="secret"
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
    }
}
