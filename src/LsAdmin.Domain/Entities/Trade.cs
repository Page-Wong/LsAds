using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public class Trade : BaseEntity {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }


        /// <summary>
        /// 流水金额
        /// </summary>
        public float Amount { get; set; }


        /// <summary>
        /// 流水类型（以用户角色为中心） 收入或支出
        /// </summary>
        public ushort Type { get; set; }

        /// <summary>
        /// 业务类型 
        /// </summary>
        public Guid BusinessTypeId { get; set; }
        public virtual TradeBusinessType BusinessType { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>        
        [Required]
        public string TradeNo { get; set; }

        /// <summary>
        /// 交易方式 支付宝、银联 等等
        /// </summary>
        public ushort TradeMethod { get; set; }


        /// <summary>
        /// 订单状态 交易中、交易成功、交易失败 等等
        /// </summary>
        public ushort TradeStatus { get; set; }
    }
}
