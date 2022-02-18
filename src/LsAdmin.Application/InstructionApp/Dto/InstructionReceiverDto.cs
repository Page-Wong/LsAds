using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;

namespace LsAdmin.Application.InstructionApp.Dto {
    public class InstructionReceiverDto {
        public Guid EquipmentId { get; set; }
        public Guid MethodId { get; set; }
        public string NotifyUrl { get; set; }
        public string Params { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

    }
}