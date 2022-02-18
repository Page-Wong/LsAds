using LsAdmin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LsAdminDbContext(serviceProvider.GetRequiredService<DbContextOptions<LsAdminDbContext>>()))
            {
                if (!context.InstructionMethods.Any()) {
                    context.InstructionMethods.Add(new InstructionMethod { Name = "syncPlayerList", Method = "syncPlayerList", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "syncPlayInfoList", Method = "syncPlayInfoList", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "syncPlayInfoResourcesList", Method = "syncPlayInfoResourcesList", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "syncOperationDictionary", Method = "syncOperationDictionary", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "syncAlarm", Method = "syncAlarm", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "syncAppVersion", Method = "syncAppVersion", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "play", Method = "play", ParamRole = "[{\"Name\":\"playerId\",\"ClassType\":\"string\",\"IsRequired\":true},{\"Name\":\"playInfoId\",\"ClassType\":\"string\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "playNext", Method = "playNext", ParamRole = "[{\"Name\":\"playerId\",\"ClassType\":\"string\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setLoopPlayMode", Method = "setLoopPlayMode", ParamRole = "[{\"Name\":\"playerId\",\"ClassType\":\"string\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setSinglePlayMode", Method = "setSinglePlayMode", ParamRole = "[{\"Name\":\"playerId\",\"ClassType\":\"string\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "stopPlay", Method = "stopPlay", ParamRole = "[{\"Name\":\"playerId\",\"ClassType\":\"string\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "getAppVersion", Method = "getAppVersion", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "getSystemInfo", Method = "getSystemInfo", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "getSDCardInfo", Method = "getSDCardInfo", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "getRamInfo", Method = "getRamInfo", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "getNetworkInfo", Method = "getNetworkInfo", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "getLog", Method = "getLog", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setVolume", Method = "setVolume", ParamRole = "[{\"Name\":\"volume\",\"ClassType\":\"Int\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setScreenBrightness", Method = "setScreenBrightness", ParamRole = "[{\"Name\":\"brightness\",\"ClassType\":\"Int\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setScreenBrightnessMode", Method = "setScreenBrightnessMode", ParamRole = "[{\"Name\":\"mode\",\"ClassType\":\"Int\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setScreenOrientation", Method = "setScreenOrientation", ParamRole = "[{\"Name\":\"orientation\",\"ClassType\":\"Int\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setHumanSensor", Method = "setHumanSensor", ParamRole = "[{\"Name\":\"time\",\"ClassType\":\"Int\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setStatusBarVisibility", Method = "setStatusBarVisibility", ParamRole = "[{\"Name\":\"enable\",\"ClassType\":\"bool\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setStatusBarDisplay", Method = "setStatusBarDisplay", ParamRole = "[{\"Name\":\"enable\",\"ClassType\":\"bool\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "setNavigationBarVisibility", Method = "setNavigationBarVisibility", ParamRole = "[{\"Name\":\"enable\",\"ClassType\":\"bool\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "openWifi", Method = "openWifi", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "closeWifi", Method = "closeWifi", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "connectWifi", Method = "connectWifi", ParamRole = "[{\"Name\":\"ssid\",\"ClassType\":\"string\",\"IsRequired\":true},{\"Name\":\"password\",\"ClassType\":\"string\",\"IsRequired\":true},{\"Name\":\"type\",\"ClassType\":\"Int\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "screenshot", Method = "screenshot", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "formatData", Method = "formatData", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "scheduledOnOff", Method = "scheduledOnOff", ParamRole = "[{\"Name\":\"onTime\",\"ClassType\":\"string\",\"IsRequired\":true}, {Name:\"offTime\",\"ClassType\":\"string\",\"IsRequired\":true}]" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "turnOnScreen", Method = "turnOnScreen", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "turnOffScreen", Method = "turnOffScreen", ParamRole = "" });
                    context.InstructionMethods.Add(new InstructionMethod { Name = "reboot", Method = "reboot", ParamRole = "" });

                    context.SaveChanges();
                }

                if (context.Users.Any())
                {
                    return;   // 已经初始化过数据，直接返回
                }
                Guid departmentId = Guid.NewGuid();
                //增加一个部门
                context.Departments.Add(
                   new Department
                   {
                       Code = "defualt",
                       Id = departmentId,
                       Name = "LsAdmin集团总部",
                       ParentId = Guid.Empty
                   }
                );
                //增加一个超级管理员用户
                context.Users.Add(
                     new User {
                         UserName = "admin",
                         Password = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes("123456"))),//"123456", //暂不进行加密
                         Name = "超级管理员",
                         DepartmentId = departmentId
                     }
                );
                //增加四个基本功能菜单
                context.Menus.AddRange(
                   new Menu
                   {
                       Name = "组织机构管理",
                       Code = "Department",
                       SerialNumber = 0,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link"
                   },
                   new Menu
                   {
                       Name = "角色管理",
                       Code = "Role",
                       SerialNumber = 1,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link"
                   },
                   new Menu
                   {
                       Name = "用户管理",
                       Code = "User",
                       SerialNumber = 2,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link"
                   },
                   new Menu
                   {
                       Name = "功能管理",
                       Code = "Department",
                       SerialNumber = 3,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link"
                   }
                );
                context.SaveChanges();
            }
        }
    }
}
