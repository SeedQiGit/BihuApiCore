using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Filters;
using BihuApiCore.Infrastructure.Configuration;
using BihuApiCore.Infrastructure.Extensions;
using BihuApiCore.Infrastructure.Helper;
using BihuApiCore.Middlewares;
using BihuApiCore.Model;
using BihuApiCore.Model.Enums;
using BihuApiCore.Model.Response;
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
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BihuApiCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
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

            //全局日志工厂，之后使用依赖注入的Ilogger都是这个工厂生成的。
            services.AddSingleton(LogHelper.LoggerFactorySingleton);

            services.AddHttpContextAccessor();

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

            #region 限流，这个插件还不是很成熟

            //Api限流
            //services.AddApiThrottle(options => {
            //    //配置redis
            //    //如果Cache和Storage使用同一个redis，则可以按如下配置
            //    options.UseRedisCacheAndStorage(opts => {
            //        opts.ConnectionString =Configuration["ApiThrottleConnectionString"];
            //        //opts.KeyPrefix = "apithrottle"; //指定给所有key加上前缀，默认为apithrottle
            //    });
            //    //如果Cache和Storage使用不同redis库，可以按如下配置
            //    //options.UseRedisCache(opts => {
            //    //    opts.ConnectionString = "localhost,connectTimeout=5000,allowAdmin=false,defaultDatabase=0";
            //    //});
            //    //options.UseRedisStorage(opts => {
            //    //    opts.ConnectionString = "localhost,connectTimeout=5000,allowAdmin=false,defaultDatabase=1";
            //    //});

            //    options.onIntercepted = (context, valve, where) =>
            //    {
            //        //valve：引发拦截的valve
            //        //where：拦截发生的地方，有ActionFilter,PageFilter,Middleware(全局)
            //        context.Response.StatusCode = 429;
            //        return new JsonResult(new BaseResponse{ Code = (int)BusinessStatusType.FrequencyRequest, Message = "访问过于频繁，请稍后重试！" });

            //    };
            //});

            #endregion

            services.AddMvc(opt =>
            {
                // 跨域
                opt.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
                //模型验证过滤器，order:数字越小的越先执行
                opt.Filters.Add(typeof(ModelVerifyFilterAttribute), 1);
                //日志记录，全局使用  已经直接使用中间件了
                //opt.Filters.Add(new LogAttribute());
                //这里添加ApiThrottleActionFilter拦截器
                //opt.Filters.Add(typeof(ApiThrottleActionFilter));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //空值处理
                options.SerializerSettings.ContractResolver = new NullToEmptyStringResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .ConfigureApiBehaviorOptions(options =>
            {
                //options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = context =>
                {
                    //var problemDetails = new ValidationProblemDetails(context.ModelState)
                    //{
                    //	Type = "https://contoso.com/probs/modelvalidation",
                    //	Title = "One or more model validation errors occurred.",
                    //	Status = StatusCodes.Status400BadRequest,
                    //	Detail = "See the errors property for details.",
                    //	Instance = context.HttpContext.Request.Path
                    //};

                    var validationErrors = context.ModelState
                        .Keys
                        .SelectMany(k => context.ModelState[k].Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();
                    var json = BaseResponse.GetBaseResponse(BusinessStatusType.ParameterError, string.Join(",", validationErrors));

                    return new BadRequestObjectResult(json)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            })
            .AddControllersAsServices();


            //services.AddAutoMapper();  这里使用另一种automapper的注入方式
            AutoMapper.IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoProfile>();
            });
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            #region redis配置

            var section = Configuration.GetSection("RedisCacheSettings");
            string connectionString = section.GetSection("RedisConnectionString").Value;
            string instanceName = section.GetSection("InstanceName").Value;
            int defaultDb = int.Parse(section.GetSection("Database").Value ?? "0");
            services.AddSingleton(new RedisCacheClient(connectionString, instanceName, defaultDb));

            #endregion

            #region 配置rabbitmq

            services.AddRabbitmq(Configuration);

            #endregion

            //返回压缩
            services.AddResponseCompression();

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
            //这个尽量放在最外层，不然会对你的返回值进行压缩
            app.UseResponseCompression();

            //loggerFactory.AddNLog();
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

            //Api限流
            //app.UseApiThrottle();

            //日志记录中间件  先注册这个，其他的中间件后注册
            app.UseHttpLogMiddleware();
            //异常处理中间件
            app.UseExceptionHandling();
           
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=User}/{action=Test}/{id?}");
            });
            ConfigureRabbitMqDirect(app);
            app.UseMvc();
            //HttpClientHelper.WarmUpClient();
        }

        #region 配置mq订阅

        /// <summary>
        /// 配置mq订阅
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureRabbitMq(IApplicationBuilder app)
        {
            var connectionFactory = app.ApplicationServices.GetRequiredService<ConnectionFactory>();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            //因为它会异步推送我们的消息，我们提供了一个回调。那是EventingBasicConsumer接收事件处理程序所做的
            var consumer = new EventingBasicConsumer(channel);
            //订阅事件  接受到事件对象consumer
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                LogHelper.Info($" [x] Received {message}" );
            };
            //用来监听channel，触发EventingBasicConsumer
            //这里原来是noAck  我改成autoAck
            //acknowledgment 是 consumer 告诉 broker 当前消息是否成功 consume，至于 broker 如何处理 NACK，取决于 consumer 是否设置了 
            channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// 配置mq订阅Direct模式
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureRabbitMqDirect(IApplicationBuilder app)
        {
            var connectionFactory = app.ApplicationServices.GetRequiredService<ConnectionFactory>();
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "direct_logs",
                type: "direct");
            var queueName = channel.QueueDeclare().QueueName;

            //为每个我们感兴趣的严重性创建一个新的绑定。只有我们给定的状态值才会接受
            foreach (var severity in new List<string>{"info","error"})
            {
                //QueueBind 绑定键，针对特定的routingKey进行绑定
                channel.QueueBind(queue: queueName,
                    exchange: "direct_logs",
                    routingKey: severity);
            }       
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                LogHelper.Info($" [x] Received {routingKey} ,{message}" );
            };
            //注册消费者
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        #endregion
        
    }
    public static class CustomExtensionsMethods
    {
        /// <summary>
        /// 个性化集成
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitmq(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("RabbitMqSettings");
            string connectionString = section.GetSection("RabbitMqConnection").Value;
            string userName = section.GetSection("RabbitMqUserName").Value;
            string password = section.GetSection("RabbitMqPwd").Value;
            
            var factory = new ConnectionFactory()
            {
                HostName = connectionString
            };

            if (!string.IsNullOrEmpty(userName))
            {
                factory.UserName = userName;
            }

            if (!string.IsNullOrEmpty(password))
            {
                factory.Password = password;
            }
            services.AddSingleton(factory);
            return services;
        }
    }
}
