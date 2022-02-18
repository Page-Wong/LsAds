using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LsAdmin.Application.EquipmentReplaceApp.Dtos
{
    public  class EquipmentReplaceDto
    {
        public Guid Id { get; set; }
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
    }
}
