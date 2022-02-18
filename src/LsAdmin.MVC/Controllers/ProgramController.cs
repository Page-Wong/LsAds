using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.PlayHistoryApp;
using Microsoft.AspNetCore.Cors;
using LsAdmin.Application.OrderTimeApp;
using System.IO;
using System.Text;
using System;
using LsAdmin.Application.ProgramApp;
using LsAdmin.Application.ProgramApp.Dtos;
using System.Collections.Generic;
using LsAdmin.Application.MaterialApp.Dtos;
using LsAdmin.Application.MaterialApp;
using LsAdmin.Utility.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class ProgramController : LsAdminControllerBase
    {

        private readonly IProgramAppService _service;
        private readonly IMaterialAppService _materialAppService;
        public ProgramController(IProgramAppService service, IMaterialAppService materialAppService) {
            _service = service;
            _materialAppService = materialAppService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(Guid id) {

            return View(_service.Get(id));
        }

        [HttpGet]
        public IActionResult TextEditorPartialView(Guid programId, ushort materialType) {
            var program = _service.Get(programId);
            if (program == null || program.OwnerUserId != CurrentUser.Id) {
                return PartialView("_TextEditor");
            }
            return PartialView("_TextEditor", program.Content);
        }

        [HttpGet]
        public IActionResult MaterialListPartialView(Guid programId, ushort materialType) {
            var program = _service.Get(programId);
            if (program==null || program.OwnerUserId != CurrentUser.Id) {
                return PartialView("_MaterialTable", new List<MaterialDto>());
            }
            var materials = _materialAppService.GetByIds(program.ProgramMaterials.Select(it => it.MaterialId).ToList());
            return PartialView("_MaterialTable", materials.Where(it => it.MaterialType == materialType).ToList());
        }

        [HttpGet]
        public IActionResult MaterialTrPartialView(Guid materialId) {
            var material = _materialAppService.Get(materialId);
            if (material == null) {
                return PartialView("_MaterialTr", new List<MaterialDto>());
            }
            return PartialView("_MaterialTr", material);
        }

        [HttpGet]
        public IActionResult PageListPartialView(int startPage, int pageSize) {
            //int rowCount = 0;
            var result = _service.GetByOwnerUser(CurrentUser.Id, startPage, pageSize, out var rowCount);
            ViewBag.RowCount = rowCount;
            ViewBag.PageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize);
            return PartialView("_ProgramTable", result);
        }

        [HttpGet]
        public IActionResult SelectPageListPartialView(int startPage, int pageSize)
        {
            //int rowCount = 0;
            var result = _service.GetByOwnerUser(CurrentUser.Id, startPage, pageSize, out var rowCount);
            ViewBag.RowCount = rowCount;
            ViewBag.PageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize);
            return PartialView("_SelectProgramTable", result);
        }

        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult Save(ProgramDto dto, string[] materialIds) {
            try {
                if (dto.Id == null || dto.Id == Guid.Empty) {
                    dto.Id = Guid.NewGuid();
                }
                if (materialIds != null || materialIds.Count() > 0) {
                    var programMaterials = new List<ProgramMaterialDto>();
                    foreach (var materialId in materialIds.Distinct()) {
                        programMaterials.Add(new ProgramMaterialDto() { ProgramId = dto.Id, MaterialId = Guid.Parse(materialId) });
                    }
                    _service.InsertOrUpdate(ref dto);
                    dto.ProgramMaterials = programMaterials;
                    dto.Duration = programMaterials.Sum(it => _materialAppService.Get(it.MaterialId).Duration);
                }
                else {
                    dto.ProgramMaterials = new List<ProgramMaterialDto>();
                    dto.Duration = 0;
                }
                dto.OwnerUserId = CurrentUser.Id;
                _service.InsertOrUpdate(ref dto);
                return Json(new { Result = "Success" });
            }
            catch (Exception ex) {
                return Json(new { Result = "Faild", Message = ex.Message });

            }
        }

        public IActionResult GetAllPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _service.GetByOwnerUser(CurrentUser.Id, startPage, pageSize, out rowCount);            
            return Json(new {
                RowCount = rowCount,
                PageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                Rows = result,
            });
        }

        public IActionResult DeleteMuti(string ids) {
            try {
                string[] idArray = ids.Split(',');
                List<Guid> delIds = new List<Guid>();
                foreach (string id in idArray) {
                    delIds.Add(Guid.Parse(id));
                }
                _service.DeleteBatch(delIds);
                return Json(new {
                    Result = "Success"
                });
            }
            catch (Exception ex) {
                return Json(new {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        public IActionResult Delete(Guid id) {
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
        public IActionResult Get(Guid id) {
            var dto = _service.Get(id);
            //if (dto.OwnerUserId != CurrentUser.Id) {
            //    dto = null;
            //}
            return Json(dto);
        }
    }
}
