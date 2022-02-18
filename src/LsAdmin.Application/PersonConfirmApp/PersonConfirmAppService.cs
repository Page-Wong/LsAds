using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using LsAdmin.Application.PersonConfirmApp.Dtos;


namespace LsAdmin.Application.PersonConfirmApp {
    public class PersonConfirmAppService : BaseAppService<PersonConfirm, PersonConfirmDto>, IPersonConfirmAppService {

        private readonly IPersonConfirmRepository _repository;
        public PersonConfirmAppService(IPersonConfirmRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
        }


    }
}
