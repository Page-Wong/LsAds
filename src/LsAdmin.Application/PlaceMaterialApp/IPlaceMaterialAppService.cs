using LsAdmin.Application.Imp;
using LsAdmin.Application.PlaceMaterialApp.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.PlaceMaterialApp
{
    public interface IPlaceMaterialAppService : IBaseAppService<PlaceMaterialDto>
    {
        List<PlaceMaterialDto> GetPlaceMaterials(Guid placeid, ushort materialType = 0);
        List<PlaceMaterialDto> GetPageListByType(int startPage, int pageSize, out int rowCount, Guid placeid, ushort materialType);

        bool SaveToOrder(Guid placeId,List<PlaceMaterialDto> dtos);

    }

}
