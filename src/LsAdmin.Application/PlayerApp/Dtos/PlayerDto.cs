using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.PlayerProgramApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.PlayerApp.Dtos {
    public class PlayerDto {

        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public virtual EquipmentDto Equipment { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Sort { get; set; }


        public PlayerType Type { get; set; }

        public Guid? OwnerUserId { get; set; }
        public virtual List<PlayerProgramDto> PlayerPrograms { get; set; }

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
