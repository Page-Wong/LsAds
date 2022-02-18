using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.WebAPI.Models
{
    public class OrderPoolModel
    {
        public DateTime? CreatetTime { get; set; }
        public string OrderPlaceId { get; set; }
        /// <summary>
        /// 广告订单id
        /// </summary>
        public string OrderTimeId { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public string PlaceId { get; set; }


        public long AvailableTimes { get; set; }
        /// <summary>
        /// 曝光次数
        /// </summary>
        public uint ExposureCount { get; set; }

        /// <summary>
        /// 已曝光次数
        /// </summary>
        public int AlreadyExposureCount { get; set; }

    }
}
