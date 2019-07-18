using System;
using System.Net.Http;

namespace BihuApiCore.Infrastructure.Helper.Http
{
    public class DefaultClient
    {
        public HttpClient Client { get; private set; }
    
        public DefaultClient(HttpClient httpClient)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(200);//设置默认200s
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            // 清除头部信息
            httpClient.DefaultRequestHeaders.Connection.Clear();
            httpClient.DefaultRequestHeaders.ConnectionClose = false;

            //httpClient.BaseAddress = new Uri("https://api.SampleClient.com/");
            //httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            Client = httpClient;
        }

    }
}
