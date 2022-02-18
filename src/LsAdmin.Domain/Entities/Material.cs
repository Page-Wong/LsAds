using System;

namespace LsAdmin.Domain.Entities {
    public class Material : BaseEntity {
        /// <summary>
        /// 播放素材名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FilenameExtension { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public byte[] Thumbnail { get; set; }

        /// <summary>
        /// MD5
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// 持续时间
        /// </summary>
        public long Duration { get; set; }
        /// <summary>

        /// <summary>
        /// 拥有者
        /// </summary>
        public Guid? OwnerUserId { get; set; }   
     
    }
}
