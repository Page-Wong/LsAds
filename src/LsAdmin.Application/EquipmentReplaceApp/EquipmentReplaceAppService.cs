using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.EquipmentReplaceApp.Dtos;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.Application.EquipmentReplaceApp
{
    class EquipmentReplaceAppService : BaseAppService<EquipmentReplace, EquipmentReplaceDto>, IEquipmentReplaceAppService
    {
        private readonly IEquipmentReplaceRepository _repository;
        public EquipmentReplaceAppService(IEquipmentReplaceRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }
    }
}
