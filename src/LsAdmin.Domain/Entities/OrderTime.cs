using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Domain.Entities
{
    public class OrderTime : BaseEntity
    {
        /// <summary>
        /// 订单播ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 时间段类型
        /// </summary>
        public string TimeRangeType { get; set; }

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
        [Required(ErrorMessage = "播放次数必须大于0")]
        public uint ExposureCount { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public float UnitPrice { get; set; }
        
        /// <summary>
        /// 订单金额
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }

        public Order Order  { get; set; }

    }
}
