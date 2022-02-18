using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Application.Imp;
using LsAdmin.Application.PlayerProgramApp.Dtos;
using LsAdmin.Application.ProgramApp.Dtos;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.EquipmentApp
{
    public interface IEquipmentAppService : IBaseAppService<EquipmentDto> {
        EquipmentDto GetByDeviceId(string deviceId);

        List<EquipmentDto> GetEquipments(int startPage, int pageSize, out int rowCount);
        List<EquipmentDto> GetEquipmentByQuery(Guid placetype, string province, string city, string district, string price, ushort favoriteType, int startPage, int pageSize, out int rowCount);

        List<EquipmentDto> GetEquipmentByPlace(Guid placeId, int startPage, int pageSize, out int rowCount);

        List<EquipmentDto> GetAllEquipmentByPlace(Guid placeId);
        List<EquipmentDto> GetAllEquipmentByPlaces(List<Guid> placeIds);

        List<EquipmentDto>  GetEquipmentByUserPageList(int startPage, int pageSize, uint status, out int rowCount, out int unInuseCount, out int inuseCount, out int repairingCount, out int scrapCount);
        List<EquipmentDto> GetEquipmentByArea(string province, string city = "", string district = "", string street = "", string streetNumber = "");

        List<EquipmentDto> GetAllEquipmentByMapPoint(decimal MaxLat, decimal MaxLng, decimal MinLat, decimal MinLng);
        List<EquipmentDto> GetPageEquipmentByMapPoint(int startPage, int pageSize, out int rowCount, decimal MaxLat, decimal MaxLng, decimal MinLat, decimal MinLng);

        List<EquipmentDto> GetOwnerPageList(int startPage, int pageSize, out int rowCount);

        List<EquipmentDto> GetOwnerEquipmentsByStatus(uint status, int startPage, int pageSize, out int rowCount, out Dictionary<uint, int> statusRowCount);

        List<EquipmentDto> GetGetOwnerEquipmentsWithStatusRowCount(int startPage, int pageSize, out int rowCount, out Dictionary<uint, int> statusRowCount);

        bool Registered(ref EquipmentDto dto, out string errormessage);

        bool Registered(string deviceId, Guid ownerUserId, string equipmentName,Guid? equipmentModeId, out string errormessage);
        List<EquipmentDto> GetEquipmentsExceptBlack(int startPage, int pageSize, out int rowCount);

        List<EquipmentDto> GetCollectionsPageList(int startPage, int pageSize, out int rowCount);

        List<EquipmentDto> GetBlacklistsPageList(int startPage, int pageSize, out int rowCount);


        /// <summary>
        /// 获取设备的需下载的节目清单
        /// </summary>
        /// <param name="equipmentId">设备id</param>
        /// <returns>素材列表</returns>
        List<ProgramDto> GetDowmloadPlayInfoByEquipmentId(Guid equipmentId);


        /// <summary>
        /// 获取节目状态为播放中的设备节目清单
        /// </summary>
        /// <param name="equipmentId">设备id</param>
        /// <returns></returns>
        List<PlayerProgramDto> GetDowmloadPlayerProgramByEquipmentId(Guid equipmentId);


        bool AddLogFile(Guid id, FilesInfoDto fileInfo);
        List<FilesInfoDto> GetLogFiles(Guid id);
    }
}
