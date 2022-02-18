using System;

namespace LsAdmin.Application.OrderMaterialApp.Dtos {
    public class OrderMaterialDto {
        public Guid Id { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        public Guid CreateUserId { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
