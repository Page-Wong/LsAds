using LsAdmin.Application.AdministrativeAreaApp.Dtos;
using LsAdmin.Application.Imp;
using System.Collections.Generic;

namespace LsAdmin.Application.AdministrativeAreaApp {
    public interface IAdministrativeAreaAppService : IBaseAppService<AdministrativeAreaDto> {
        List<AdministrativeAreaDto> GetProvinces();
        List<AdministrativeAreaDto> GetCitysByProvinceCode(uint Code);
        List<AdministrativeAreaDto> GetDistrictsByCityCode(uint Code);
        AdministrativeAreaDto GetByCode(uint Code);
    }
}
