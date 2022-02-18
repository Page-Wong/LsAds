using LsAdmin.Application.EquipmentModelApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.EquipmentModelApp
{
    public interface IEquipmentModelAppService : IBaseAppService<EquipmentModelDto>
    {

        /// <summary>
        /// 根据Id获取信息实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        EquipmentModelInfoDto GetInfo(Guid id);
        

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertInfo(EquipmentModelInfoDto infodto);


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool UpdateInfo(EquipmentModelInfoDto infodto);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        void DeleteInfo(Guid id);


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        void DeleteBatcheInfo(List<Guid> ids);

        /// <summary>
        /// 检测是否存在同型号记录，true 存在 false 不存在
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manufacturer"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ExistSameModel(string model,string manufacturer ,Guid id);

     
    }
}
