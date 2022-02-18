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
    public class NotifyController : LsAdminControllerBase
    {
        private readonly INotifyAppService _service;
        public NotifyController(INotifyAppService service)
        {
            _service = service;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取当前用户web平台所有系统通知信息
        /// </summary>
        /// <param name="startPage">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public IActionResult GetWebNotifyAllPageList(int startPage, int pageSize, bool unRead) {
            int rowCount = 0;
            var result = unRead ? _service.GetCurrentUserWebNotifyUnreadPageList(startPage, pageSize, out rowCount) : _service.GetCurrentUserWebNotifyAllPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result.Select(item => new NotifyViewModel {
                    Id =item.Id,
                    Remarks =item.Remarks,
                    Status =item.Status,
                    StatusString  = item.StatusString,
                    TypeDisplayName = item.Type.DisplayName,
                    Icon = item.Type.Icon,
                    SendTime = item.SendTime,
                    ReceiveTime = item.ReceiveTime,
                    Message = item.Message,
                    SenderId = item.SenderId,
                    ReceiverId = item.ReceiverId
                }),
            });
        }

        public void Read(List<Guid> notifyIds) {
            foreach (var notifyId in notifyIds) {
                var notify = _service.Get(notifyId);
                _service.CurrentUserReadWebNotify(notify);
            }
        }
    }
}
