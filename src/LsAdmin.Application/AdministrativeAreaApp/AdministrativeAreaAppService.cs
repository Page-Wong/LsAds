using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.AdministrativeAreaApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace LsAdmin.Application.AdministrativeAreaApp {
    public class AdministrativeAreaAppService : BaseAppService<AdministrativeArea, AdministrativeAreaDto>, IAdministrativeAreaAppService {

        private readonly IAdministrativeAreaRepository _repository;
        public AdministrativeAreaAppService(IAdministrativeAreaRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public AdministrativeAreaDto GetByCode(uint Code) {
            return Mapper.Map<AdministrativeAreaDto>(_repository.GetEntities().FirstOrDefault(item => item.Code == Code));
        }

        public List<AdministrativeAreaDto> GetCitysByProvinceCode(uint Code) {
            return Mapper.Map<List<AdministrativeAreaDto>>(_repository.GetEntities().Where(item => item.ParentCode == Code).ToList());
        }

        public List<AdministrativeAreaDto> GetDistrictsByCityCode(uint Code) {
            return Mapper.Map<List<AdministrativeAreaDto>>(_repository.GetEntities().Where(item => item.ParentCode == Code).ToList());
        }

        public List<AdministrativeAreaDto> GetProvinces() {
            return Mapper.Map<List<AdministrativeAreaDto>>(_repository.GetEntities().Where(item => item.ParentCode == 0).ToList());
        }
    }
}
