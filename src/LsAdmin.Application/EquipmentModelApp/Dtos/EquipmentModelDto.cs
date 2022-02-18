using System;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Application.EquipmentModelApp.Dtos
{
    public class EquipmentModelDto
    {

        public Guid Id { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        [MaxLength(100)]
        public string Model { get; set; }

        /// <summary>
        /// 屏幕尺寸（寸）
        /// </summary>
        public double? ScreenSize { get; set; }

        /// <summary>
        /// 屏幕宽（cm）
        /// </summary>
        public double? ScreenWidth { get; set; }

        /// <summary>
        /// 屏幕高（cm）
        /// </summary>
        public double? ScreenHeight { get; set; }

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
        [MaxLength(50)]
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
        public string ApplyTo { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manufacturer { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }



        /// <summary>
        /// 分辨率
        /// </summary>
        public virtual string ResolutionRatio {
            get { return ResolutionRatioLength * ResolutionRatioWidth > 0 ? ResolutionRatioLength.ToString() + "*" + ResolutionRatioWidth.ToString() : "";}
        }


        public virtual string ScreenRatio{
            get{
                if(ResolutionRatioWidth>0 && ResolutionRatioLength > 0)
                {
                    uint bigmun;
                    uint smallnum;
                    uint gr;

                    if (ResolutionRatioWidth > ResolutionRatioLength){
                        bigmun = (uint)ResolutionRatioWidth;
                        smallnum = (uint)ResolutionRatioLength;
                    }
                    else { 
                        bigmun = (uint)ResolutionRatioLength;
                        smallnum = (uint)ResolutionRatioWidth;
                    }

                    while (true) { if (bigmun % smallnum == 0) { gr = smallnum; break; } else { uint r = (uint)bigmun%(uint)smallnum; bigmun = smallnum; smallnum = r; } }
                    
                    return (ResolutionRatioLength / smallnum).ToString() + ":" + (ResolutionRatioWidth / smallnum).ToString();

                }
                else
                return "";
            }
        }
         
        public virtual string voicedName { get {
                if (voiced == true){
                    return "有声音";
                }
                else{
                    return "无声音";
                }
            }
        }
}
}
