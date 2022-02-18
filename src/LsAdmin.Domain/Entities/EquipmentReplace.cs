using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    /// <summary>
    /// 更换设备信息表
    /// </summary>
    public class EquipmentReplace : BaseEntity{

        /// <summary>
        /// 原设备编码
        /// </summary>
        [Display(Name = "原设备编码")]
        public Guid OldEquipmentId { get; set; }


        /// <summary>
        /// 设备更换后旧设备状态
        /// </summary>
        public uint AfterOldEquipmentStatus { get; set; }

        /// <summary>
        /// 新设备编码
        /// </summary>
        [Display(Name = "新设备编码")]
        public Guid NewEquipmentId { get; set; }


        /// <summary>
        /// 设备更换后新设备状态
        /// </summary>
        public uint AfterNewEquipmentStatus { get; set; }

        /// <summary>
        /// 场所编码
        /// </summary>
        [Display(Name = "场所编码")]
        public Guid PlaceId { get; set; }

        /// <summary>
        /// 更换原因
        /// </summary>
        [Display(Name = "更换原因")]
        public string reason { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        [MaxLength(30)]
        [Display(Name = "处理人")]
        public string Operator { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        [Display(Name = "处理时间")]
        public DateTime? OperationTime { get; set; }

        [ForeignKey("NewEquipmentId")]
        public virtual Equipment OldEquipment { get; set; }

        [ForeignKey("OldEquipmentId")]
        public virtual Equipment NewEquipment { get; set; }

        [ForeignKey("PlaceId")]
        public virtual Place Place { get; set; }

    }
}
