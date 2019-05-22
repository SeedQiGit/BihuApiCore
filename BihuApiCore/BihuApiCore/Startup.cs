﻿using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Filters;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Middlewares;
using BihuApiCore.Model;
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
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace BihuApiCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// 服务添加到服务容器中
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<bihu_apicoreContext>(options => options.UseMySql(Configuration.GetConnectionString("EntityContext")).UseLoggerFactory(LogHelper.LoggerFactorySingleton));
            // Warning: Do not create a new ILoggerFactory instance each time

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

            #region 依赖注入仓储和service及上下文等

            services.RegisterAssembly("BihuApiCore.Service", Lifecycle.Scoped);

            services.RegisterAssembly("BihuApiCore.Repository", Lifecycle.Scoped);
            services.AddScoped(typeof(IRepositoryBase<>), typeof(EfRepositoryBase<>));
         
            services.AddScoped<DbContext, bihu_apicoreContext>();

            #endregion

            services.AddMvc(opt =>
            {
                // 跨域
                opt.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                //模型验证过滤器，order:数字越小的越先执行
                opt.Filters.Add(typeof(ModelVerifyFilterAttribute), 1);
                //日志记录，全局使用  已经直接使用中间件了
                //opt.Filters.Add(new LogAttribute());
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //空值处理
                options.SerializerSettings.ContractResolver = new NullToEmptyStringResolver();
            });

            //services.AddAutoMapper();  这里使用另一种automapper的注入方式
            AutoMapper.IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoProfile>();
            });
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            #region 配置

            //获取api地址
            services.Configure<UrlModel>(Configuration.GetSection("UrlModel"));
            //上面语句类似于 services.AddSingleton<UrlModel>(Configuration.GetSection("UrlModel"));

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
            app.UseBufferedResponseBody();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=User}/{action=Test}/{id?}");
            });
          
            //app.UseMvc(); 
            //HttpClientHelper.WarmUpClient();
        }
    }
}
