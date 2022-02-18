using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public class Equipment : BaseEntity
    {
        /// <summary>
        /// 设备型号编码
        /// </summary>
        [Display(Name = "设备型号编码")]
        public  Guid? EquipmentModelId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Display(Name = "设备名称")]
        [MaxLength(100)]
        public string Name { get; set; }

        //设备开始使用时间
        [Display(Name = "启用时间")]
        public DateTime? StartDate { get; set; }

        //设备停用时间
        [Display(Name = "停用时间")]
        public DateTime? DiscontinuationDate { get; set; }

        //设备状态
        [Display(Name = "设备状态")]
        public uint Status { get; set; }

        /// <summary>
        /// 设备标识号
        /// </summary>
        [Display(Name = "设备标识号")]
        [MaxLength(50)]
        public string DeviceId { get; set; }

        /// <summary>
        /// 拥有者
        /// </summary>
        [Display(Name = "拥有者")]
        public Guid? OwnerUserId { get; set; }

        /// <summary>
        /// 使用场所
        /// </summary>
        [Display(Name = "使用场所")]
        public Guid? PlaceId { get; set; }

        [Display(Name = "设备维修编码")]
        public Guid? EquipmentRepairId { get; set; }

        public decimal MapPointX { get; set; }
        public decimal MapPointY { get; set; }

        [MaxLength(30)]
        public string Province { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        [MaxLength(30)]
        public string District { get; set; }
        [MaxLength(50)]
        public string Street { get; set; }
        [MaxLength(100)]
        public string StreetNumber { get; set; }

        [ForeignKey("EquipmentModelId")]
        public EquipmentModel EquipmentModel { get; set; }

        [ForeignKey("OwnerUserId")]
        public User OwnerUser { get; set; }

        [ForeignKey("PlaceId")]
        public Place Place { get; set; }

        [ForeignKey("EquipmentRepairId")]
        public EquipmentRepair EquipmentRepair { get; set; }


        public virtual ICollection<EquipmentLogFile> EquipmentLogfiles { get; set; }

    }
}
