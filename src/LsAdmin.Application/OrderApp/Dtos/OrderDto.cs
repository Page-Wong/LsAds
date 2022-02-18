using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using LsAdmin.Application.OrderPlaceApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LsAdmin.Application.OrderApp.Dtos {
    public class OrderDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 广告标签
        /// </summary>
        public string AdsTag { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry { get; set; }

        ///<summary>
        ///宣传网址
        ///</summary>
        public string Url { get; set; }

        /// <summary>
        /// 广告类型
        /// </summary>
        public UInt16 Type { get; set; }

        /// <summary>
        /// 素材类型
        /// </summary>
        public UInt16 MateralType { get; set; }

        ///<summary>
        ///广告时间长度(单位：s）
        ///</summary>
        [Required(ErrorMessage = "广告时长必须大于0")]
        public uint TotalSeconds { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>        
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public ushort Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        public Guid CreateUserId { get; set; }

        public DateTime? CreateTime { get; set; }

        public List<OrderTime> OrderTimes { get; set; }
        public List<OrderMaterialDto> OrderMaterials { get; set; }
        public virtual List<OrderTradeDto> OrderTrades { get; set; }
        public virtual List<OrderPlayerDto> OrderPlayers { get; set; }
        public virtual List<OrderPlayerProgramDto> OrderPlayerPrograms { get; set; }


        public const ushort ORDER_STATUS_UNPUBLISHED = 0;
        public const ushort ORDER_STATUS_PUBLISHED = 1;
        public const ushort ORDER_STATUS_AUDITING = 2;
        public const ushort ORDER_STATUS_PREPARING = 3;
        public const ushort ORDER_STATUS_RUNNING = 4;
        public const ushort ORDER_STATUS_COMPLETE = 5;
        public const ushort ORDER_STATUS_CANCEL = 6;
        public const ushort ORDER_STATUS_REFUNDBEFOREAUDIT = 7;
        public const ushort ORDER_STATUS_REFUNDAFTERAUDIT = 8;
        public const ushort ORDER_STATUS_PAUSED = 9;

        public virtual string StatusString {
            get {
                if (Status == ORDER_STATUS_UNPUBLISHED) return "未发布";
                if (Status == ORDER_STATUS_PUBLISHED) return "待付款";
                if (Status == ORDER_STATUS_AUDITING) return "审核中";
                if (Status == ORDER_STATUS_PREPARING) return "待播放";
                if (Status == ORDER_STATUS_RUNNING) return "播放中";
                if (Status == ORDER_STATUS_COMPLETE) return "已完成";
                if (Status == ORDER_STATUS_CANCEL) return "已取消";
                if (Status == ORDER_STATUS_REFUNDBEFOREAUDIT) return "待退款";
                if (Status == ORDER_STATUS_REFUNDAFTERAUDIT) return "待退款";
                if (Status == ORDER_STATUS_PAUSED) return "暂停";

                return "未知";
            }
        }

        public const ushort ORDER_TYPE_1 = 1;
        public const ushort ORDER_TYPE_11 = 11;
        public const ushort ORDER_TYPE_12 = 12;
        public const ushort ORDER_TYPE_2 = 2;
        public const ushort ORDER_TYPE_3 = 3;

        public virtual string TypeString
        {
            get
            {

                if (Type == ORDER_TYPE_1) return "广告主";
                if (Type == ORDER_TYPE_11) return "广告主_室内广告";
                if (Type == ORDER_TYPE_12) return "广告主_户外广告";
                if (Type == ORDER_TYPE_2) return "场所主";
                if (Type == ORDER_TYPE_3) return "共享";

                return "未知";
            }
        }


        /// <summary>
        /// 可播放的订单状态
        /// </summary>
        public ushort[] CanPlayStatus => new ushort[] { 3, 4 };

        /// <summary>
        /// 订单可以播放为true 否则为false
        /// </summary>
        public virtual bool IsCanPlay
        {
            get {
                return CanPlayStatus.Contains(Status) && StartDate <= DateTime.Now.Date && EndDate >= DateTime.Now.Date;
            }
        }

        /// <summary>
        /// 是使用order结构的订单类型否则使用ordertime结构
        /// </summary>
        public virtual bool IsUseOrderType
        {
            get {
                return (new UInt16[] { 12 }).Contains(Type);
            }
        }

    }
}
