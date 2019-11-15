using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Extensions.Redis
{
    public static class RedisDistributedLock
    {
        /// <summary>
        /// 获取分布式锁
        /// 释放直接使用原生 await database.LockReleaseAsync(lockName, lockValue);
        /// </summary>
        /// <param name="database">redis database实例</param>
        /// <param name="lockName">自定义锁的key</param>
        /// <param name="lockValue">自定义锁的value 释放锁的时候要用</param>
        /// <param name="expiredTimeout">锁的过期时间</param>
        /// <param name="acquireTimeout">获取锁的请求时长</param>
        /// <param name="sleepWaitLockTime">重复尝试间隔(毫秒)</param>
        /// <returns></returns>
        public static async Task<bool> TryAcquire(this IDatabase database, string lockName, string lockValue, TimeSpan expiredTimeout, TimeSpan? acquireTimeout = null, int sleepWaitLockTime = 300)
        {
            bool bLock = false;
            var dtStart = DateTime.Now.Ticks;
            while (!bLock)
            {
                bLock =  await database.LockTakeAsync(lockName, lockValue, expiredTimeout);
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
    }
}
