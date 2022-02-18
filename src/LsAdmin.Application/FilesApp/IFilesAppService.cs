using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Application.Imp;
using LsAdmin.Application.MenuApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.FilesApp
{
    public interface IFilesAppService : IBaseAppService<FilesDto>
    {
        /// <summary>
        /// 根据Id获取信息实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        FilesInfoDto GetInfo(Guid id);

        /// <summary>
        /// 获取信息分页列表
        /// </summary>
        /// <param name="OwnerObjId">拥有对象</param>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<FilesDto> GetOwnerObjPageList(Guid OwnerObjId, int startPage, int pageSize, out int rowCount);
        List<FilesDto> GetPageListByType(Guid OwnerObjId, int startPage, int pageSize, out int rowCount, ushort type);
        List<FilesDto> GetPageListByEquipment(Guid equipmentId, Guid placeId, int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertInfo(ref FilesInfoDto dto);


        /// <summary>
        /// 删除所有者的文件
        /// </summary>
        /// <param name="Ownerid"></param>
        void DeletetOwnerObj(Guid Ownerid);

        /// <summary>
        /// 获取文件所有者所有文件
        /// </summary>
        /// <param name="OwnerObjId"></param>
        /// <returns></returns>
         List<FilesDto> GetOwnerObj(Guid OwnerObjId);

         FilesDto GetOwnerOneObj(Guid OwnerObjId);
    }
}
