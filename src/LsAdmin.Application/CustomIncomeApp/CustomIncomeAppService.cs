using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.CustomIncomeApp.Dtos;
using Microsoft.AspNetCore.Http;

namespace LsAdmin.Application.CustomIncomeApp
{
    public class CustomIncomeAppService : BaseAppService<CustomIncome, CustomIncomeDto>, ICustomIncomeAppService
    {
        private readonly ICustomIncomeRepository _repository;
        public CustomIncomeAppService(ICustomIncomeRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
        }
    }
}
