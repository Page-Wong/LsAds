using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using AutoMapper;

namespace LsAdmin.Application.OrderMaterialApp {
    public class OrderMaterialAppService : BaseAppService<OrderMaterial, OrderMaterialDto>, IOrderMaterialAppService {
        private readonly IOrderMaterialRepository _repository;
        public OrderMaterialAppService(IOrderMaterialRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }
        public List<OrderMaterialDto> GetByOrderId(Guid orderId)
        {
            //根据订单号查询所有素材
            var ordermaterialdto = _repository.GetAllList(it => it.OrderId == orderId);
            return Mapper.Map<List<OrderMaterialDto>>(ordermaterialdto);
        }

    }
}
