using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class OrderPlayerProgram {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid PlayerProgramId { get; set; }
        public PlayerProgram PlayerProgram { get; set; }

    }
}
