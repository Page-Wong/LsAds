using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class PlayOrderModel {
        /// <summary>
        /// 广告订单id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// 指定开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 播放类型，0 视频；1 图片
        /// </summary>
        public int PlayType { get; set; }

    }
}
