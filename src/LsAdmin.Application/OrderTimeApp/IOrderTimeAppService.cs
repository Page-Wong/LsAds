using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderTimeApp.Dtos;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.OrderTimeApp
{
    public interface IOrderTimeAppService:IBaseAppService<OrderTimeDto> {
        List<OrderTimeDto> GetByOrderId(Guid orderId);
        List<OrderTimeDto> GetByOrderIds(Guid[] orderIds);
        Boolean BatchDeleteByOrderId(Guid orderId);

    }
   

}
