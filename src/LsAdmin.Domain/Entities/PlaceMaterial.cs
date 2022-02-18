using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class PlaceMaterial:BaseEntity
    {
        /// <summary>
        /// 场所编码
        /// </summary>
        public Guid PlaceId { get; set; }

        /// <summary>
        /// 使用素材
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

        public Place Place { get; set; }

        public Material Material { get; set; }

    }
}
