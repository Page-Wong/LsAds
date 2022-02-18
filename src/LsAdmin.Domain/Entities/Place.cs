using System;

namespace LsAdmin.Domain.Entities {
    public class Place : BaseEntity
    {
        //场所类型Id
        public Guid TypeId { get; set; }

        //场所名称
        public string Name { get; set; }

        //场所介绍
        public string Introduction { get; set; }


        //联系电话
        public string Phone { get; set; }

        //负责人
        public string Contact { get; set; }

        //场所标签
        public string PlaceTag { get; set; }

        //场地广告白名单
        public string AdsWhiteTag { get; set; }

        //场地广告黑名单
        public string AdsBlackTag { get; set; }

        //可播放广告时间段
        public string TimeRange { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public Guid? OwnerUserId { get; set; }

        public decimal? MapPointX { get; set; }
        public decimal? MapPointY { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }        

    }
}
