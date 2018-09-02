using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BihuApiCore.EntityFrameworkCore;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Filters;
using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Middlewares;
using BihuApiCore.Model.Model;
using BihuApiCore.Repository.IRepository;
using BihuApiCore.Repository.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BihuApiCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// 服务添加到服务容器中
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EntityContext>(options =>options.UseMySql(Configuration.GetConnectionString("EntityContext")));
            services.AddSwaggerGen(c =>
            {
                //配置第一个Doc
                c.SwaggerDoc("v1", new Info { Title = "My API_1", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BihuApiCore.xml"));
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                    //. WithOrigins(allowOriginArr)
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    );
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region 依赖注入仓储和service及上下文等

            services.RegisterAssembly("BihuApiCore.Service", Lifecycle.Scoped);

            services.RegisterAssembly("BihuApiCore.Repository", Lifecycle.Scoped);
            services.AddScoped(typeof(IRepositoryBase<>), typeof(EfRepositoryBase<>));
         
            services.AddScoped<DbContext, EntityContext>();

            #endregion

            services.AddMvc(opt =>
            {
                // 跨域
                opt.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                //模型验证过滤器，order:数字越小的越先执行
                opt.Filters.Add(typeof(ModelVerifyFilterAttribute), 1);

            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            services.AddAutoMapper();
            #region 配置

            //获取api地址
            services.Configure<UrlModel>(Configuration.GetSection("UrlModel"));

           
            #endregion
        }

        /// <summary>
        /// 我的理解是对组建进行配置,注册中间件到管道中
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });
            app.UseSwagger();

            // 配置跨域
            app.UseCors("AllowSpecificOrigin");

            //异常处理中间件
            app.UseExceptionHandling();

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
