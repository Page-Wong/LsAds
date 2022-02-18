using LsAdmin.Application.EquipmentRepairApp.Dtos;
using LsAdmin.Application.Imp;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.EquipmentRepairApp
{
    public interface IEquipmentRepairAppService : IBaseAppService<EquipmentRepairDto>{
        List<EquipmentRepairDto> GetOwnerEquipmentRepairPageList(int startPage, int pageSize, uint status ,out int rowCount,out int unConfirmedCount, out int confirmedCount, out int completeCount);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        //new bool Update(EquipmentRepairDto dto);

        EquipmentRepairDto GetByEquipmentId(Guid equipmentId);
        List<EquipmentRepairDto> GetListByEquipmentId(Guid equipmentId);
    }

   
}



