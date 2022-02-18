using System;
using System.IO;
using System.Linq;

namespace LsAdmin.Application.FilesApp.Dtos {
 

    public class FilesDto
    {

        public Guid Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FilenameExtension { get; set; }

        /// <summary>
        ///素材缩略图 
        /// </summary>
        public byte[] Thumbnail { get; set; }

        /// <summary>
        /// MD5
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 拥有对象
        /// </summary>
        public Guid? OwnerObjId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public virtual ushort Type
        {
            get
            {
                if (FilenameExtension == null)
                {
                    return 0;
                }
                string[] type1 = new string[] { "png", "jpg", "jpeg", "bmp" };
                string[] type2 = new string[] { "mp4", "avi", "3gp" };

                if (type1.Contains(FilenameExtension.ToLower()))
                {
                    return 1;
                }
                if (type2.Contains(FilenameExtension.ToLower()))
                {
                    return 2;
                }
                return 0;
            }
        }
    }
}
