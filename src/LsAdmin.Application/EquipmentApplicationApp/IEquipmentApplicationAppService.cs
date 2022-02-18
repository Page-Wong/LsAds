using LsAdmin.Application.EquipmentApplicationApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.EquipmentApplicationApp
{
    public interface IEquipmentApplicationAppService : IBaseAppService<EquipmentApplicationDto>
    {
        List<EquipmentApplicationDto> GetEquipmentApplicationByPlace(Guid placeId, int startPage, int pageSize, out int rowCount);

        List<EquipmentApplicationDto> GetEquipmentApplicationByStatus(uint status, int startPage, int pageSize,out int rowCount,out Dictionary<ushort, int> statusRowCount);

        List<EquipmentApplicationDto> GetEquipmentApplicationWithStatusRowCount(int startPage, int pageSize, out int rowCount, out Dictionary<ushort, int> statusRowCount);
    }
}
