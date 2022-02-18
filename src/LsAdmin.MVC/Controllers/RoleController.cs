using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.RoleApp.Dtos;
using LsAdmin.Application.RoleApp;
using LsAdmin.MVC.Models;
using LsAdmin.Application.RoleApplyApp;
using LsAdmin.Application.RoleApplyApp.Dtos;
using LsAdmin.Application.UserApp;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class RoleController : LsAdminControllerBase {
        private readonly IRoleAppService _service;
        private readonly IRoleApplyAppService _roleApplyService;
        private readonly IUserAppService _userService;
        public RoleController(IRoleAppService service, IRoleApplyAppService roleApplyService, IUserAppService userService) {
            _service = service;
            _roleApplyService = roleApplyService;
            _userService = userService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult Edit(RoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (dto.Id == Guid.Empty)
                dto.CreateTime = DateTime.Now;
            //dto.CreateUserId = 
            if (_service.InsertOrUpdate(ref dto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

        public IActionResult GetAllPageList(int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetAllPageList(startPage, pageSize, out rowCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
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

        /// <summary>
        /// 根据角色获取权限
        /// </summary>
        /// <returns></returns>
        public IActionResult GetMenusByRole(Guid roleId)
        {
            var dtos = _service.GetAllMenuListByRole(roleId);
            return Json(dtos);
        }

        public IActionResult SavePermission(Guid roleId, List<RoleMenuDto> roleMenus)
        {
            if (_service.UpdateRoleMenu(roleId, roleMenus))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

        #region 角色申请审核
        public IActionResult RoleApplyAudit() {
            return View();
        }

        public IActionResult GetRoleApplyAllPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _roleApplyService.GetAllPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        [HttpPost]
        public IActionResult RoleApplyAudit(Guid roleApplyId, int audit, string comment) {
            var dto = _roleApplyService.Get(roleApplyId);
            if (dto == null) {
                return Json(
                new { Result = "Faild", Message = "系统不存在此订单" });

            }
            switch (audit) {
                case 0:
                    dto.Status = RoleApplyDto.STATUS_UNPASS;
                    break;
                case 1:
                    dto.Status = RoleApplyDto.STATUS_PASS;
                    break;
                default:
                    return Json(
                            new { Result = "Faild", Message = "审核结果无效" });
            }
            if (_roleApplyService.Update(dto)) {
                if (dto.Status == RoleApplyDto.STATUS_PASS) {
                    _userService.AddRole(dto.ApplyUserId, dto.RoleId);
                    Notify.SendWebNotifyToUser(dto.ApplyUserId, "System_Notify", new string[] { "你已成功开通 " + _service.Get(dto.RoleId).Name + " 功能", "#" });
                }
                else if (dto.Status == RoleApplyDto.STATUS_UNPASS) {
                    Notify.SendWebNotifyToUser(dto.ApplyUserId, "System_Notify", new string[] { "你申请的 " + _service.Get(dto.RoleId).Name + " 功能开通失败，请联系客服", "#" });
                }
                return Json(
                    new {
                        Result = "Success"
                    });
            }
            else {
                return Json(
                new { Result = "Faild", Message = "数据保存失败！" });
            }
        }
        #endregion
    }
}
