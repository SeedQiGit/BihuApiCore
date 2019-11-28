using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace BihuApiCore.Infrastructure.Extensions
{
    public class ServiceProviderExtension
    {
        /// <summary>
        /// 这里在stratup赋值了app.ApplicationServices
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }
    }

    public class Test
    {
        public void Test1()
        {
            IHostingEnvironment env = ServiceProviderExtension.ServiceProvider.GetRequiredService<IHostingEnvironment>();
            ILogger logger = ServiceProviderServiceExtensions.GetRequiredService<ILogger<Test>>(
                ServiceProviderExtension.ServiceProvider);

        }
    }
}
