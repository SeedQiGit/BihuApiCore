using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace BihuApiCore.Infrastructure.Extensions
{
    public static class DapperExtension
    {
        public static async Task<int> ExecuteAsyncRetry(this IDbConnection connection,string sql , object args,IDbTransaction transaction=null )
        {
            int count =0;
            for (int i = 0; i < 5; i++)
            {
                //这里应该是ExecuteAsync
                count=await connection.ExecuteAsyncRetry(sql, args,transaction);
                if (count>0)
                {
                    break;
                }
                //以2的n次幂去进行重试 
                //Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2,i)));
                Thread.Sleep(TimeSpan.FromSeconds(2*i+1));
            }
            return count;
        }
    }
}
