using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.ShoppingCartApp.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }

        ///<summary>
        ///用户
        ///</summary>
        public Guid UserId { get; set; }

        ///<summary>
        ///媒体位Id
        ///</summary>
        public string EquipmentIds { get; set; }

        /// <summary>
        /// 购物车类型
        /// </summary>
        public UInt16 Type { get; set; }

        public DateTime? CreateTime { get; set; }

        public const ushort CART_TYPE_1 = 1;
        public const ushort CART_TYPE_2 = 2;
        public virtual string TypeString
        {
            get
            {

                if (Type == CART_TYPE_1) return "原始购物车";
                if (Type == CART_TYPE_2) return "订单购物车";

                return "未知";
            }
        }
    }
}
