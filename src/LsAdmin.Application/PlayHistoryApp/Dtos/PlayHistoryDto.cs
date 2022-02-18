using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using System;

namespace LsAdmin.Application.PlayHistoryApp.Dtos {
    public class PlayHistoryDto {
        public Guid Id { get; set; }

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
