using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BihuApiCore.Infrastructure.Helper;

namespace BihuApiCore.Middlewares
{
    public class HttpLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLogMiddleware> _logger;

        public HttpLogMiddleware(RequestDelegate next, ILogger<HttpLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //if (expr)
            //{
                
            //}

            using (var ms = new MemoryStream())
            {
                var orgBodyStream = context.Response.Body;
                context.Items["Stream"] = orgBodyStream;
                context.Response.Body = ms;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                StringBuilder logSb = new StringBuilder();
                var requestUri = context.Request.Host.ToString() + context.Request.Path.ToString() + context.Request.QueryString.Value;
                logSb.Append("请求地址：" + requestUri + Environment.NewLine);

                await _next(context);//there is running MVC

                var employeeIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "employeeId");
                if (employeeIdClaim != null)
                {
                    logSb.Append("token解析到的用户id:" + employeeIdClaim.Value + Environment.NewLine);
                }
                else
                {
                    logSb.Append("传入token："+context.Request.Headers["Authorization"]+ Environment.NewLine);
                }

                if (context.Request.Method.ToUpper() == "POST")
                {
                    var contentLength = context.Request.ContentLength;
                    if (contentLength.HasValue && contentLength > 0)
                    {
                        context.Request.EnableRewind();
                        using (var mem = new MemoryStream())
                        {
                            context.Request.Body.Position = 0;
                            context.Request.Body.CopyTo(mem);
                            mem.Position = 0;
                            using (StreamReader reader = new StreamReader(mem, Encoding.UTF8))
                            {
                                logSb.Append("请求参数：" + reader.ReadToEnd() + Environment.NewLine);
                            }
                        }
                    }
                }

                try
                {
                    using (var sr = new StreamReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        stopWatch.Stop();
                        logSb.Append($"[接口执行时间(ms):{stopWatch.Elapsed.TotalMilliseconds}]" + Environment.NewLine);

                        _logger.LogInformation(logSb + "请求返回值：" + sr.ReadToEnd());
                        //将原有的返回值复制给基类stream
                        ms.Seek(0, SeekOrigin.Begin);
                        await ms.CopyToAsync(orgBodyStream);
                        context.Response.Body = orgBodyStream;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("异常信息：" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
                    throw;
                }
               
            }
        }
    }
    public static class HttpLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpLogMiddleware>();
        }
    }
}
