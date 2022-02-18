using LsAdmin.Application.DepartmentApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.DepartmentApp
{
    public interface IDepartmentAppService : IBaseAppService<DepartmentDto> {        

        /// <summary>
        /// 根据父级Id获取子级列表
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<DepartmentDto> GetChildrenByParent(Guid parentId, int startPage, int pageSize, out int rowCount);       

        /// <summary>
        /// 根据Code获取实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        DepartmentDto FindByCode(string code);

        /// <summary>
        /// 获取默认实体
        /// </summary>
        /// <returns></returns>
        DepartmentDto GetDefualt();
    }
}
