using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.CollectionsBlacklistsApp.Dtos
{
    public class CollectionsBlacklistsDto
    {
        public Guid Id { get; set; }

        ///<summary>
        ///用户
        ///</summary>
        public Guid UserId { get; set; }

        ///<summary>
        ///媒体位Id
        ///</summary>
        public Guid EquipmentId { get; set; }

        /// <summary>
        /// 收藏/黑名单
        /// </summary>
        public UInt16 FavoriteType { get; set; }

        public DateTime? CreateTime { get; set; }

        public const ushort FAVORITE_TYPE_1 = 1;
        public const ushort FAVORITE_TYPE_2 = 2;
        public virtual string TypeString
        {
            get
            {

                if (FavoriteType == FAVORITE_TYPE_1) return "收藏";
                if (FavoriteType == FAVORITE_TYPE_2) return "黑名单";

                return "未知";
            }
        }
    }
}
