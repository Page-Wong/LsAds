using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class ShoppingCart : BaseEntity
    {
        ///<summary>
        ///用户
        ///</summary>
        public Guid UserId { get; set; }
        
        ///<summary>
        ///媒体位Id
        ///</summary>
        public string EquipmentIds { get; set; }

        /// <summary>
        ///购物车类型
        /// </summary>
        public UInt16 Type { get; set; }


        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
