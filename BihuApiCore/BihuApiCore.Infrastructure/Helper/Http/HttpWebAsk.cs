using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Helper
{
    public static class HttpWebAsk
    {
        #region form请求

        /// <summary>
        /// 模拟表单请求，不能上传文件 （上传文件方法需要自己写） 只能处理简单类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<string> FormRequest<T>(this HttpClient client, string url, T value)
        {
            var formData = new MultipartFormDataContent();
            var modelType = typeof(T);

            #region 处理form表单

            //遍历所有成员 类型统一按字符串处理(DateTime,Enum;long ;bool;int...)
            foreach (var item in modelType.GetProperties())
            {
                HttpContent content = new StringContent(item.GetValue(value).ToString());
                formData.Add(content, item.Name);
            }

            #endregion
            Task<string> result = Task.FromResult(string.Empty);
            HttpResponseMessage response = await client.PostAsync(url, formData);

            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync();
            }
            return await result;
        }

        #endregion


        public static async Task<string> HttpClientPostAsync(string postData, string url)
        {
            var client = HttpClientHelper.GetClient();
            Task<string> result = Task.FromResult(string.Empty);
            HttpContent content = new StringContent(postData);
            MediaTypeHeaderValue typeHeader = new MediaTypeHeaderValue("application/json");
            typeHeader.CharSet = "UTF-8";
            content.Headers.ContentType = typeHeader;

            #region  增加tls设置

            ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            #endregion

            HttpResponseMessage response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync();
            }
            return await result;
        }

        public static async Task<string> HttpClientGetAsync(string url)
        {
            var client = HttpClientHelper.GetClient();
            Task<string> result = Task.FromResult(string.Empty);
            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync();
            }
            return await result;
        }
    }
}
