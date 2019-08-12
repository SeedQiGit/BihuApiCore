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
        /// 测试
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

        #region 锁的使用

        /// <summary>
        /// 使用redis作为并发锁
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> RedisConcurrentLock()
        {
            string key = "test_lock1122112312311111";//如果把这个key当作锁，那么对应的token 就是他的值
            string token = DateTime.Now.ToLongTimeString() + Guid.NewGuid().ToString();
            //await _database.StringSetAsync(key, value, TimeSpan.FromMinutes(5));

            var captchaValue = (await _database.StringGetAsync(key)).ToString();

            //获取锁的token  这里应该是没有的
            var userInfoLockObj1 = (await _database.LockQueryAsync(key));

            //string token = "12312";

            bool lockResult = await _database.LockTakeAsync(key, token, TimeSpan.FromSeconds(5));
            bool lockResult1 = await _database.LockTakeAsync(key, 2, TimeSpan.FromSeconds(5));
            //获取锁的token  这里应该是12312
            var userInfoLockObj2 = (await _database.LockQueryAsync(key));
            
            //await _database.StringSetAsync(key, 1, TimeSpan.FromMinutes(5));
            //上一行代码如果修改了锁的值，也就是key对应的value ，那么久无法释放锁
            bool releaselockResult = await _database.LockReleaseAsync(key, token);

            //获取锁的token  这里应该是没有的
            var userInfoLockObj3 = await _database.LockQueryAsync(key);
            bool lockResult2 = await _database.LockTakeAsync(key, 2, TimeSpan.FromSeconds(5));
            return BaseResponse.Ok();
        }
        

        #endregion



    }
}
