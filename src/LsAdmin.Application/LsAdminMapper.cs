using AutoMapper;
//using LsAdmin.Application.AndroidapkApp.Dtos;
using LsAdmin.Application.DepartmentApp.Dtos;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.PlayHistoryApp.Dtos;
using LsAdmin.Application.RoleApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Domain;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LsAdmin.Application {
    /// <summary>
    /// Enity与Dto映射
    /// </summary>
    public class LsAdminMapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {

                Assembly assembly = Assembly.Load("LsAdmin.Domain");
                List<Type> tsDomain = assembly.GetTypes().ToList();
                assembly = Assembly.Load("LsAdmin.Application");
                List<Type> tsApplication = assembly.GetTypes().ToList();
                foreach (var item in tsDomain.Where(s => s.Namespace.Contains("Entities"))) {                    
                    foreach (var dtoType in tsApplication.Where(a => a.Name.StartsWith(item.Name) && a.Name.EndsWith("Dto"))) {
                        MethodInfo mi = cfg.GetType().GetMethods().Where(m => m.Name.Equals("CreateMap") && m.GetParameters().Count() == 0).SingleOrDefault();
                        mi.MakeGenericMethod(dtoType, item).Invoke(cfg, new object[] { });
                        mi.MakeGenericMethod(item, dtoType).Invoke(cfg, new object[] { });
                    }
                }

                /*cfg.CreateMap<Menu, MenuDto>();
                cfg.CreateMap<MenuDto, Menu>();
                cfg.CreateMap<Department, DepartmentDto>();
                cfg.CreateMap<DepartmentDto, Department>();
                cfg.CreateMap<RoleDto, Role>();
                cfg.CreateMap<Role, RoleDto>();
                cfg.CreateMap<RoleMenuDto, RoleMenu>();
                cfg.CreateMap<RoleMenu, RoleMenuDto>();
                cfg.CreateMap<UserDto, User>();
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserRoleDto, UserRole>();
                cfg.CreateMap<UserRole, UserRoleDto>();

                cfg.CreateMap<Material, MaterialDto>();
                cfg.CreateMap<MaterialDto, Material>();
                cfg.CreateMap<MaterialDto, MaterialInfoDto>();
                cfg.CreateMap<MaterialInfoDto, MaterialDto>();
                cfg.CreateMap<Material, MaterialInfoDto>();
                cfg.CreateMap<MaterialInfoDto, Material>();
                cfg.CreateMap<Equipment, EquipmentDto>();
                cfg.CreateMap<EquipmentDto, Equipment>();
                cfg.CreateMap<Order, OrderDto>();
                cfg.CreateMap<OrderDto, Order>();
                cfg.CreateMap<OrderEquipment, OrderEquipmentDto>();
                cfg.CreateMap<OrderEquipmentDto, OrderEquipment>();
                cfg.CreateMap<OrderMaterial, OrderMaterialDto>();
                cfg.CreateMap<OrderMaterialDto, OrderMaterial>();
                cfg.CreateMap<Place, PlaceDto>();
                cfg.CreateMap<PlaceDto, Place>();
                cfg.CreateMap<PlayHistory, PlayHistoryDto>();
                cfg.CreateMap<PlayHistoryDto, PlayHistory>();
                cfg.CreateMap<Androidapk, AndroidapkDto>();
                cfg.CreateMap<AndroidapkDto, Androidapk>();*/
            });
        }
    }
}
