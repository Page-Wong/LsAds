using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.DepartmentApp;
using LsAdmin.MVC.Models;
using LsAdmin.Application.DepartmentApp.Dtos;
using LsAdmin.Application.NotifyApp;
using LsAdmin.Application.NotifyApp.Dtos;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class AdminNotifyController : LsAdminControllerBase
    {
        private readonly INotifyAppService _service;
        public AdminNotifyController(INotifyAppService service)
        {
            _service = service;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取所有系统通知信息
        /// </summary>
        /// <param name="startPage">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public IActionResult GetAllPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _service.GetAllPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result.Select(item => new NotifyViewModel {
                    Id = item.Id,
                    Remarks = item.Remarks,
                    Status = item.Status,
                    StatusString = item.StatusString,
                    TypeDisplayName = item.Type.DisplayName,
                    Icon = item.Type.Icon,
                    SendTime = item.SendTime,
                    ReceiveTime = item.ReceiveTime,
                    Message = item.Message,
                    SenderId = item.SenderId,
                    ReceiverId = item.ReceiverId
                })
            });
        }

        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult Edit(NotifyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (_service.InsertOrUpdate(ref dto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

        public IActionResult DeleteMuti(string ids)
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
        public IActionResult Delete(Guid id)
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
    }
}
