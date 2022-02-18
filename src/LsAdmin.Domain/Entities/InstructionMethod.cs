using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LsAdmin.Domain.Entities {
    public class InstructionMethod : BaseEntity
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string ParamRole { get; set; }
        public string ResultRole { get; set; }
    }
}
