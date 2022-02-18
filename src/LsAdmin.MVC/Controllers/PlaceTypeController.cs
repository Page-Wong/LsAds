using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.PlaceTypeApp;
using LsAdmin.MVC.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class PlaceTypeController : Controller
    {
        private readonly IPlaceTypeAppService _service;
        public PlaceTypeController(IPlaceTypeAppService service)
        {
            _service = service;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// 获取功能树JSON数据
        
        [HttpGet]
        public IActionResult GetGridData()
        {
            var dtos = _service.GetAllList();
            List<GridModel> gridModels = new List<GridModel>();
            foreach (var dto in dtos)
            {
                gridModels.Add(new GridModel() { Id = dto.Id.ToString(), Text = dto.Type });
            }
            return Json(gridModels);
        }
    }
}
