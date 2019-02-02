using BihuApiCore.Infrastructure.Helper;
using Microsoft.AspNetCore.Builder;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http.Internal;

namespace BihuApiCore.Middlewares
{
    public static class BufferedResponseBodyExtensions
    {
        public static IApplicationBuilder UseBufferedResponseBody(this IApplicationBuilder builder)
        {
            return builder.Use(async (context, next) =>
            {
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

                    await next();//there is running MVC

                    if (context.Items.ContainsKey("UserId"))
                    {
                        var userId = context.Items["UserId"];
                        logSb.Append("token解析到的用户id:" + userId.ToString() + Environment.NewLine);
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

                    using (var sr = new StreamReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        stopWatch.Stop();
                        logSb.Append($"[接口执行时间(ms):{stopWatch.Elapsed.TotalMilliseconds}]"+Environment.NewLine);
                        LogHelper.Info(logSb  + "请求返回值：" + sr.ReadToEnd());
                        //将原有的返回值复制给基类stream
                        ms.Seek(0, SeekOrigin.Begin);
                        await ms.CopyToAsync(orgBodyStream);
                        context.Response.Body = orgBodyStream;
                    }
                }
            });
        }
    }
}
