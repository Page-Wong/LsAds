using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.PlayHistoryApp;
using Microsoft.AspNetCore.Cors;
using LsAdmin.Application.AdministrativeAreaApp;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class AdministrativeAreaController : LsAdminControllerBase
    {

        private readonly IAdministrativeAreaAppService _service;
        public AdministrativeAreaController(IAdministrativeAreaAppService service) {
            _service = service;
        }


        [HttpGet]
        public IActionResult GetProvinces() {
            return Json(new {
                Result = "Success",
                Data = _service.GetProvinces().Select(item => new { Code = item.Code, Name = item.Name }).ToList()
            });            
        }

        [HttpGet]
        public IActionResult GetCitys(uint Code) {
            return Json(new {
                Result = "Success",
                Data = _service.GetCitysByProvinceCode(Code).Select(item => new { Code = item.Code, Name = item.Name}).ToList()
            });
        }

        [HttpGet]
        public IActionResult GetDistricts(uint Code) {
            return Json(new {
                Result = "Success",
                Data = _service.GetDistrictsByCityCode(Code).Select(item => new { Code = item.Code, Name = item.Name }).ToList()
            });
        }
    }
}
