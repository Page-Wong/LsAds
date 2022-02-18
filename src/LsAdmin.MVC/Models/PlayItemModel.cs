using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class PlayItemModel {
        public string FileId { get; set; }
        public string FileName { get; set; }

        public string Md5 { get; set; }
        public int Sort { get; set; }

        /// <summary>
        /// 播放类型，0 视频；1 图片
        /// </summary>
        public int PlayType { get; set; }
        public string OrderId { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public long Duration { get; set; }

    }
}
