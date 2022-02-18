using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using LsAdmin.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LsAdmin.Application;
using LsAdmin.Utility.Service;
using Hangfire;
using Hangfire.MySql;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LsAdmin.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //初始化映射关系
            LsAdminMapper.Initialize();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            //获取数据库连接字符串
            var sqlConnectionString = Configuration.GetConnectionString("Default");

            //添加数据上下文
            services.AddDbContext<LsAdminDbContext>(options => options.UseMySql(sqlConnectionString));
            //集中注册服务
            BatchAddScoped("LsAdmin.Application", "Service", services);
            BatchAddScoped("LsAdmin.EntityFrameworkCore", "Repository", services);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc();
            //Session服务
            services.AddSession();

            services.AddHangfire(r => r.UseStorage(new MySqlStorage(
             sqlConnectionString,
             new MySqlStorageOptions
             {
                 TransactionIsolationLevel = IsolationLevel.ReadCommitted, // - 事务隔离级别。默认是读取提交。
                QueuePollInterval = TimeSpan.FromSeconds(15),             // - 作业队列轮询间隔。默认值是15秒。
                JobExpirationCheckInterval = TimeSpan.FromHours(1),       // - 作业到期检查间隔（管理已过期的记录）。默认为1小时。
                CountersAggregateInterval = TimeSpan.FromMinutes(5),      // - 间隔计数器聚合。默认是5分钟。
                PrepareSchemaIfNecessary = true,                          // - 如果设置为true，则创建数据库表。默认是true。
                DashboardJobListLimit = 50000,                            // - 仪表板工作列表限制。默认是50000。   
                TransactionTimeout = TimeSpan.FromMinutes(1),             // - 超时。默认值是1分钟。
            })));
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else {
                //生产环境异常处理
                app.UseExceptionHandler("/System/Error");
            }

            //Session
            app.UseSession();
            app.UseMvc();
 
            app.UseHangfireServer(
              new BackgroundJobServerOptions
              {
                  WorkerCount = 1
              });
            app.UseHangfireDashboard();

            HttpHelper.ServiceProvider = app.ApplicationServices;

            BackgroundJob.Enqueue(() => new BackStageManagement.CycleRunJob().UpdateOrderPool());
            RecurringJob.AddOrUpdate(() => new BackStageManagement.CycleRunJob().UpdateOrderPool(), Cron.MinuteInterval(1));
        }

        private void BatchAddScoped(string assemblyName, string key, IServiceCollection services) {
            if (!String.IsNullOrEmpty(assemblyName)) {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();
                foreach (var item in ts.Where(s => !s.IsInterface && s.Name.EndsWith(key))) {
                    foreach (var interfaceType in item.GetInterfaces().Where(i => i.Name.Equals("I" + item.Name))) {
                        services.AddScoped(interfaceType, item);
                    }
                }
            }
        }
    }
}
