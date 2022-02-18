using LsAdmin.Application.PlayerProgramApp.Dtos;
using System;

namespace LsAdmin.Application.OrderApp.Dtos {
    public class OrderPlayerProgramDto {
        public Guid OrderId { get; set; }
        public virtual OrderDto Order { get; set; }

        public Guid PlayerProgramId { get; set; }
        public virtual PlayerProgramDto PlayerProgram { get; set; }
    }
}
