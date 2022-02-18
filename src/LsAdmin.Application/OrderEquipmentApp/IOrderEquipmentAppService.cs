using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.OrderEquipmentApp
{
    public interface IOrderEquipmentAppService : IBaseAppService<OrderEquipmentDto>
    {
        bool BatchDeleteByOrderId(Guid orderId);
        List<OrderEquipmentDto> GetByOrderId(Guid orderId);
    }
}
