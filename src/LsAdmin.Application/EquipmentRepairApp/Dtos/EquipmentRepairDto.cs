using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Utility.Service;
using System;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Application.EquipmentRepairApp.Dtos
{
    public class EquipmentRepairDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "设备编码")]
        public Guid EquipmentId { get; set; }

        /// <summary>
        /// 场所编码
        /// </summary>
        [Display(Name = "场所编码")]
        public Guid PlaceId { get; set; }

        /// <summary>
        /// 报障日期
        /// </summary>
        [Display(Name = "报障日期")]
        public DateTime WarningDate { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        [Display(Name = "问题描述")]
        public string ProblemDescription { get; set; }

        /// <summary>
        /// 场所联系人
        /// </summary>
        [MaxLength(30)]
        [Display(Name = "场所联系人")]
        public string PlaceContact { get; set; }

        /// <summary>
        /// 场所联系电话
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "场所联系电话")]
        public string PlaceContactPhone { get; set; }

        /// <summary>
        /// 记录状态
        /// </summary>
        [Display(Name = "记录状态")]
        public uint Status { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        [Display(Name = "处理人")]
        [MaxLength(30)]
        public string ProcessingPerson { get; set; }

        /// <summary>
        /// 处理人联系电话
        /// </summary>
        [Display(Name = "处理人联系电话")]
        [MaxLength(50)]
        public string ProcessingPersonPhone { get; set; }

        /// <summary>
        /// 处理方法
        /// </summary>
        [Display(Name = "处理方法")]
        public uint ProcessingMethod { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        [Display(Name = "处理结果")]
        [MaxLength(500)]
        public string ProcessingResults { get; set; }

        /// <summary>
        /// 设备故障素材 图片\视频
        /// </summary>
        public Guid BeforeMaterial { get; set; }

        /// <summary>
        /// 设备维修后素材 图片\视频
        /// </summary>
        public Guid AfterMaterial { get; set; }

        /// 备注
        public string Remarks { get; set; }


        /// 创建人
        public Guid CreateUserId { get; set; }

        /// 创建时间
        public DateTime? CreateTime { get; set; }


        public  Domain.Entities.Equipment Equipment;

        public Domain.Entities.Place Place;

        //public Domain.Entities.Equipment Equipment { get; }

        //public  Place Place { get; }

        public virtual PlaceDto PlaceDto{   
            get {
                if (Place != null) { return AutoMapper.Mapper.Map<PlaceDto>(Place); }
                return null;
                /*
                if (PlaceId == null) return null;
                var service = (IPlaceAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlaceAppService));
                var dto = service.Get(PlaceId);
                return dto;*/
            }
        }

        public virtual EquipmentDto EquipmentDto
        {
            get{
                if (Equipment != null){return AutoMapper.Mapper.Map<EquipmentDto>(Equipment);}
                    return null;
                
                /*

                if (EquipmentId == null) return null;
                var service = (IEquipmentAppService)HttpHelper.ServiceProvider.GetService(typeof(IEquipmentAppService));
                var dto = service.Get(EquipmentId);
                return dto;*/
            }
        }

        /// <summary>
        /// 场所拥有者
        /// </summary>
        public Guid? PlaceOwnerUserId{
            get{
                if (PlaceDto != null) { return PlaceDto?.OwnerUserId == null ? null : PlaceDto?.OwnerUserId;  }
                return null;
                /*
                var service = (IPlaceAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlaceAppService));
                var dto = service.Get(PlaceId);

                return (Guid)dto?.OwnerUserId;*/
            }
        }

        public string TimeAgo{
            get{
                if (WarningDate == null) return "";
                return Utility.Convert.TimeConvertHelper.TimeDiffString(DateTime.Now, WarningDate);

                /*
                double TotalMinutes = ((TimeSpan)(DateTime.Now - WarningDate)).TotalMinutes;
                if (TotalMinutes < 60){
                    return TotalMinutes.ToString("f1") + "分钟";
                }
                else if (TotalMinutes< 24*60){
                    return (TotalMinutes/60).ToString("f1") + "小时";
                }
                else
                {
                    return (TotalMinutes/60/24).ToString("f1") + "天";
                }*/
            }
        }

        public const ushort STATUS_UNCONFIRMED = 0;
        public const ushort STATUS_CONFIRMED = 1;
        public const ushort STATUS_COMPLETE = 2;

        public virtual string StatusString{
            get{
                if (Status == STATUS_UNCONFIRMED) return "待设备主确认";
                if (Status == STATUS_CONFIRMED)   return "已确认";
                if (Status == STATUS_COMPLETE)    return "已完成";
                return "未知";
            }
        }

        public const ushort PROCESSINGMETHOD_NULL = 0;
        public const ushort PROCESSINGMETHOD_REPAIR = 1;
        public const ushort PROCESSINGMETHOD_REPLACE = 2;
        public const ushort PROCESSINGMETHOD_OTHER = 3;

        public virtual string ProcessingMethodString{
            get{
                if (ProcessingMethod == PROCESSINGMETHOD_NULL) return "";
                if (ProcessingMethod == PROCESSINGMETHOD_REPAIR) return "维修";
                if (ProcessingMethod == PROCESSINGMETHOD_REPLACE) return "更换";
                if (ProcessingMethod == PROCESSINGMETHOD_OTHER) return "其他";
                return ProcessingMethod.ToString();
            }
        }

    }
}
