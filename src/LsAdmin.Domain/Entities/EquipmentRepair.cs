using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities
{
    /// <summary>
    /// 设备报障维修信息表
    /// </summary>
   public class EquipmentRepair : BaseEntity {

        /// <summary>
        /// 设备编码
        /// </summary>
        [Display(Name = "设备编码")]
        public Guid EquipmentId { get; set; }

        /// <summary>
        /// 场所编码
        /// </summary>
        [Display(Name = "场所编码")]
        public Guid PlaceId { get; set; }

        /// <summary>
        /// 报障日期
        /// </summary>
        [Display(Name = "报障日期")]
        public DateTime WarningDate { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        [Display(Name = "问题描述")]
        public string ProblemDescription { get; set; }

        /// <summary>
        /// 场所联系人
        /// </summary>
        [MaxLength(30)]
        [Display(Name = "场所联系人")]
        public string PlaceContact { get; set; }

        /// <summary>
        /// 场所联系电话
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "场所联系电话")]
        public string PlaceContactPhone { get; set; }

        /// <summary>
        /// 记录状态
        /// </summary>
        [Display(Name = "记录状态")]
        public uint Status { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        [Display(Name = "处理人")]
        [MaxLength(30)]
        public string  ProcessingPerson { get; set; }

        /// <summary>
        /// 处理人联系电话
        /// </summary>
        [Display(Name = "处理人联系电话")]
        [MaxLength(50)]
        public string ProcessingPersonPhone{ get; set; }

        /// <summary>
        /// 处理方法
        /// </summary>
        [Display(Name = "处理方法")]
        public uint ProcessingMethod { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        [Display(Name = "处理结果")]
        [MaxLength(500)]
        public string ProcessingResults { get; set; }

        /// <summary>
        /// 设备故障素材 图片\视频
        /// </summary>
        public Guid BeforeMaterial { get; set; }

        /// <summary>
        /// 设备维修后素材 图片\视频
        /// </summary>
        public Guid AfterMaterial { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; }

        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }

    }
}
