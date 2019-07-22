using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Helper.Http
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// 如果不成功，返回空值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url">发送链接</param>
        /// <returns></returns>
        public static async Task<string> HelperGetAsync(this HttpClient client,string url)
        {
            Task<string> result = Task.FromResult(string.Empty);
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync();
            }
            return await result;
        }

        /// <summary>
        /// 如果不成功，返回空值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="postData">发送数据的字符串格式</param>
        /// <returns></returns>
        public static async Task<string> HelperPsotAsync(this HttpClient client,string url,string postData)
        {
            Task<string> result = Task.FromResult(string.Empty);
            HttpContent content = new StringContent(postData);
            MediaTypeHeaderValue typeHeader = new MediaTypeHeaderValue("application/json")
            {
                CharSet = "UTF-8"
            };
            content.Headers.ContentType = typeHeader;

            //#region  增加tls设置

            //ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            //#endregion

            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync();
            }
            return await result;           
        }
    }
}
