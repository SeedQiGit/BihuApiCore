using BihuApiCore.Infrastructure.Extensions.UserClientExtensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace BihuApiCore.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取客户的ip地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIp(this HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        /// <summary>
        /// 获取浏览器及操作系统信息
        /// </summary>
        /// <returns></returns>
        public static UserClient GetBrowserAndOs(this HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var userAgent = context.Request.Headers["User-Agent"];
            var ua = new UserClient(userAgent);
            return ua;
        }
    }
}
