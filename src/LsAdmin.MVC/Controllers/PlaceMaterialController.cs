using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.PlaceMaterialApp;
using LsAdmin.Application.PlaceMaterialApp.Dtos;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.MaterialApp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class PlaceMaterialController : LsAdminControllerBase
    {
        private readonly IPlaceMaterialAppService _service;
        private readonly IMaterialAppService _mservice;

        public PlaceMaterialController(IPlaceMaterialAppService service, IMaterialAppService mservice)
        {
            _service = service;
            _mservice = mservice;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllPageList(int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetAllPageList(startPage, pageSize, out rowCount).Where(m => m.CreateUserId.Equals(CurrentUser.Id));
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        public IActionResult Add(Guid id, PlaceMaterialDto dto)
        {
            try
            {
                var mdto = _mservice.Get(id);

                dto.Id = Guid.Empty;
                dto.CreateTime = DateTime.Now;
                dto.CreateUserId = CurrentUser.Id;
                dto.MaterialId = id;
                //dto.MaterialName = mdto.Name;

                //按文件名校验重复上传
                var existPerson = _service.GetAllList().FirstOrDefault(f => f.MaterialName == dto.MaterialName);
                if (existPerson != null)
                {
                    return Json(new { Result = "Faild" });
                }

                _service.Insert(ref dto);
                return Json(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }

        public IActionResult Delete(string ids)
        {
            try
            {
                string[] idArray = ids.Split(',');
                List<Guid> delIds = new List<Guid>();
                foreach (string id in idArray)
                {
                    delIds.Add(Guid.Parse(id));
                }
                _service.DeleteBatch(delIds);
                return Json(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }

        public IActionResult DeleteSingle(Guid id)
        {
            try
            {
                _service.Delete(id);
                return Json(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }


        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }

        public IActionResult Edit(PlaceMaterialDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var odto = _service.Get(dto.Id);
            odto.Remarks = dto.Remarks;

            if (_service.InsertOrUpdate(ref odto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

       

    }
}
