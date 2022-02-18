using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderTimeApp.Dtos;
using System;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LsAdmin.Application.OrderTimeApp
{
    public class OrderTimeAppService : BaseAppService<OrderTime, OrderTimeDto>, IOrderTimeAppService
    {
        private readonly IOrderTimeRepository _repository;

        public OrderTimeAppService(IOrderTimeRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public bool BatchDeleteByOrderId(Guid orderId)
        {
            try
            {
                var orderTimesids = this.GetByOrderId(orderId).Select(s => s.Id).ToList();
                if (orderTimesids.Count > 0)
                {
                    this.DeleteBatch(orderTimesids);
                }
                   
                return true;
            }
            catch (Exception)
            {
                return false;
            }     
        }

        public List<OrderTimeDto> GetByOrderId(Guid orderId)
        {
            //根据订单号查询
            var dtos = _repository.GetAllList(it =>it.OrderId== orderId);
            return Mapper.Map<List<OrderTimeDto>>(dtos);
        }

        public List<OrderTimeDto> GetByOrderIds(Guid[] orderIds) {
            //根据订单号查询
            var dtos = _repository.GetAllList(it => orderIds.Contains(it.OrderId));
            return Mapper.Map<List<OrderTimeDto>>(dtos);
        }
    }
}
