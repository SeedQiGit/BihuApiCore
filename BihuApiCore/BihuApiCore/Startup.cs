using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BihuApiCore.EntityFrameworkCore;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Repository.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;

namespace BihuApiCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EntityContext>(options =>options.UseMySql(Configuration.GetConnectionString("EntityContext")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region 依赖注入仓储和service及上下文等

            services.RegisterAssembly("BihuApiCore.Service", Lifecycle.Scoped);

            services.RegisterAssembly("BihuApiCore.Repository", Lifecycle.Scoped);
            services.AddScoped(typeof(IRepositoryBase<>), typeof(EfRepositoryBase<>));
         
            services.AddScoped<DbContext, EntityContext>();

            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=User}/{action=Test}/{id?}");
            });
            //app.UseMvc(); 
        }
    }
}
