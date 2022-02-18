using System;
using System.Collections.Generic;

namespace LsAdmin.Application.OrderEquipmentApp.Dtos {
    public class OrderEquipmentDto {
        public Guid Id { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid EquipmentId { get; set; }


        /// <summary>
        /// 播放状态 0=待批准播放，1=可播放，2=完成播放，3，播放搁置  
        /// </summary>
        public UInt16 PlayingStatus { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        public Guid CreateUserId { get; set; }

        public DateTime? CreateTime { get; set; }


        public const ushort PLAYING_STATUS_PENDING = 0;
        public const ushort PLAYING_STATUS_CANPlAY = 1;
        public const ushort PLAYING_STATUS_COMPLETE = 2;
        public const ushort PLAYING_STATUS_PAUSED = 3;

        public static Dictionary<ushort, string> OrderPlayingStatus = new Dictionary<ushort, string>
        {
            { PLAYING_STATUS_PENDING, "待批准播放" },
            { PLAYING_STATUS_CANPlAY, "可播放" },
            { PLAYING_STATUS_COMPLETE, "完成播放" },
            { PLAYING_STATUS_PAUSED, "播放搁置" },
        };
    }
}
