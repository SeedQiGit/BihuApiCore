using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Infrastructure.Extensions.Redis;
using BihuApiCore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Controllers
{
    public class RedisController:BaseController
    {
        private readonly IDatabase _database;
        private readonly RedisCacheClient _client;
        private readonly ConnectionMultiplexer _redis;
        private readonly ILogger<RedisController> _logger;

        public RedisController( RedisCacheClient client , ConnectionMultiplexer redis, ILogger<RedisController> logger)
        {
            //这种获取每次都要连接一次redis
            //可以考虑实际使用的时候再GetDatabase
            _database = client.GetDatabase();
            _client = client;

            //直接ConnectionMultiplexer单例。
            _redis = redis;
            _database = _redis.GetDatabase();
            _logger = logger;
        }


        #region CSRedis

        #region Lock

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> CSRedisLock()
        {
            var lockKey = "test";

            List<Task> taskList = new List<Task>();

            var w = Task.Run(async () =>
            {
                var rlock = RedisHelper.Lock(lockKey, 10);
                if (rlock != null)
                {
                    _logger.LogInformation("成功获取锁");
                }
                else 
                {
                    _logger.LogInformation("获取锁失败");
                }
                Thread.Sleep(10000);
                //不释放的情况下，再次请求这个接口，会获取不到锁。具体什么时候自动释放，不确定。应该跟那个看门狗线程或者是c#线程回收有关。但是这种不受控的情况还是避免发生，所以要释放。
                rlock?.Unlock();
            });

            taskList.Add(w);

            var a = Task.Run(async () =>
            {
                Thread.Sleep(1000);
                var rlock = RedisHelper.Lock(lockKey, 5);
                if (rlock != null)
                {
                    _logger.LogInformation("获取锁成功1");
                }
                else
                {
                    _logger.LogInformation("获取锁失败1");
                }
                Thread.Sleep(5000);
                rlock?.Unlock();
                var rlock2 = RedisHelper.Lock(lockKey, 5);
                if (rlock2 != null)
                {
                    _logger.LogInformation("获取锁成功2");
                }
                else
                {
                    _logger.LogInformation("获取锁失败2");
                }
                rlock2?.Unlock();
            });

            taskList.Add(a);
            for (int i = 0; i < 2; i++)
            {
                
            }
            await Task.WhenAll(taskList);

            return BaseResponse.Ok();
        }

        #endregion

        #endregion


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

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> TestConnectionMultiplexer()
        {
            var database = _redis.GetDatabase();
            string key = "test";
            string value = DateTime.Now.ToLongTimeString() + Guid.NewGuid().ToString();
            await database.StringSetAsync(key, value, TimeSpan.FromMinutes(5));

            var captchaValue = (await database.StringGetAsync(key)).ToString();
            return BaseResponse.Ok(captchaValue);
        }


        #region 锁的使用

        /// <summary>
        /// 使用redis作为并发锁
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> RedisDistributedLock()
        {
            string lockKey = "test_" +   DateTime.Now.ToString("yyyy-MM-dd");
            string token = DateTime.Now.ToLongTimeString() + Guid.NewGuid().ToString();
  
            var res = await _database.TryAcquire(lockKey,token,TimeSpan.FromSeconds(30),TimeSpan.FromSeconds(10),500);

            if (res)
            {
                //先查再增数据库
            }

            bool release = await _database.LockReleaseAsync(lockKey, token);

            return BaseResponse.Ok();
        }

        /// <summary>
        /// 使用redis作为并发锁
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<BaseResponse> RedisDistributedLockMy()
        {
            string lockKey = "test_" +   DateTime.Now.ToString("yyyy-MM-dd");
            string token = DateTime.Now.ToLongTimeString() + Guid.NewGuid().ToString();
            bool lockRedis = false;

            while (!lockRedis)//这个是一直获取的写法
            {
                lockRedis = await _database.LockTakeAsync(lockKey, token, TimeSpan.FromSeconds(20));
                if (!lockRedis)
                {
                    Thread.Sleep(3000);//等待3s
                }
            }

            //先查再增数据库
            
            await _database.LockReleaseAsync(lockKey, token);

            bool release = await _database.LockReleaseAsync(lockKey, token);

            return BaseResponse.Ok();
        }

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
