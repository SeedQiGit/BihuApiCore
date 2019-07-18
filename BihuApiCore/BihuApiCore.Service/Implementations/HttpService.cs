using System;
using System.IO;
using System.Net.Http;
using BihuApiCore.Infrastructure.Helper.Http;
using BihuApiCore.Model.Response;
using BihuApiCore.Service.Interfaces;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Model.Request;

namespace BihuApiCore.Service.Implementations
{
    public class HttpService:IHttpService
    {

        private readonly DefaultClient _defaultClient;

        public HttpService(DefaultClient defaultClient)
        {
            _defaultClient = defaultClient;
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
            var response = await _defaultClient.Client.FormRequest("http://identity.91bihu.me/connect/token", request);

            return BaseResponse.Ok(response);
        }
    }
}
