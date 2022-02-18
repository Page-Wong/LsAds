using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Domain.Entities {
    public class Order : BaseEntity {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 广告标签
        /// </summary>
        public string AdsTag { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry{ get; set; }

        ///<summary>
        ///宣传网址
        ///</summary>
        public string Url { get; set; }

        /// <summary>
        /// 广告类型
        /// </summary>
        public UInt16 Type { get; set; }

        /// <summary>
        /// 素材类型
        /// </summary>
        public UInt16 MateralType { get; set; }

        ///<summary>
        ///广告时间长度(单位：s）
        ///</summary>
        [Required(ErrorMessage = "广告时长必须大于0")]
        public uint TotalSeconds { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public float Amount{ get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>    
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单状态(0:未发布 1：待付款  2：待审核 3：待播放 4：播放中 5：已完成  6：取消 7：审核前待退款 8：审核后待退款 9：暂停）
        /// </summary>
        public ushort Status { get; set; }


        public virtual ICollection<OrderTime> OrderTimes { get; set; }
        public virtual ICollection<OrderMaterial> OrderMaterials { get; set; }
        public virtual ICollection<OrderTrade> OrderTrades { get; set; }
        public virtual ICollection<OrderPlayer> OrderPlayers { get; set; }
        public virtual ICollection<OrderPlayerProgram> OrderPlayerPrograms { get; set; }
    }
}
