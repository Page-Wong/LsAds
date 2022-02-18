using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.EquipmentApplicationApp;
using LsAdmin.Application.EquipmentApplicationApp.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class EquipmentApplicationController : LsAdminControllerBase
    {
        private readonly IEquipmentApplicationAppService _service;
        public EquipmentApplicationController(IEquipmentApplicationAppService service)
        {
            _service = service;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetEquipmentApplicationByPlace(Guid placeId, int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetEquipmentApplicationByPlace(placeId, startPage, pageSize, out rowCount);

            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,

            });
        }

        public IActionResult AuditingIndex()
        {
            return View();
        }


        public IActionResult GetEquipmentApplicationByStatus(uint status, int startPage, int pageSize)
        {
            int rowCount = 0;
           
            Dictionary<ushort, int> statusRowCount = new Dictionary<ushort, int>();
            var result = _service.GetEquipmentApplicationByStatus(status, startPage, pageSize, out rowCount, out statusRowCount);
            int allrowCount = 0;
            foreach(var count in statusRowCount){
                allrowCount += count.Value;
            }

            return Json(new{
                rowCount = rowCount,
                allrowCount= allrowCount,
                statusRowCount = statusRowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }


        public IActionResult  GetEquipmentApplicationWithStatusRowCount(int startPage, int pageSize){
            int rowCount = 0;
            Dictionary<ushort, int> statusRowCount = new Dictionary<ushort, int>();
            var result = _service.GetEquipmentApplicationWithStatusRowCount( startPage, pageSize, out rowCount, out statusRowCount);
            int allrowCount = 0;
            foreach (var count in statusRowCount){
                allrowCount += count.Value;
            }
            return Json(new
            {
                rowCount = rowCount,
                allrowCount = allrowCount,
                statusRowCount = statusRowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        public IActionResult UpdateStatus(Guid id, ushort status){

            if (id == null) return Json(new { Result = "Faild", Message = "系统中没找到对应的申请记录！" });

            if(!(new uint[] {0,1,2,3 }).Contains(status)) return Json(new { Result = "Faild", Message = "你提交的状态信息有误！" });
        
            try{
                if (!ModelState.IsValid)  return Json(new{ Result = "Faild", Message = GetModelStateError() });
                
                var dto = _service.Get(id);

                if (dto == null) return Json(new { Result = "Faild", Message = "系统中没找到对应的申请记录！" });
                
                switch (status){
                    case 0: if (dto.Status != 1) { return Json(new { Result = "Faild", Message = "记录的状态为："+dto.StatusString+",操作失败！" }); }break;
                    case 1: if (!(new uint[] { 0, 2, 3 }).Contains(dto.Status)){ return Json(new { Result = "Faild", Message = "记录的状态为：" + dto.StatusString + ",操作失败！" });}break;
                    case 2:
                    case 3: if (dto.Status != 1) { return Json(new { Result = "Faild", Message = "记录的状态为：" + dto.StatusString + ",操作失败！" }); } break;
                    default: return Json(new { Result = "Faild", Message = "你提交的状态信息有误！" });
                }
        
                dto.Status = status;

                if (_service.Update(dto)){
                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Faild" });
            }
            catch (Exception ex) {
                return Json(new { Result = "Faild", Message = "系统出错，请联系系统维护人员！" });
            }
        }



        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }

        public IActionResult Edit(EquipmentApplicationDto dto)
        {
           
            if (dto.Id == Guid.Empty)
            {
                dto.CreateUserId = CurrentUser.Id;
                dto.CreateTime = DateTime.Now;
                dto.Status = 0;
            }

            if(dto.Id !=Guid.Empty)
            {
                if (!(new uint[] { 0, 1, 2, 3 }).Contains(dto.Status)) return Json(new { Result = "Faild", Message = "您的审核状态信息有误！" });

                try
                {

                    switch (dto.Status)
                    {
                        case 0:
                        case 1: if (!(new uint[] { 0, 2, 3 }).Contains(dto.Status)) { return Json(new { Result = "Faild", Message = "您的申请审核状态为：" + dto.StatusString + "。不可再修改申请信息。" }); } break;
                        case 2: if (!(new uint[] { 0, 1, 3 }).Contains(dto.Status)) { return Json(new { Result = "Faild", Message = "您的申请审核状态为：" + dto.StatusString + "。不可再修改申请信息。" }); } break;
                        case 3: if (!(new uint[] { 0, 1, 2 }).Contains(dto.Status)) { return Json(new { Result = "Faild", Message = "您的申请审核状态为：" + dto.StatusString + "。不可再修改申请信息。" }); } break;
                        default: return Json(new { Result = "Faild", Message = "您的审核状态信息有误！" });
                    }

                    dto.CreateTime = DateTime.Now;

                    if (string.IsNullOrEmpty(dto.Reason))
                    {
                        return Json(new { Result = "Faild", Message = "申请原因不可为空！" });
                    }

                    if (_service.Update(dto))
                    {
                        return Json(new { Result = "Success" });
                    }
                    return Json(new { Result = "Faild" });
                }
                catch (Exception ex)
                {
                    return Json(new { Result = "Faild", Message = "系统出错，请联系系统维护人员！" });
                }

            }

            if (string.IsNullOrEmpty(dto.Reason))
            {
                return Json(new { Result = "Faild", Message = "申请原因不可为空！" });
            }
            
            if (_service.InsertOrUpdate(ref dto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }
    }
}

