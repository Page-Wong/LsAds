using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Utility.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.EquipmentApplicationApp.Dtos
{
    public class EquipmentApplicationDto
    {
        public Guid Id { get; set; }

        //场所ID
        public Guid PlaceId { get; set; }

        //申请信息

        public string Reason { get; set; }

        //审核状态
        public ushort Status { get; set; }

        //报装进度
        public string Progress { get; set; }

        /// 备注
        public string Remarks { get; set; }


        /// 创建人
        public Guid CreateUserId { get; set; }

        /// 创建时间
        public DateTime? CreateTime { get; set; }

        public const ushort APPLICATION_STATUS_WAITING = 0;
        public const ushort APPLICATION_STATUS_CHECKING = 1;
        public const ushort APPLICATION_STATUS_PASS = 2;
        public const ushort APPLICATION_STATUS_FAILED = 3;

        public virtual string StatusString
        {
            get
            {
                if (Status == APPLICATION_STATUS_WAITING) return "待确认";
                if (Status == APPLICATION_STATUS_CHECKING) return "审核中";
                if (Status == APPLICATION_STATUS_PASS) return "审核通过";
                if (Status == APPLICATION_STATUS_FAILED) return "审核不通过";
                
                return "未知";
            }
        }

        public virtual PlaceDto PlaceDto{
            get{
                if (PlaceId==null){
                    return null;
                }
                var service = (IPlaceAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlaceAppService));
                return service.Get(PlaceId);
            }
        }

        public string TimeAgo{
            get
            {
                if (CreateTime == null) return "";
                return Utility.Convert.TimeConvertHelper.TimeDiffString(DateTime.Now, (DateTime)CreateTime);
                /*
                double TotalMinutes = ((TimeSpan)(DateTime.Now - CreateTime)).TotalMinutes;
                if (TotalMinutes < 60){
                    return TotalMinutes.ToString("f1") + "分钟";
                }
                else if (TotalMinutes < 24 * 60){
                    return (TotalMinutes / 60).ToString("f1") + "小时";
                }
                else{
                    return (TotalMinutes / 60 / 24).ToString("f1") + "天";
                }*/
            }
        }

    }
}
