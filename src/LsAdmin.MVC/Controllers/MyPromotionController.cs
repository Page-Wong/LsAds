using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LsAdmin.Application.PlayerApp;
using LsAdmin.Application.PlayerProgramApp;
using LsAdmin.Application.ProgramApp;
using System.Collections.Generic;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Domain.Entities;

namespace LsAdmin.MVC.Controllers
{
    [AllowAnonymous]
    public class MyPromotionController : LsAdminControllerBase
    {
        private readonly IPlayerAppService _playerService;
        private readonly IPlayerProgramAppService _playerProgramService;
        private readonly IOrderAppService _orderService;
        private readonly IProgramAppService _programService;


        public MyPromotionController(IPlayerAppService playerService, IPlayerProgramAppService playerProgramService, IProgramAppService programService, IOrderAppService orderService)
        {
            _playerService = playerService;
            _playerProgramService = playerProgramService;
            _programService = programService;
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetPageList(int startPage, int pageSize)
        {
            int rowCount = 0;
            var myplayers = _playerService.GetCanSetByOwnerUserId(CurrentUser.Id, startPage, pageSize, out rowCount);
            _playerService.GetPlayersAllInfo(ref myplayers);

            return Json(new{
                rowsCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = myplayers,    
            });
        }


        public IActionResult GetProgramList(Guid playerid)
        {
            var orderPlayerPrograms = _playerService.GetAllOrderPlayerProgramsByPlayerId(playerid).Where(w => (new PlayerProgramStatus[] { PlayerProgramStatus.Pause, PlayerProgramStatus.Playing, PlayerProgramStatus.Ready, PlayerProgramStatus.Unpublished }).Contains(w.PlayerProgram.Status));
           return Json(new { orderPlayerPrograms = orderPlayerPrograms, });
        }

        public IActionResult EditProgram(string selectedPlayerids)
        {
            // var  asd = selectedPlayerids.Split(",");
            // ViewData["selectedPlayerids"] = selectedPlayerids.Split(",");
            ViewBag.selectedPlayerids =selectedPlayerids;
            return View();
        }

        [HttpGet]
        public IActionResult GetMyPlayers()
        {
            var players = _playerService.GetByOwnerUserId(CurrentUser.Id);
            _playerService.GetPlayersAllInfo(ref players);
            return Json(new { players = players, });
        }

        /// <summary>
        /// 保存我的推广方案
        /// </summary>
        /// <param name="ProgramId">节目id</param>
        /// <param name="OrderName">方案名称</param>
        /// <param name="StartDate">播放开始日期</param>
        /// <param name="EndDate">播放结束日期</param>
        /// <param name="playerids">播放器列表</param>
        /// <param name="OrderRemarks">备注</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveMyPromotion(string ProgramId, string OrderName , DateTime StartDate, DateTime EndDate,  List<string> playerids, string OrderRemarks="")
        {

            #region 数据有效性校验
            if(string.IsNullOrEmpty(ProgramId) || !Guid.TryParse(ProgramId, out Guid ProgramIdGuid) )
            {
                return Json(new
                {
                    ErrorField = "ProgramId",
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }

            var program=_programService.Get(ProgramIdGuid);

            if(program==null || program.CreateUserId != CurrentUser.Id)
            {
                return Json(new
                {
                    ErrorField = "ProgramId",
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }

            if (string.IsNullOrEmpty(OrderName)){
                return Json(new {
                    ErrorField = "OrderName",
                    Message = "方案名称不能为空!",
                    Result = "Faild"
                });
            }

            if (StartDate==null || StartDate.Date<DateTime.Now.Date){
                return Json(new{
                    message = "'播放开始时间'信息有误!",
                    Result = "False"
                });
            }

            if (EndDate == null || EndDate.Date < DateTime.Now.Date){
                return Json(new
                {
                    ErrorField = "StartDate",
                    message = "'播放结束时间'信息有误!",
                    Result = "False"
                });
            }


            if (StartDate > EndDate){
                return Json(new{
                    ErrorField = "EndDate",
                    message = "'播放结束时间'必须大于'播放结束时间'!",
                    Result = "False"
                });
            }

            if(playerids==null || playerids.Count == 0)
            {
                return Json(new
                {
                    ErrorField = "playerids",
                    message = "播放器列表信息有误！",
                    Result = "False"
                });
            }

            List<Guid> playerIdGuids = new List<Guid>();

            var myPlayers= _playerService.GetByOwnerUserId(CurrentUser.Id);

            foreach (var playerid in playerids)
            {
                if (string.IsNullOrEmpty(playerid) || !Guid.TryParse(playerid, out Guid playerIdGuid))
                {
                    return Json(new
                    {
                        ErrorField = "playerids",
                        message = "播放器列表信息有误！",
                        Result = "False"
                    });
                }

                if (myPlayers.FirstOrDefault(s => s.Id== playerIdGuid)==null )
                {
                    return Json(new
                    {
                        ErrorField = "playerids",
                        message = "播放器列表信息有误！",
                        Result = "False"
                    });
                }

                playerIdGuids.Add(playerIdGuid);
            }
            #endregion

            OrderDto order = new OrderDto
            {
                Name = OrderName,
                StartDate = StartDate,
                EndDate = EndDate,
                Remarks = OrderRemarks,
                OrderNo = new Guid().ToString(),
                Status = OrderDto.ORDER_STATUS_AUDITING,
                Type = OrderDto.ORDER_TYPE_2,  
            };

             // 保存订单信息
            if (_orderService.SaveAll(ref order, ProgramIdGuid, playerIdGuids) == false){
                return Json(new
                {
                    ErrorField = "OrderName",
                    message = "数据保存失败！",
                    Result = "False"
                });
            }

            return Json(new { Result = "Success" });
        }


        /// <summary>
        /// 将播放器节目状态变为播放中
        /// </summary>
        /// <param name="playerProgramid">播放器节目ID</param>
        /// <returns>成功返回Success 否则 Faild 并返回错误信息 </returns>
        [HttpPost]
        public IActionResult SetplayerprogramToPlaying(string playerProgramid)
        {
            #region 数据有效性校验
            if (string.IsNullOrEmpty(playerProgramid) || !Guid.TryParse(playerProgramid, out Guid playerProgramidGuid))
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }
            #endregion 数据有效性校验

            if ( _playerProgramService.UpdateStatus(playerProgramidGuid, PlayerProgramStatus.Playing, out string  errorMessage) == false)
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });

            }

            return Json(new { Result = "Success" });
        }


        /// <summary>
        /// 将播放器节目状态变更为暂停播放
        /// </summary>
        /// <param name="playerProgramid">播放器节目ID</param>
        /// <returns>成功返回Success 否则 Faild 并返回错误信息 </returns>
        [HttpPost]
        public IActionResult SetplayerprogramToPause(string playerProgramid)
        {
            #region 数据有效性校验
            if (string.IsNullOrEmpty(playerProgramid) || !Guid.TryParse(playerProgramid, out Guid playerProgramidGuid))
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }
            #endregion 数据有效性校验

            if (_playerProgramService.UpdateStatus(playerProgramidGuid, PlayerProgramStatus.Pause, out string errorMessage) == false)
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });

            }

            return Json(new { Result = "Success" });
        }


        /// <summary>
        /// 将播放器节目状态变更为取消播放
        /// </summary>
        /// <param name="playerProgramid">播放器节目ID</param>
        /// <returns>成功返回Success 否则 Faild 并返回错误信息 </returns>
        [HttpPost]
        public IActionResult SetplayerprogramToCancel(string playerProgramid)
        {
            #region 数据有效性校验
            if (string.IsNullOrEmpty(playerProgramid) || !Guid.TryParse(playerProgramid, out Guid playerProgramidGuid))
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }
            #endregion 数据有效性校验

            if (_playerProgramService.UpdateStatus(playerProgramidGuid, PlayerProgramStatus.Cancel, out string errorMessage) == false)
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });

            }

            return Json(new { Result = "Success" });
        }


        /// <summary>
        /// 将播放器节目状态变更为准备播放
        /// </summary>
        /// <param name="playerProgramid">播放器节目ID</param>
        /// <returns>成功返回Success 否则 Faild 并返回错误信息 </returns>
        [HttpPost]
        public IActionResult SetplayerprogramToReady(string playerProgramid)
        {
            #region 数据有效性校验
            if (string.IsNullOrEmpty(playerProgramid) || !Guid.TryParse(playerProgramid, out Guid playerProgramidGuid))
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }
            #endregion 数据有效性校验

            if (_playerProgramService.UpdateStatus(playerProgramidGuid, PlayerProgramStatus.Ready, out string errorMessage) == false)
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });

            }

            return Json(new { Result = "Success" });
        }


        /// <summary>
        /// 将播放器节目状态变更为完成播放
        /// </summary>
        /// <param name="playerProgramid">播放器节目ID</param>
        /// <returns>成功返回Success 否则 Faild 并返回错误信息 </returns>
        [HttpPost]
        public IActionResult SetplayerprogramToComplete(string playerProgramid)
        {
            #region 数据有效性校验
            if (string.IsNullOrEmpty(playerProgramid) || !Guid.TryParse(playerProgramid, out Guid playerProgramidGuid))
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });
            }
            #endregion 数据有效性校验

            if (_playerProgramService.UpdateStatus(playerProgramidGuid, PlayerProgramStatus.Complete, out string errorMessage) == false)
            {
                return Json(new
                {
                    Message = "节目信息有误！",
                    Result = "Faild"
                });

            }

            return Json(new { Result = "Success" });
        }

    

        
    }
}
