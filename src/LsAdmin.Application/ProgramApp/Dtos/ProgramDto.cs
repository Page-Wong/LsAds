using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.ProgramApp.Dtos {
    public class ProgramDto {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ProgramType Type { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public long Duration { get; set; }
        public string Content { get; set; }
        public Guid? OwnerUserId { get; set; }
        public virtual List<ProgramMaterialDto> ProgramMaterials { get; set; }

        public virtual string Launcher { get; set; }


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
