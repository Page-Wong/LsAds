using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.EquipmentApp.Dtos;
using Microsoft.AspNetCore.Http;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;
using System;
using LsAdmin.Application.EquipmentRepairApp;
using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.PlayerProgramApp.Dtos;
using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Application.FilesApp;

namespace LsAdmin.Application.EquipmentApp
{
    public class EquipmentAppService : BaseAppService<Equipment, EquipmentDto>, IEquipmentAppService {

        private readonly IEquipmentRepository _repository;
        private readonly IEquipmentRepairAppService _repairRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IFilesAppService _filesAppService;

        public EquipmentAppService(IFilesAppService filesAppService, IEquipmentRepository repository, IEquipmentRepairAppService repairRepository,IHttpContextAccessor httpContextAccessor, IPlayerRepository playerRepository) : base(repository, httpContextAccessor) {
            _repository = repository;
            _repairRepository = repairRepository;
            _playerRepository = playerRepository;
            _filesAppService = filesAppService;
        }

        public List<EquipmentDto> GetEquipments(int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<EquipmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.Status == 1, it => it.CreateTime));

        }

        public List<EquipmentDto> GetEquipmentByQuery(Guid placetype, string province, string city, string district, string price, ushort favorite, int startPage, int pageSize, out int rowCount)
        {
            var equipments = Mapper.Map<List<EquipmentDto>>(_repository.GetEntities().Where(it => it.Status == 1));
            var equipmentsbytype = favorite == 0 ? equipments : equipments.Where(it => it.FavoriteType == favorite);
            var resultByPlace = equipmentsbytype.Where(it => (placetype == Guid.Parse("00000000-0000-0000-0000-000000000000") ? 1 == 1 : it.PlaceDto.TypeId == placetype)).ToList();
            var templateResult = new List<EquipmentDto>();
            if (province == null || province == "")
            {
                templateResult = resultByPlace;
            }
            else
            {
                templateResult = resultByPlace.Where(it => (it.Province == province) && (city == null ? 1 == 1 : it.City == city) && (district == null ? 1 == 1 : it.District == district)).ToList();
            }
            var result = new List<EquipmentDto>();
            if (price == null || price == "")
            {
                result = templateResult;
            }
            else
            {
                switch (price)
                {
                    case "1":
                        result = templateResult.Where(it => it.PriceDto.Price < 500).ToList();
                        break;
                    case "2":
                        result = templateResult.Where(it => (it.PriceDto.Price >= 500) && (it.PriceDto.Price < 800)).ToList();
                        break;
                    case "3":
                        result = templateResult.Where(it => (it.PriceDto.Price >= 800) && (it.PriceDto.Price < 1000)).ToList();
                        break;
                    case "4":
                        result = templateResult.Where(it => (it.PriceDto.Price >= 1000) && (it.PriceDto.Price < 1500)).ToList();
                        break;
                    case "5":
                        result = templateResult.Where(it => it.PriceDto.Price >= 1500).ToList();
                        break;
                }
            }
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }


        public List<EquipmentDto> GetEquipmentByPlace(Guid placeId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<EquipmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.PlaceId == placeId, it => it.CreateTime));
        }

        public List<EquipmentDto> GetEquipmentByUserPageList(int startPage, int pageSize, uint status, out int rowCount, out int unInuseCount, out int inuseCount, out int repairingCount, out int scrapCount)
        {
            
            var equipments = GetAllList().Where(w => w.Status == status && w.OwnerUserId == CurrentUser.Id).OrderByDescending(o => o.StartDate);
            var equipmentRepair = _repairRepository.GetAllList().Where(w => (new uint[] { 0, 1, 2 }).Contains(w.Status)).ToList();

            var equipmenttemps = equipments;
            foreach (var equipment in equipmenttemps)
            {
                var equipmentRepairtemp = equipmentRepair.FirstOrDefault(f => f.EquipmentId== equipment.Id);
                if (equipmentRepairtemp != null) { 
                var equipmenttemp = equipments.First(f => f.Id == equipment.Id);
                    equipmenttemp.EquipmentRepairDto = equipmentRepairtemp;
                }

            }
            var result = equipments;
            
            rowCount = result.Count();

            var equipmentlists = GetAllList();
            unInuseCount = equipmentlists.Where(w => w.Status == 0 && w.OwnerUserId == CurrentUser.Id).Count();
            inuseCount = equipmentlists.Where(w => w.Status == 1 && w.OwnerUserId == CurrentUser.Id).Count();
            repairingCount = equipmentlists.Where(w => w.Status == 2 && w.OwnerUserId == CurrentUser.Id).Count();
            scrapCount = equipmentlists.Where(w => w.Status == 3 && w.OwnerUserId == CurrentUser.Id).Count();

            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public EquipmentDto GetByDeviceId(string deviceId) {
            return Mapper.Map<EquipmentDto>(_repository.GetEntities().Where(e =>e.DeviceId.Equals(deviceId)).FirstOrDefault());
            
        }

        public List<EquipmentDto> GetEquipmentByArea(string province, string city = "", string district = "", string street = "", string streetNumber = "")
        {
            var equipments = this.GetAllList();

            if (!string.IsNullOrEmpty(province))
            {
                equipments = equipments.FindAll(f => f.Province == province);

                if (!string.IsNullOrEmpty(city))
                {
                    equipments = equipments.FindAll(f => f.City == city);

                    if (!string.IsNullOrEmpty(district))
                    {
                        equipments = equipments.FindAll(f => f.District == district);

                        if (!string.IsNullOrEmpty(street))
                        {
                            equipments = equipments.FindAll(f => f.Street == street);
                            if (!string.IsNullOrEmpty(streetNumber))
                            {
                                equipments = equipments.FindAll(f => f.StreetNumber == streetNumber);
                            }
                        }
                    }
                }
                return equipments;
            }
            else
            {
                return equipments;
            }
        }

        public List<EquipmentDto> GetAllEquipmentByMapPoint(decimal MaxLat, decimal MaxLng, decimal MinLat, decimal MinLng) {
            return Mapper.Map<List<EquipmentDto>>(_repository.GetEntities().Where(item => 
                                                    item.MapPointX >= MinLng && 
                                                    item.MapPointX <= MaxLng && 
                                                    item.MapPointY >= MinLat && 
                                                    item.MapPointY <= MaxLng
                                                ).ToList());
        }

        public List<EquipmentDto> GetPageEquipmentByMapPoint(int startPage, int pageSize, out int rowCount, decimal MaxLat, decimal MaxLng, decimal MinLat, decimal MinLng) {
            return Mapper.Map<List<EquipmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, item =>
                                                    item.MapPointX >= MinLng &&
                                                    item.MapPointX <= MaxLng &&
                                                    item.MapPointY >= MinLat &&
                                                    item.MapPointY <= MaxLng, _orderby, _orderbyDesc));
        }

        public List<EquipmentDto> GetAllEquipmentByPlace(Guid placeId)
        {
            return Mapper.Map<List<EquipmentDto>>(_repository.GetEntities().Where(item =>item.PlaceId== placeId).ToList());
        }


       public  List<EquipmentDto> GetOwnerPageList(int startPage, int pageSize, out int rowCount)
        {
          
           var result = from p in GetAllList().Where(w => w.OwnerUserId  == CurrentUser.Id)
                        select p;
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<EquipmentDto> GetOwnerEquipmentsByStatus(uint status, int startPage, int pageSize, out int rowCount, out Dictionary<uint, int> statusRowCount)
        {
            statusRowCount = new Dictionary<uint, int>();
            foreach (var statusCount in _repository.GetEntities().Where(it => it.OwnerUserId == CurrentUser.Id).GroupBy(g => g.Status).Select(s => new { s.Key, Counts = s.Count() }))
            {
                statusRowCount.Add(statusCount.Key, statusCount.Counts);
            }
            return Mapper.Map<List<EquipmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.Status == status && it.OwnerUserId == CurrentUser.Id, it => it.CreateTime));
        }

        public List<EquipmentDto> GetGetOwnerEquipmentsWithStatusRowCount(int startPage, int pageSize, out int rowCount, out Dictionary<uint, int> statusRowCount)
        {
            statusRowCount = new Dictionary<uint, int>();
            foreach (var statusCount in _repository.GetEntities().Where(it => it.OwnerUserId == CurrentUser.Id).GroupBy(g => g.Status).Select(s => new { s.Key, Counts = s.Count() }))
            {
                statusRowCount.Add(statusCount.Key, statusCount.Counts);
            }
            return Mapper.Map<List<EquipmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it =>  it.OwnerUserId == CurrentUser.Id, it => it.CreateTime));
        }

        public bool Registered(ref EquipmentDto dto, out string errormessage)
        {
            errormessage = "";
            try
            {
                var dtotemp = dto;

                if (dto.Id == null && GetByDeviceId(dto.DeviceId) != null)
                {
                    errormessage = "设备已注册";
                    return false;
                }

                if (dto.Id == null && _repository.GetEntities().Where(w => w.OwnerUserId == dtotemp.OwnerUserId && w.Name == dtotemp.Name).FirstOrDefault() != null)
                {
                    errormessage = "存在同设备名称记录";
                    return false;
                }

                if (InsertOrUpdate(ref dto)==false)
                { errormessage = "注册失败"; return false; }

                return true;
            }catch(Exception ex)
            {
                errormessage = "注册失败";
                return false;
            }
        }

        public bool Registered(string deviceId, Guid ownerUserId, string equipmentName,Guid? equipmentModeId, out string errormessage)
        {
            errormessage = "";
            try
            {
                EquipmentDto dto = new EquipmentDto
                {
                    
                    EquipmentModelId= equipmentModeId,
                    OwnerUserId = ownerUserId,
                    DeviceId = deviceId,
                    Name = equipmentName,
                    Status=0,
                };

                return Registered(ref dto, out errormessage);
            }
            catch (Exception ex)
            {
                errormessage = "注册失败";
                return false;
            }

          
        }
        /*设备列表（不包括黑名单）*/
        public List<EquipmentDto> GetEquipmentsExceptBlack(int startPage, int pageSize, out int rowCount)
        {
            var equipments = Mapper.Map<List<EquipmentDto>>(_repository.GetEntities().Where(it => it.Status == 1));
            var result = equipments.Where(w => w.FavoriteType != 2);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        /*获取用户收藏的媒体位列表*/
        public List<EquipmentDto> GetCollectionsPageList(int startPage, int pageSize, out int rowCount)
        {
            var equipments = Mapper.Map<List<EquipmentDto>>(_repository.GetEntities().Where(it => it.Status == 1));
            var result = equipments.Where(w => w.FavoriteType == 1);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }


        /*获取用户黑名单的媒体位列表*/
        public List<EquipmentDto> GetBlacklistsPageList(int startPage, int pageSize, out int rowCount)
        {
            var equipments = Mapper.Map<List<EquipmentDto>>(_repository.GetEntities().Where(it => it.Status == 1));
            var result = equipments.Where(w => w.FavoriteType == 2);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<ProgramDto> GetDowmloadPlayInfoByEquipmentId(Guid equipmentId)
        {
            return    Mapper.Map<List<ProgramDto>>(_repository.GetDowmloadPlayInfoByEquipmentId(equipmentId));
        }

        public List<PlayerProgramDto> GetDowmloadPlayerProgramByEquipmentId(Guid equipmentId)
        {
            return Mapper.Map<List<PlayerProgramDto>>(_repository.GetDowmloadPlayerProgramByEquipmentId(equipmentId));
        }

        public List<EquipmentDto> GetAllEquipmentByPlaces(List<Guid> placeIds) {
            return Mapper.Map<List<EquipmentDto>>(_repository.GetAllList(it => it.PlaceId != null && placeIds.Contains(it.PlaceId.Value)));
        }

        public bool AddLogFile(Guid id, FilesInfoDto fileInfo) {
            return _filesAppService.InsertInfo(ref fileInfo) && _repository.AddLogFile(id, fileInfo.Id);
        }

        public List<FilesInfoDto> GetLogFiles(Guid id) {
            var files = _repository.GetLogFiles(id);
            return files.Select(it => _filesAppService.GetInfo(it.Id)).ToList();

        }
    }
}
