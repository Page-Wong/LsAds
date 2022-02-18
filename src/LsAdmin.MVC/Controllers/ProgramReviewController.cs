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
using LsAdmin.Application.ProgramReviewApp;
using LsAdmin.Application.PlaceApp;
using System.Threading.Tasks;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.ProgramReviewApp.Dtos;
using LsAdmin.Application.PlayerProgramApp;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class ProgramReviewController : LsAdminControllerBase
    {

        private readonly IProgramReviewAppService _service;
        private readonly IProgramAppService _programService;
        private readonly IPlayerProgramAppService _playerProgramService;
        private readonly IPlaceAppService _placeService;
        private readonly IMaterialAppService _materialAppService;
        public ProgramReviewController(
            IProgramReviewAppService service, 
            IProgramAppService programService, 
            IPlayerProgramAppService playerProgramService, 
            IPlaceAppService placeService, 
            IMaterialAppService materialAppService) {
            _service = service;
            _programService = programService;
            _playerProgramService = playerProgramService;
            _placeService = placeService;
            _materialAppService = materialAppService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPageList(int startPage, int pageSize) {
            var places = _placeService.GetUserAllPlaces(CurrentUser.Id);

            int rowCount = 0;
            var result = _service.LoadProgramPageListByUserId(CurrentUser.Id, startPage, pageSize, out rowCount);            
            return Json(new {
                RowCount = rowCount,
                PageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                Rows = result,
            });
        }

        [HttpGet]
        public IActionResult GetProgramAuditStatus(Guid programId) {
            var playerPrograms =_playerProgramService.GetByProgramIds(new List<Guid> { programId });
            if (playerPrograms.FirstOrDefault(it => it.Status != PlayerProgramStatus.Unpublished) != null) {
                return Json(new { Result = "Fail", Message = "无需审核" });
            }
            var programReview = _service.Get(CurrentUser.Id, programId);
            if (programReview != null) {
                return Json(new { Result = "Fail", Message = programReview.ResultName });
            }
            return Json(new { Result = "Success", Message = "待审核" });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult CustomProgramPlay(Guid programId) {
            if (programId == null) { 
                return View("ShowNone");
            }
            var program = _programService.Get(programId);
            if (program == null) {
                return View("ShowNone");
            }
            switch (program.Type) {
                case ProgramType.Web:
                    return View("ShowHtml", program.Content);
                case ProgramType.Video:
                    return View("ShowVideo", program.Id);
                default:
                    return View("ShowNone");
            }            
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task PlayAsync(Guid programId) {
            if (programId == null) return;
            var program = _programService.Get(programId);
            var materialId = program?.ProgramMaterials?.FirstOrDefault()?.MaterialId;
            if (materialId == null) {
                return;
            }
            var dto = _materialAppService.GetInfo(materialId.Value);
            var bufferSize = 256 * 1024;
            var buffer = 0;
            var file = dto.File;
            while (true) {
                if (buffer >= file.Length) {
                    break;
                }

                if (buffer + bufferSize > file.Length) {
                    bufferSize = file.Length - buffer;
                }

                await Response.Body.WriteAsync(file, buffer, bufferSize);
                buffer += bufferSize;
            }
        }

        [HttpPost]
        public IActionResult Audit(Guid programId, ProgramReviewResult result, string content) {
            var playerPrograms = _playerProgramService.GetByProgramIds(new List<Guid> { programId });
            if (playerPrograms.FirstOrDefault(it => it.Status != PlayerProgramStatus.Unpublished) != null) {
                return Json(new { Result = "Fail", Message = "该方案不需要审核！" });
            }
            if (_service.Get(CurrentUser.Id, programId)?.Id != null) {
                return Json(new { Result = "Fail", Message = "您已审核该方案！" });
            }
            var programReview = new ProgramReviewDto {
                ProgramId = programId,
                ReviewerId = CurrentUser.Id,
                Result = result,
                Content = content
            };
            _service.Insert(ref programReview);
            return Json(new { Result = "Success" });
        }
    }
}
