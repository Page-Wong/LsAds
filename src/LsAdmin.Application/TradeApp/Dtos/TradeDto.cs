using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.TradeBusinessTypeApp;
using LsAdmin.Application.TradeBusinessTypeApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Utility.Service;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LsAdmin.Application.TradeApp.Dtos {
    public class TradeDto {        

        public Guid Id { get; set; }

        /// <summary>
        /// 交易摘要
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

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }


        /// <summary>
        /// 流水金额
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// 流水类型 支付、退款、消费 等等
        /// </summary>
        public ushort Type { get; set; }

        public const ushort TYPE_NONE = 0;
        public const ushort TYPE_INCOME = 1;
        public const ushort TYPE_SPEND = 2;
        public virtual string TypeString {
            get {
                if (Type == TYPE_INCOME) return "收入";
                if (Type == TYPE_SPEND) return "支出";               
                return "未知";                
            }
        }

        /// <summary>
        /// 业务类型 
        /// </summary>
        public Guid BusinessTypeId { get; set; }
        public virtual TradeBusinessTypeDto BusinessType { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 交易方式 支付宝、银联 等等
        /// </summary>
        public ushort TradeMethod { get; set; }

        public const ushort TRADEMETHOD_NONE = 0;
        public const ushort TRADEMETHOD_ALIPAY = 1;
        public const ushort TRADEMETHOD_UNIONPAY = 2;
        public const ushort TRADEMETHOD_WECHATPAY = 3;
        public const ushort TRADEMETHOD_POINT = 4;
        public virtual string TradeMethodString {
            get {
                if (TradeMethod == TRADEMETHOD_ALIPAY) return "支付宝";
                if (TradeMethod == TRADEMETHOD_UNIONPAY) return "银联";
                if (TradeMethod == TRADEMETHOD_WECHATPAY) return "微信支付";
                if (TradeMethod == TRADEMETHOD_POINT) return "积分";
                return "未知";
            }
        }
        public virtual string TradeMethodName {
            get {
                if (TradeMethod == TRADEMETHOD_ALIPAY) return "alipay";
                if (TradeMethod == TRADEMETHOD_UNIONPAY) return "unionpay";
                if (TradeMethod == TRADEMETHOD_WECHATPAY) return "wechatpay";
                if (TradeMethod == TRADEMETHOD_POINT) return "point";
                return "未知";
            }
        }

        /// <summary>
        /// 订单状态 交易中、交易成功、交易失败 等等
        /// </summary>
        public ushort TradeStatus { get; set; }

        public const ushort TRADESTATUS_NONE = 0;
        public const ushort TRADESTATUS_TRADING = 1;
        public const ushort TRADESTATUS_SUCCESS = 2;
        public const ushort TRADESTATUS_FAILD = 3;
        public const ushort TRADESTATUS_CANCEL = 4;
        public virtual string TradeStatusString {
            get {
                if (TradeStatus == TRADESTATUS_TRADING) return "交易中";
                if (TradeStatus == TRADESTATUS_SUCCESS) return "成功";
                if (TradeStatus == TRADESTATUS_FAILD) return "失败";
                if (TradeStatus == TRADESTATUS_CANCEL) return "取消";
                return "未知";
            }
        }
    }
}
