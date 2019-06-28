using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class RedisController:BaseController
    {
        private readonly IDatabase _database;

        public RedisController( RedisCacheClient client )
        {
            _database = client.GetDatabase();
        }

        /// <summary>
        /// 测试get 同步
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> Test()
        {
            string key = "test";
            string value = DateTime.Now.ToLongTimeString() + Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, value, TimeSpan.FromMinutes(5));

            var captchaValue = (await _database.StringGetAsync(key)).ToString();
            return BaseResponse.Ok(captchaValue);
        }
    }
}
