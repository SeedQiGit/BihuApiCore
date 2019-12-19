using BihuApiCore.Infrastructure.Helper.Http;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BihuApiCore.Controllers
{
    public class BaiduController:ApiBaseController
    {

        private readonly DefaultClient _defaultClient;
       
        public BaiduController(DefaultClient defaultClient)
        {
            _defaultClient = defaultClient;          
        }

        /// <summary>
        /// 表单请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("BaiduOauth")]
        [ProducesResponseType(typeof(BaseResponse), 1)]
        public async Task<BaseResponse> BaiduOauth()
        {				
            String url = "https://aip.baidubce.com/oauth/2.0/token";
            List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", "dWEp78bfAvpGmcqKe7a5qY8y"));
            paraList.Add(new KeyValuePair<string, string>("client_secret", "XHz3WLzlbSInHfaLNUe4IGTgvFBvjopB"));

            string result =await _defaultClient.Client.HttpClientFromAsync(url,paraList);
            var jObject = JObject.Parse(result);

            if ( jObject["error"]!=null)
            {
                return BaseResponse.Failed(jObject["error"].ToString()+";"+jObject["error_description"].ToString());
            }; 

            return BaseResponse.Ok(result);
        }


    }
}
