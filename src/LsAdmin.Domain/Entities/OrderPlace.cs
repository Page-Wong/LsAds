using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class OrderPlace: BaseEntity {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public Guid PlaceId { get; set; }

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
        public int ExposureCount { get; set; }

        public Order Order { get; set; }

        public Place Place { get; set;}
    }
}
