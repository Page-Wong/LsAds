using LsAdmin.Application.Imp;
using LsAdmin.Application.MaterialApp.Dtos;
using LsAdmin.Application.MenuApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.MaterialApp {
    public interface IMaterialAppService : IBaseAppService<MaterialDto>
    {

        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <returns></returns>
        List<MaterialInfoDto> GetAllInfoList();       

        /// <summary>
        /// 获取信息分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<MaterialInfoDto> GetAllInfoPageList(int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 获取信息分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <param name="materialType">素材类型</param>
        /// <returns></returns>
        List<MaterialDto> GetPageListByType(int startPage, int pageSize, out int rowCount, ushort materialType);

        /// <summary>
        /// 根据Id获取信息实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        MaterialInfoDto GetInfo(Guid id);


        List<MaterialDto> GetOwnerPageList(int startPage, int pageSize, out int rowCount);

    }
}
