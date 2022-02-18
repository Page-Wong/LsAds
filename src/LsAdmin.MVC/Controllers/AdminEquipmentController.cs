using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.EquipmentManagementApp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class AdminEquipmentController : LsAdminControllerBase
    {
        private readonly IEquipmentAppService _service;
        public AdminEquipmentController(IEquipmentAppService service)
        {
            _service = service;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPageEquipmentList(int startPage, int pageSize, decimal MaxLat, decimal MaxLng, decimal MinLat, decimal MinLng) {
            int rowCount = 0;
            var result = _service.GetPageEquipmentByMapPoint(startPage, pageSize, out rowCount, MaxLat, MaxLng, MinLat, MinLng);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }
    }
}
