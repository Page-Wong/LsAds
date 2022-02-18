using LsAdmin.Application.Imp;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.RoleApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.RoleApp
{
    public interface IRoleAppService : IBaseAppService<RoleDto> {        

        /// <summary>
        /// 根据角色获取权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <returns></returns>
        List<Guid> GetAllMenuListByRole(Guid roleId);

        /// <summary>
        /// 更新角色权限关联关系
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="roleMenus">角色权限集合</param>
        /// <returns></returns>
        bool UpdateRoleMenu(Guid roleId, List<RoleMenuDto> roleMenus);

        /// <summary>
        /// 根据Code获取实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        RoleDto GetByCode(string code);
    }
}
