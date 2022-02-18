using ActiveEquipment.Application.ActiveEquipmentApp;
using ActiveEquipment.Application.Common;
using ActiveEquipment.Application.InstructionApp;
using AutoMapper;
using AutoMapper.Configuration;
using LsAdmin.Utility.Services.WebSocketService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ActiveEquipment.Application {
    public static class ActiveEquipmentExtensions {
        public static IServiceCollection AddActiveEquipmentApp(this IServiceCollection services) {
            MapperHelper.Initialize();
            services.AddSingleton<IActiveEquipmentAppService, ActiveEquipmentAppService>();
            services.AddScoped<IActiveEquipmentAppHandler, ActiveEquipmentAppHandler>();
            services.AddScoped<IInstructionAppHandler, InstructionAppHandler>();
            services.AddTimedJob();

            return services;
        }

        public static IApplicationBuilder MapActiveEquipmentApp(this IApplicationBuilder app,
                                                                PathString path) {
            app.UseTimedJob();
            app.Map(path, (_app) => _app.UseMiddleware<ActiveEquipmentMiddleware>());
            return app;
        }

    }

}