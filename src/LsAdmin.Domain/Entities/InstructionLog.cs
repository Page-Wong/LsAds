using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {

    public enum InstructionLogType {
        Send,
        Receive
    }
    public class InstructionLog : BaseEntity {
        public Guid InstructionId { get; set; }
        public Guid EquipmentId { get; set; }
        public InstructionLogType Type { get; set; }
        public InstructionStatus InstructionStatus { get; set; }
        public string Result { get; set; }
        public Instruction Instruction { get; set; }
        public Equipment Equipment { get; set; }

    }
}
