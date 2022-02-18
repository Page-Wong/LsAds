using AutoMapper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RegistEquipment.Application.DataModel;
using RegistEquipment.Application.RegistEquipmentApp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RegistEquipment.Application
{
    public static class RegistEquipmentExtensions
    {
        public static IServiceCollection AddRegistEquipmentApp(this IServiceCollection services)
        {
            services.AddSingleton<IRegistEquipmentAppService, RegistEquipmentAppService>();
            services.AddScoped<IRegistEquipmentAppHandler, RegistEquipmentAppHandler>();
            services.AddTimedJob();
           

            return services;
        }

        public static IApplicationBuilder MapRegistEquipmentApp(this IApplicationBuilder app,
                                                                PathString path)
        {
            app.UseTimedJob();
            app.Map(path, (_app) => _app.UseMiddleware<RegistEquipmentMiddleware>());
            return app;
        }
    }
}
