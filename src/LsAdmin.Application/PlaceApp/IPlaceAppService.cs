using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.Imp;
using System.Collections.Generic;
using System;
using LsAdmin.Application.EquipmentApp.Dtos;

namespace LsAdmin.Application.PlaceApp {
    public interface IPlaceAppService : IBaseAppService<PlaceDto>
    {
        List<PlaceDto> GetPlaceByType(Guid typeId, int startPage, int pageSize, out int rowCount);

        List<PlaceDto> GetUserPagePlaceByType(Guid typeId, int startPage, int pageSize,Guid  UserId,  out int rowCount);

        List<PlaceDto> GetUserPagePlaces(int startPage, int pageSize, Guid UserId, out int rowCount);

        List<PlaceDto> GetParentPlace(PlaceDto place);

        List<string> GetProvince();
        List<string> GetCity(string province);

        List<string> GetDistrict(string province, string city);

        List<string> GetStreet(string province, string city, string district);

        double GetAreaStockPlayMinutes(DateTime startDate, DateTime endDate, string province, string city, string district = "", string street = "", string adsTag = "", string timeRange="");

        bool DeletePlace(Guid id,out string errormessage);

        bool DeleteBatchPlaces(List<Guid> ids, out string errormessage);

        List<PlaceDto> GetUserAllPlaces(Guid UserId);

        #region 场所设备相关
        List<EquipmentDto> GetUserPageEquipments(int startPage, int pageSize, Guid UserId, out int rowCount, List<uint> status = null);
        List<EquipmentDto> GetUserAllEquipments(Guid UserId);
        #endregion
    }
}
