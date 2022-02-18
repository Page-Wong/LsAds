using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public enum InstructionStatus {
        Waiting,
        Send,
        Receive,
        Processing,
        Done
    }
    public class Instruction : BaseEntity
    {
        public Guid EquipmentId { get; set; }
        public Guid MethodId { get; set; }
        public InstructionStatus Status { get; set; }
        public string NotifyUrl { get; set; }
        public string Sign { get; set; }
        public long Timestamp { get; set; }
        public string Params { get; set; }
        public string Content { get; set; }
        public string Token { get; set; }

    }
}
