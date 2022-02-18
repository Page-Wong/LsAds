using System;

namespace LsAdmin.Domain.Entities {
    public class OrderMaterial : BaseEntity {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// 素材ID
        /// </summary>
        public Guid MaterialId { get; set; }

        ///<sumary>
        ///素材顺序
        ///</sumary>
        public int Orderby { get; set; }

        /// <summary>
        /// 播放时间（单位：s)
        /// </summary>
        public int Seconds { get; set; }
        public Order Order { get; set; }

        public Material Material { get; set; }
    }
}
