using System;

namespace LsAdmin.Domain.Entities {
    public class PlayHistory : BaseEntity {
        /// <summary>
        /// 订单
        /// </summary>
        public Guid OrderTimeId { get; set; }

        /// <summary>
        /// 素材
        /// </summary>
        public Guid MaterialId { get; set; }

        /// <summary>
        /// 设备标识号
        /// </summary>
        public string DeviceId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long FrontalfaceCount { get; set; }
        public long ProfilefaceCount { get; set; }
        public int ClickCount { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        

    }
}
