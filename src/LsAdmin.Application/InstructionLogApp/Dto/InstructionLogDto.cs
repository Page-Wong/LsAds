using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;


namespace LsAdmin.Application.InstructionLogApp.Dto {
    public class InstructionLogDto {
        public Guid InstructionId { get; set; }
        public Guid EquipmentId { get; set; }
        public InstructionLogType Type { get; set; }
        public InstructionStatus InstructionStatus { get; set; }
        public string Result { get; set; }
        public Instruction Instruction { get; set; }
        public EquipmentDto Equipment { get; set; }


        public Guid Id { get; set; }

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