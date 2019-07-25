using System.IO;
using System.Net;
using BihuApiCore.Infrastructure.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BihuApiCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if !DEBUG
                .UseKestrel(opts =>
                {
                    opts.Listen(IPAddress.Parse(ConfigurationManager.GetValue("HostIp")), ConfigurationManager.GetValue<int>("Port"));
                    opts.Listen(IPAddress.Parse(ConfigurationManager.GetValue("HostIp")), ConfigurationManager.GetValue<int>("SPort"), lopts =>
                    {
                        lopts.UseHttps(Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.GetValue("File"))  , ConfigurationManager.GetValue("Password"));
                    });
                })
#endif
                //.UseUrls(ConfigurationManager.GetValue("HostUrl").Split(','))
                .UseStartup<Startup>()
              
                ;
    }
}
