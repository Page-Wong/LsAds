using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderPlaceApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace LsAdmin.Application.OrderPlaceApp
{
    public class OrderPlaceAppService : BaseAppService<OrderPlace, OrderPlaceDto>, IOrderPlaceAppService
    {
        private readonly IOrderPlaceRepository _repository;

        public OrderPlaceAppService(IOrderPlaceRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }


    }
}