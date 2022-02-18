using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace LsAdmin.Application.OrderEquipmentApp
{
    public class OrderEquipmentAppService : BaseAppService<OrderEquipment, OrderEquipmentDto>, IOrderEquipmentAppService {
        private readonly IOrderEquipmentRepository _repository;


        public OrderEquipmentAppService(IOrderEquipmentRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public List<OrderEquipmentDto> GetByOrderId(Guid orderId)
        {
            var orderEquipments = _repository.GetAllList(g => g.OrderId == orderId);                   
            return Mapper.Map<List<OrderEquipmentDto>>(orderEquipments);
        }

        public bool BatchDeleteByOrderId(Guid orderId)
        {
            try
            {
                var orderEquipments = this.GetByOrderId(orderId).Select(s => s.Id).ToList();
                if (orderEquipments.Count > 0)
                {
                    this.DeleteBatch(orderEquipments);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
