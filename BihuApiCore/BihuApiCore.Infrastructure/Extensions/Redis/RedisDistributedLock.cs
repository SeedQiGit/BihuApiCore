using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Extensions.Redis
{
    public class RedisDistributedLock
    {
         /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <param name="database">redis database实例</param>
        /// <param name="lockName">自定义锁的key</param>
        /// <param name="lockValue">自定义锁的value</param>
        /// <param name="expiredTimeout">锁的过期时间</param>
        /// <param name="acquireTimeout">获取锁的请求时长</param>
        /// <param name="sleepWaitLockTime">重复尝试间隔(毫秒)</param>
        /// <returns></returns>
        public static async Task<bool> TryAcquire(IDatabase database, string lockName, string lockValue, TimeSpan expiredTimeout, TimeSpan? acquireTimeout = null, int sleepWaitLockTime = 50)
        {
            bool bLock = false;
            var dtStart = DateTime.Now.Ticks;
            while (!bLock)
            {
                bLock = await TryAcquireOnce(lockName, lockValue, expiredTimeout, database);
                if (acquireTimeout == null)
                {
                    break;
                }
                if (!bLock)
                {
                    Thread.Sleep(sleepWaitLockTime);
                }

                var ts = new TimeSpan(DateTime.Now.Ticks - dtStart);
                if (ts >= acquireTimeout)
                {
                    break;
                }
            }

            return bLock;
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="database">redis实例</param>
        /// <param name="lockName">自定义锁的key</param>
        /// <param name="lockValue">自定义锁的value</param>
        /// <returns></returns>
        public static async Task TryRelease(IDatabase database, string lockName, string lockValue)
        {
            try
            {
                var bRtn = await database.LockReleaseAsync(lockName, lockValue);
            }
            catch (Exception e)
            {
                // await database.StringIncrementAsync("error",1);               
            }
        }


        private static async Task<bool> TryAcquireOnce(string lockName, string lockValue, TimeSpan expiredTimeout, IDatabase database)
        {
            try
            {
                var @lock = await database.LockTakeAsync(lockName, lockValue, expiredTimeout);
                return @lock;
            }
            catch (Exception e)
            {
                //await database.StringIncrementAsync("error", 1);
                return false;
            }
        }
    }
}
