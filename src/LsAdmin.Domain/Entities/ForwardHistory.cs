using System;

namespace LsAdmin.Domain.Entities {
    /// <summary>
    /// 转发历史
    /// </summary>
    public class ForwardHistory : BaseEntity {

        /// <summary>
        /// 设备标识号
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderTimeId { get; set; }
        public DateTime ForwardTime { get; set; }
        public string ForwardUrl { get; set; }

    }
}
