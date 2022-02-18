using System;

namespace LsAdmin.Domain.Entities {
    public class OrderEquipment : BaseEntity {
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


        public Order Order { get; set; }
      
        public Equipment Equipment { get; set; }
    }
}
