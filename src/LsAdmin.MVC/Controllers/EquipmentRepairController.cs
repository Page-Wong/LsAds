using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentModelApp;
using LsAdmin.Application.EquipmentModelApp.Dtos;
using LsAdmin.Application.EquipmentRepairApp;
using LsAdmin.Application.EquipmentRepairApp.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LsAdmin.MVC.Controllers
{
    public class EquipmentRepairController : LsAdminControllerBase{
        private readonly IEquipmentRepairAppService _service;
        private readonly IEquipmentAppService _serviceEquipment;
        public EquipmentRepairController(IEquipmentRepairAppService service, IEquipmentAppService serviceEquipment)
        {
            _service = service;
            _serviceEquipment = serviceEquipment;

        }

        // GET: /<controller>/
        [AllowAnonymous]
        public IActionResult Index(){
            return View();
        }
        public IActionResult GetOwnerEquipmentRepairPageList(int startPage, int pageSize,uint status)
        {
            int rowCount = 0;
            var result = _service.GetOwnerEquipmentRepairPageList(startPage, pageSize, status, out rowCount, out int unConfirmedCount, out int confirmedCount, out int completeCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                statusUnConfirmed= unConfirmedCount,
                statusConfirmed= confirmedCount,
                statusComplete= completeCount
            });
        }
        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }


        public IActionResult SaveConfirmedInfo(Guid id, string processingPerson, string processingPersonPhone)
        {
            if (id == null) {
                return Json(new { Result = "Faild", Message = "系统中没找到对应的维修申请记录！" });
            }

            if (string.IsNullOrEmpty(processingPerson)){
                return Json(new { Result = "Faild", Message = "维修人员不能为空！", ErrorField = "processingPerson" });
            }

            if (string.IsNullOrEmpty(processingPersonPhone)){
                return Json(new { Result = "Faild", Message = "维修人员联系方式不能为空", ErrorField = "processingPersonPhone" });
            }

            try { 
                var dto = _service.Get(id);


                if (dto == null) {
                    return Json(new { Result = "Faild", Message = "系统中没找到对应的维修申请记录！" });
                }
                if (!(new uint[] { 0, 1 }).Contains(dto.Status)) {
                    return Json(new { Result = "Faild", Message = "维修记录的状态为！" + dto.StatusString + "不能进行确认！" });
                }
                
                dto.ProcessingPerson = processingPerson;
                dto.ProcessingPersonPhone = processingPersonPhone;
                dto.Status = 1;

                if (_service.Update(dto) ){
                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Faild" });
            }catch(Exception ex){
                return Json(new { Result = "Faild", Message = "请联系系统维护人员！" });
            } 

        }



        public IActionResult SaveCompleteInfo(Guid id,uint processingMethod, string processingResults)
        {
            if (id == null){
                return Json(new { Result = "Faild", Message = "系统中没找到对应的维修申请记录！" });
            }

            if (!(new uint[] { 1,2,3}).Contains(processingMethod)){
                return Json(new { Result = "Faild", Message = "处理方式有误！", ErrorField = "processingMethod" });
            }

            if (string.IsNullOrEmpty(processingResults)) {
                return Json(new { Result = "Faild", Message = "处理结果不能为空", ErrorField = "processingResults" });
            }

            try
            {
                var dto = _service.Get(id);
                if (dto == null)  {
                    return Json(new { Result = "Faild", Message = "系统中没找到对应的维修申请记录！" });
                }
                if (!(new uint[] { 1,2 }).Contains(dto.Status)){
                    return Json(new { Result = "Faild", Message = "维修记录的状态为！" + dto.StatusString + "不能进行完成操作！" });
                }

                dto.ProcessingMethod = processingMethod;
                dto.ProcessingResults = processingResults;
                dto.Status = 2;

                if (_service.Update(dto)){
                    return Json(new { Result = "Success" });
                }
                return Json(new { Result = "Faild" });
            }
            catch (Exception ex){
                return Json(new { Result = "Faild", Message = "请联系系统维护人员！" });
            }
        }

        public IActionResult CancelComplete(Guid id){

            if (id == null) return Json(new { Result = "Faild", Message = "系统中没找到对应的维修申请记录！" });      
            try
            {
                var dto = _service.Get(id);
                if (dto == null) return Json(new { Result = "Faild", Message = "系统中没找到对应的维修申请记录！" });
                if (dto.Status!=2) return Json(new { Result = "Faild", Message = "维修记录的状态为！" + dto.StatusString + "不能进行取消完成操作！" });
 
                dto.ProcessingMethod = 0;
                dto.ProcessingResults = "";
                dto.Status = 1;

                if (_service.Update(dto))  return Json(new { Result = "Success" });

                return Json(new { Result = "Faild" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Faild", Message = "请联系系统维护人员！" });
            }
        }

    }
}
