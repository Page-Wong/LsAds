using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using LsAdmin.Application.EnterpriseConfirmApp.Dtos;

namespace LsAdmin.Application.EnterpriseConfirmApp {
   public class EnterpriseConfirmAppService : BaseAppService<EnterpriseConfirm, EnterpriseConfirmDto>, IEnterpriseConfirmAppService {

        private readonly IEnterpriseConfirmRepository _repository;
        public EnterpriseConfirmAppService(IEnterpriseConfirmRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
        }


    }
}
