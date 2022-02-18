using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.PlaceMaterialApp.Dtos;
using LsAdmin.Application.MaterialApp;
using LsAdmin.Application.PlaceMaterialApp;

namespace LsAdmin.MVC.Controllers
{
    public class PlacePlayMyselfController : LsAdminControllerBase
    {

        private readonly IMaterialAppService _materialService;
        private readonly IPlaceMaterialAppService _placeMaterialService;
        public PlacePlayMyselfController(IMaterialAppService materialService, IPlaceMaterialAppService placeMaterialService)
        {
            _materialService = materialService;
            _placeMaterialService = placeMaterialService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult Save(Guid placeId, List<PlaceMaterialDto> dtos)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }

            if (_placeMaterialService.SaveToOrder(placeId, dtos))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

        public List<PlaceMaterialDto> GetPlaceMaterials(Guid placeid, ushort materialType)
        {
            return _placeMaterialService.GetPlaceMaterials(placeid, materialType);
        }

        public IActionResult GetAllPageList(int startPage, int pageSize, Guid placeid, ushort materialType = 0)
        {
            int rowCount = 0;

            var result = _placeMaterialService.GetPageListByType(startPage, pageSize, out rowCount, placeid, materialType);

            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

    }
}