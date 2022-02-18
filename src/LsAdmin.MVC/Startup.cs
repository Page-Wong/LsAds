using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using LsAdmin.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;
using LsAdmin.Application;
using System.Reflection;
using LsAdmin.Utility.Service;
using Alipay.AopSdk.AspnetCore;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using LsAdmin.MVC.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.RegisterServices;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.Weixin.Entities;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen;

namespace LsAdmin.MVC
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            //初始化映射关系
            LsAdminMapper.Initialize();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //获取数据库连接字符串
            var sqlConnectionString = Configuration.GetConnectionString("Default");

            //添加数据上下文
            //services.AddDbContext<LsAdminDbContext>(options => options.UseNpgsql(sqlConnectionString));
            services.AddDbContext<LsAdminDbContext>(options => options.UseMySql(sqlConnectionString));
            //依赖注入
            //集中注册服务
            BatchAddScoped("LsAdmin.Application", "Service", services);
            BatchAddScoped("LsAdmin.EntityFrameworkCore", "Repository", services);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; //设置不处理循环引用
            });
            services.Configure<FormOptions>(x => {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });
            /*services.AddMvc().AddDataAnnotationsLocalization(options => {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(AnnotationsLocalizationResource));
            });*/
            //Session服务
            services.AddSession();
            services.AddMemoryCache();
            //支付宝支付服务 
            //TODO 上线前 转为正式支付
            /*services.AddAlipay(options => {
                options.AlipayPublicKey = Configuration["Alipay:AlipayPublicKey"];
                options.AppId = Configuration["Alipay:AppId"];
                options.CharSet = Configuration["Alipay:CharSet"];
                options.Gatewayurl = Configuration["Alipay:Gatewayurl"];
                options.PrivateKey = Configuration["Alipay:PrivateKey"];
                options.SignType = Configuration["Alipay:SignType"];
                options.Uid = Configuration["Alipay:Uid"];
            });*/
            services.AddAlipay(options => {
                options.AlipayPublicKey = Configuration["DevAlipay:AlipayPublicKey"];
                options.AppId = Configuration["DevAlipay:AppId"];
                options.CharSet = Configuration["DevAlipay:CharSet"];
                options.Gatewayurl = Configuration["DevAlipay:Gatewayurl"];
                options.PrivateKey = Configuration["DevAlipay:PrivateKey"];
                options.SignType = Configuration["DevAlipay:SignType"];
                options.Uid = Configuration["DevAlipay:Uid"];
            });

            services.AddOptions();
            services.Configure<WebAPIAddressModel>(Configuration.GetSection("WebAPIAddressModel"));

            //定时任务
            services.AddTimedJob();

            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting) {
            loggerFactory.AddConsole();
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            loggerFactory.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                //开发环境异常处理
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //生产环境异常处理
                app.UseExceptionHandler("/System/Error");
            }
            app.UseStatusCodePages();
            app.UseStatusCodePagesWithReExecute("/System/Error/{0}");

            //使用静态文件
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory())
            });

            //Session
            app.UseSession();
            //使用Mvc，设置默认路由为系统登录
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.Initialize(app.ApplicationServices); //初始化数据
            HttpHelper.ServiceProvider = app.ApplicationServices;

            //定时任务
            app.UseTimedJob();

            #region 注册微信服务
            // 启动 CO2NET 全局注册，必须！
            IRegisterService register = RegisterService.Start(env, senparcSetting.Value)
                                                        //关于 UseSenparcGlobal() 的更多用法见 CO2NET Demo：https://github.com/Senparc/Senparc.CO2NET/blob/master/Sample/Senparc.CO2NET.Sample.netcore/Startup.cs
                                                     .UseSenparcGlobal();
            register.RegisterTraceLog(ConfigTraceLog);//配置TraceLog

            //开始注册微信信息，必须！
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value)
                //注册多个公众号或小程序（可注册多个）
                .RegisterWxOpenAccount(senparcWeixinSetting.Value, "【设备主管理】小程序")
                ;
            #endregion
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

        /// <summary>
        /// 配置微信跟踪日志
        /// </summary>
        private void ConfigTraceLog() {
            //这里设为Debug状态时，/App_Data/WeixinTraceLog/目录下会生成日志文件记录所有的API请求日志，正式发布版本建议关闭

            //如果全局的IsDebug（Senparc.CO2NET.Config.IsDebug）为false，此处可以单独设置true，否则自动为true
            Senparc.CO2NET.Trace.SenparcTrace.SendCustomLog("系统日志", "系统启动");//只在Senparc.Weixin.Config.IsDebug = true的情况下生效

            //全局自定义日志记录回调
            Senparc.CO2NET.Trace.SenparcTrace.OnLogFunc = () => {
                //加入每次触发Log后需要执行的代码
                
            };

            //当发生基于WeixinException的异常时触发
            WeixinTrace.OnWeixinExceptionFunc = ex => {
                //加入每次触发WeixinExceptionLog后需要执行的代码
                
            };
        }
    }

}
