using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.OrderMaterialApp {
    public interface IOrderMaterialAppService : IBaseAppService<OrderMaterialDto>
    {
        List<OrderMaterialDto> GetByOrderId(Guid orderId);
    }
}
