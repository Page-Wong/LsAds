using LsAdmin.Application.AdministrativeAreaApp;
using LsAdmin.Application.PlaceTypeApp;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Utility.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Application.PlaceApp.Dtos {
    public class PlaceDto {
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        //场所类型Id
        public Guid TypeId { get; set; }

        //场所介绍
        public string Introduction { get; set; }

        //联系电话
        public string Phone { get; set; }

        //负责人
        public string Contact { get; set; }

        //场所标签
        public string PlaceTag { get; set; }

        //场地广告白名单
        public string AdsWhiteTag { get; set; }

        //场地广告黑名单
        public string AdsBlackTag { get; set; }

        //可播放广告时间段
        public string TimeRange { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public Guid? OwnerUserId { get; set; }


        public decimal? MapPointX { get; set; }
        public decimal? MapPointY { get; set; }
        public string Province { get; set; }
        public string ProvinceName {
            get {
                if (string.IsNullOrEmpty(Province)) {
                    return "";
                }
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                return service.GetByCode(uint.Parse(Province))?.Name;
            }
        }
        public string City { get; set; }
        public string CityName {
            get {
                if (string.IsNullOrEmpty(City)) {
                    return "";
                }
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                return service.GetByCode(uint.Parse(City))?.Name;
            }
        }
        public string District { get; set; }
        public string DistrictName {
            get {
                if (string.IsNullOrEmpty(District)) {
                    return "";
                }
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                return service.GetByCode(uint.Parse(District))?.Name;
            }
        }
        public string Street { get; set; }
        public string StreetNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        public Guid? CreateUserId { get; set; }

        public DateTime? CreateTime { get; set; }


        
       public string TypeName
        {
            get
            {
                if (TypeId == null)
                {
                    return "";
                }
                var service = (IPlaceTypeAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlaceTypeAppService));
                return service.Get(TypeId)?.Type;
            }
        }

       public string Address
        {
            get
            {
                return ProvinceName + CityName + DistrictName + Street + StreetNumber;
            }
        }



    }
}
