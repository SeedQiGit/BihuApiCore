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
        public static HttpClient GetClient()
        {
            try
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.Timeout = TimeSpan.FromSeconds(360);//设置默认200s
                    _client.DefaultRequestHeaders.Connection.Add("keep-alive");
                    _client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727)");
                    _client.DefaultRequestHeaders.Connection.Clear();
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
