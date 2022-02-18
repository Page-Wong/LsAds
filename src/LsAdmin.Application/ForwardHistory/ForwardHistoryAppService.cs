using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.ForwardHistoryApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace LsAdmin.Application.ForwardHistoryApp {
    public class ForwardHistoryAppService : BaseAppService<ForwardHistory, ForwardHistoryDto>, IForwardHistoryAppService {

        private readonly IForwardHistoryRepository _repository;
        public ForwardHistoryAppService(IForwardHistoryRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }


    }
}
