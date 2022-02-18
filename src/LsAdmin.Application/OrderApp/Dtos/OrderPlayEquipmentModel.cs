using LsAdmin.Application.OrderApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.OrderApp.Dtos
{
    public class OrderPlayEquipmentModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        public UInt16 OrderType { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderTimeId { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 时间段
        /// </summary>
        public string TimeRange { get; set; }

        /// <summary>
        /// 曝光率
        /// </summary>
        public string ExposureRate { get; set; }

        /// <summary>
        /// 曝光次数
        /// </summary>
        public uint ExposureCount { get; set; }

        //剩余可播放时间
        public double Availabletimes { get; set; }

        public double Totaltimes { get; set; }

        public int AvailableDays { get; set; }


        /// <summary>
        /// 素材类型
        /// </summary>
        public UInt16 MateralType { get; set; }

        ///<summary>
        ///广告时间长度(单位：s）
        ///</summary>
        public uint TotalSeconds { get; set; }

        /// <summary>
        /// 广告标签
        /// </summary>
        public string AdsTag { get; set; }
    }
}
