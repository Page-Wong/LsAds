using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.EquipmentIncomeApp.Dtos;
using Microsoft.AspNetCore.Http;

namespace LsAdmin.Application.EquipmentIncomeApp
{
    public class EquipmentIncomeAppService : BaseAppService<EquipmentIncome, EquipmentIncomeDto>, IEquipmentIncomeAppService
    {
        private readonly IEquipmentIncomeRepository _repository;
        public EquipmentIncomeAppService(IEquipmentIncomeRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
        }
    }
}
