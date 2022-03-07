using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Savorboard.CAP.InMemoryMessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCAPDemo.Filter;
using DotNetCore.CAP;

namespace WebAppCAPDemo
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
            services.AddDbContext<CAPDbcontext>(option =>
            {
                option.UseMySQL("Server=42.192.3.5;port=3306;Database=testcap;User ID=root;Password=123456;charset=utf8;Allow User Variables=true");
            });

            services.AddCap(x =>
            {
                //UI 默认地址是 /cap
                x.UseDashboard();

                //默认分组名称
                //x.DefaultGroupName

                //失败后的重试次数
                x.FailedRetryCount = 1;

                //失败重试间隔
                x.FailedRetryInterval = 24 * 3600;

                //失败达到一定次数后的回调
                //x.FailedThresholdCallback = x => 
                //{
                //};

                //消息成功后在多久后过期
                //x.SucceedMessageExpiredAfter

                //x.UseMySql("Server=42.192.3.5;port=3306;Database=testcap;User ID=root;Password=123456;charset=utf8;Allow User Variables=true");

                //x.UseRabbitMQ("localhost"); //简单的配置
                //x.UseRabbitMQ(x =>
                //{
                //    x.HostName = "localhost"; // ip
                //    x.Port = 5672; // 端口
                //    x.UserName = "guest"; // 账户
                //    x.Password = "guest"; // 密码
                //    x.VirtualHost = "/"; // 虚拟主机
                //});

                //Redis Streams 可以在 CAP 中用作消息传输器
                //x.UseRedis("");

                //便于快速启动时的设置
                x.UseInMemoryMessageQueue();
                x.UseInMemoryStorage();

            }).AddSubscribeFilter<MyCapFilter>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAppCAPDemo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppCAPDemo v1"));
            }

            app.UseRouting();

            app.UseCapDashboard();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
