using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.TradeBusinessTypeApp.Dtos;
using System;

namespace LsAdmin.Application.TradeBusinessTypeApp {
    public class TradeBusinessTypeAppService : BaseAppService<TradeBusinessType, TradeBusinessTypeDto>, ITradeBusinessTypeAppService {

        private readonly ITradeBusinessTypeRepository _repository;
        public TradeBusinessTypeAppService(ITradeBusinessTypeRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public Guid GetIdByName(string name) {
            var type = _repository.FirstOrDefault(item => item.Name == name);
            if (type == null) {
                throw new NullReferenceException();
            }
            return type.Id;
        }
    }
}
