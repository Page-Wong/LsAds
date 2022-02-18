using LsAdmin.Application.AdministrativeAreaApp;
using LsAdmin.Application.CollectionsBlacklistsApp.Dtos;
using LsAdmin.Application.EquipmentModelApp;
using LsAdmin.Application.EquipmentModelApp.Dtos;
using LsAdmin.Application.EquipmentRepairApp;
using LsAdmin.Application.EquipmentRepairApp.Dtos;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.PlayPriceApp;
using LsAdmin.Application.PlayPriceApp.Dtos;
using LsAdmin.Utility.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LsAdmin.Application.EquipmentApp.Dtos
{
    public class EquipmentDto {

        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        //设备开始使用时间
        public DateTime? StartDate { get; set; }


        //设备停用时间
        public DateTime? DiscontinuationDate { get; set; }


        //设备状态
        public uint Status { get; set; }

        /// <summary>
        /// 设备标识号
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public Guid OwnerUserId { get; set; }


        /// <summary>
        /// 设备型号编码
        /// </summary>
        public Guid? EquipmentModelId { get; set; }


        /// <summary>
        /// 使用场所
        /// </summary>
        public Guid? PlaceId { get; set; }

        /// <summary>
        /// 设备维修编码
        /// </summary>
        public Guid? EquipmentRepairId { get; set; }

        public decimal MapPointX { get; set; }
        public decimal MapPointY { get; set; }
        public string Province { get; set; }
        public string ProvinceName {
            get {
                if (string.IsNullOrEmpty(Province)) {
                    return "";
                }
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                var dto = service.GetByCode(uint.Parse(Province));
                return dto == null ? "" : dto.Name;
            }
        }
        public string City { get; set; }
        public string CityName {
            get {
                if (string.IsNullOrEmpty(City)) {
                    return "";
                }
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                var dto = service.GetByCode(uint.Parse(City));
                return dto == null ? "" : dto.Name;
            }
        }
        public string District { get; set; }
        public string DistrictName {
            get {
                if (string.IsNullOrEmpty(District)) {
                    return "";
                }
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                var dto = service.GetByCode(uint.Parse(District));
                return dto == null ? "" : dto.Name;
            }
        }
        public string Street { get; set; }
        public string StreetNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

       // public  EquipmentModel EquipmentModel { get; }

       // public Place Place { get; }
        
        public virtual EquipmentModelDto EquipmentModelDto
        {
            
            get {
                if (EquipmentModelId == null){return null;}
                try { 
                    var EquipmentModeservice = (IEquipmentModelAppService)HttpHelper.ServiceProvider.GetService(typeof(IEquipmentModelAppService));
                    return EquipmentModeservice.Get((Guid)EquipmentModelId);
                }
                catch (Exception ex){
                    var a = ex;
                    return null;
                }
            }
        }

        public virtual PlayPriceDto PriceDto {
            get
            {
                try
                {
                    var PlayPriceservice = (IPlayPriceAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlayPriceAppService));
                    return PlayPriceservice.GetEquipmentPlayPrice(Id, "Day");
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        public virtual PlaceDto PlaceDto
        {
            get
            {
                if (PlaceId == null) { return null; }
                try
                {
                    var Placeservice = (IPlaceAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlaceAppService));
                    return Placeservice.Get((Guid)PlaceId);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public virtual EquipmentRepairDto EquipmentRepairDto
        {
            get;
            set;
            /*  get
             {
                 return null;
                 if (Id == null) { return null; }
                 try
                 {
                     var EquipmentRepairservice = (IEquipmentRepairAppService)HttpHelper.ServiceProvider.GetService(typeof(IEquipmentRepairAppService));
                     return EquipmentRepairservice.GetAllList().FirstOrDefault(f => f.EquipmentId == Id && (new uint[]{ 0,1}).Contains(f.Status) );                }
                 catch (Exception ex)
                 {
                     return null;
                 }
             }*/
        }

        public const ushort STATUS_UNINUSE = 0;
        public const ushort STATUS_INUSE = 1;
        public const ushort STATUS_REPAIRING = 2;
        public const ushort STATUS_SCRAP = 3;

        /// <summary>
        /// 0： 待投放、 1：已投放:2:维修中、3:报废
        /// </summary>
        public virtual string StatusString
        {
            get{
                switch (Status)
                {
                    case STATUS_UNINUSE :  return "待投放";
                    case STATUS_INUSE : return "已投放";
                    case STATUS_REPAIRING: return "维修中";
                    case STATUS_SCRAP: return "报废";
                    default: return "未知状态";
                }
            }
        }

        /// <summary>
        /// 收藏/黑名单
        /// </summary>
        public virtual UInt16 FavoriteType    
        {
            get
            {
                var service = (ICollectionsBlacklistsAppService)HttpHelper.ServiceProvider.GetService(typeof(ICollectionsBlacklistsAppService));
                var result = service.FavoriteType(Id);
                return result;
            }
        }


    }


}
