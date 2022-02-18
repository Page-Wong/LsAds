using LsAdmin.Application.Imp;
using LsAdmin.Application.MenuApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.MenuApp
{
    public interface IMenuAppService : IBaseAppService<MenuDto> {

        /// <summary>
        /// 根据父级Id获取功能列表
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<MenuDto> GetMenusByParent(Guid parentId, int startPage, int pageSize, out int rowCount);
        
        /// <summary>
        /// 根据用户获取功能菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        List<MenuDto> GetMenusByUser(Guid userId);

        /// <summary>
        /// 根据用户获取所有菜单功能
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        List<MenuDto> GetAllMenusByUser(Guid userId);
    }
}
