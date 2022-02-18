using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using LsAdmin.Application.OrderPlaceApp.Dtos;
using LsAdmin.Application.TradeApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Application.OrderApp.Dtos {
    public class OrderTradeDto {
        public Guid OrderId { get; set; }
        public OrderDto Order { get; set; }

        public Guid TradeId { get; set; }
        public TradeDto Trade { get; set; }
    }
}
