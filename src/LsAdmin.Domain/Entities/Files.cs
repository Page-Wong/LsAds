using System;

namespace LsAdmin.Domain.Entities
{
    public class Files : BaseEntity
    {
        /// <summary>
        /// 文件名称
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
        /// 所属记录号编码
        /// </summary>
        public Guid? OwnerObjId { get; set; }

    }
}
