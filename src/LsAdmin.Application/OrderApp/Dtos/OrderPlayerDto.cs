using LsAdmin.Application.PlayerApp.Dtos;
using System;

namespace LsAdmin.Application.OrderApp.Dtos {
    public class OrderPlayerDto {
        public Guid OrderId { get; set; }
        public virtual OrderDto Order { get; set; }

        public Guid PlayerId { get; set; }
        public virtual PlayerDto Player { get; set; }
    }
}
