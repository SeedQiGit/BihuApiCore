using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BihuApiCore.Infrastructure.Helper
{
    public class HttpClientHelper
    {
        //私用的静态变量应该是线程安全的，静态方法也是线程安全的。
        private static HttpClient _client;

        //static HttpClientHelper()
        //{
        //    _client = new HttpClient() { BaseAddress = new Uri(BASE_ADDRESS) };

        //    //帮HttpClient热身
        //    _client.SendAsync(new HttpRequestMessage
        //    {
        //        Method = new HttpMethod("HEAD"),
        //        RequestUri = new Uri(BASE_ADDRESS + "/")
        //    }) .Result.EnsureSuccessStatusCode();
        //}

        public static HttpClient GetClient()
        {
            try
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.Timeout = TimeSpan.FromSeconds(200);//设置默认200s
                    _client.DefaultRequestHeaders.Connection.Add("keep-alive");
                    // 清除头部信息
                    _client.DefaultRequestHeaders.Connection.Clear();
                    // 
                    _client.DefaultRequestHeaders.ConnectionClose = false;
                    return _client;
                }
                return _client;
            }
            catch (Exception)
            {
                return new HttpClient();
            }
        }
    }
}
