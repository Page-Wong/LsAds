using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.AndroidapkApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace LsAdmin.Application.AndroidapkApp {
    public class AndroidapkAppService : BaseAppService<Androidapk, AndroidapkDto>, IAndroidapkAppService {

        private readonly IAndroidapkRepository _repository;
        public AndroidapkAppService(IAndroidapkRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }


    }
}
