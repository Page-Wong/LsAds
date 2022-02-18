using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Domain.Entities
{
   public class EquipmentModel : BaseEntity{
        /// <summary>
        /// 设备型号
        /// </summary>
        [Required(ErrorMessage = "设备型号不能为空")]
        public string Model { get; set;}

        /// <summary>
        /// 屏幕尺寸（寸）
        /// </summary>
        public float? ScreenSize { get; set; }

        /// <summary>
        /// 屏幕宽（cm）
        /// </summary>
        public float? ScreenWidth { get; set; }

        /// <summary>
        /// 屏幕高（cm）
        /// </summary>
        public float? ScreenHeight { get; set; }


        /// <summary>
        /// 分辨率-宽
        /// </summary>
        [Display(Name = "分辨率-宽")]
        public uint? ResolutionRatioWidth { get; set; }

        /// <summary>
        /// 分辨率长
        /// </summary>
        [Display(Name = "分辨率-长")]
        public uint? ResolutionRatioLength { get; set; }


        /// <summary>
        /// 屏幕材质
        /// </summary>
        public string ScreenMaterial { get; set; }

        /// <summary>
        /// 有声音
        /// </summary>
        public Boolean voiced { get; set; }

        /// <summary>
        /// 副屏个数
        /// </summary>
        public uint SideScreensNumber { get; set; }

        /// <summary>
        /// 适用范围(标签)
        /// </summary>
        public string  ApplyTo { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }



    }
}
