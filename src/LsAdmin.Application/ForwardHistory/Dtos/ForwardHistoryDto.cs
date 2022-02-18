using System;

namespace LsAdmin.Application.ForwardHistoryApp.Dtos {
    public class ForwardHistoryDto {
        public Guid Id { get; set; }

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
    }
}
