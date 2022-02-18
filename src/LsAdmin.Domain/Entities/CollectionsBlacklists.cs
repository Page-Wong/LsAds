using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class CollectionsBlacklists : BaseEntity
    {
        ///<summary>
        ///用户
        ///</summary>
        public Guid UserId { get; set; }

        ///<summary>
        ///媒体位Id
        ///</summary>
        public Guid EquipmentId { get; set; }

        /// <summary>
        ///收藏/黑名单
        /// </summary>
        public UInt16 FavoriteType { get; set; }


        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
