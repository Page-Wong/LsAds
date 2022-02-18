using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Cors;
using ActiveEquipment.Application.DataModel;
using ActiveEquipment.Application.InstructionApp;
using LsAdmin.Application.InstructionApp.Dto;
using ActiveEquipment.Application.ActiveEquipmentApp;
using static ActiveEquipment.Application.DataModel.Result;
using System.Web;
using System.IO;
using System.Threading;
using System.Linq;
using LsAdmin.Application.EquipmentApp.Dtos;

namespace EquipmentService.WebAPI.Controllers {
    // For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
    [EnableCors("AllowWebServiceOrigins")]
    public class WebServiceController : Controller {

        private IInstructionAppHandler _instructionAppHandler { get; set; }
        private IActiveEquipmentAppService _activeEquipmentAppService { get; set; }
        private IInstructionMethodAppService _instructionMethodAppService { get; set; }
        public WebServiceController(IInstructionMethodAppService instructionMethodAppService, IInstructionAppHandler instructionAppHandler, IActiveEquipmentAppService activeEquipmentAppService) {
            _instructionAppHandler = instructionAppHandler;
            _activeEquipmentAppService = activeEquipmentAppService;
            _instructionMethodAppService = instructionMethodAppService;
        }

        //[HttpPost]
        public async Task<JsonResult> SendInstructionAsync(InstructionReceiverDto dto, bool isEnforce = false) {
            var result = new Result();
            try {
                var instruction = new InstructionDto {
                    EquipmentId = dto.EquipmentId,
                    MethodId = dto.MethodId,
                    NotifyUrl = dto.NotifyUrl,
                    Params = dto.Params,
                    Content = dto.Content,
                    Remarks = dto.Remarks,
                    CreateUserId = dto.CreateUserId
                };
                result = await _instructionAppHandler.SendInstructionAsync(instruction, isEnforce);
            }
            catch (Exception e) {
                result.Code = Result.ResultCode.SEND_INSTRUCTION_ERROR;
                result.Exception = e;
            }
            return Json(result);
        }

        public async Task<FileContentResult> GetScreenshotAsync(Guid id, Guid userId) {   
            var mothod = _instructionMethodAppService.GetByName("screenshot");
            var instruction = new InstructionDto {
                EquipmentId = id,
                MethodId = mothod.Id,
                NotifyUrl = "",
                CreateUserId = userId
            };
            var result = await _instructionAppHandler.SendInstructionAsync(instruction);
            if (result.IsSuccess()) {
                var i = 0;
                while (i<50) {
                    var item = _activeEquipmentAppService.Get(id);
                    if (item != null && item.LastScreenshot != null) {

                        /*var file = new FileStream($"{id}.jpg", FileMode.Create);
                        file.Write(item.LastScreenshot, 0, item.LastScreenshot.Length);*/
                        return File(item.LastScreenshot, "application/octet-stream");
                    }
                    Thread.Sleep(100);
                    i++;
                }
            }
            return null;
        }

        [HttpGet]
        public IActionResult GetActiveEquipment(Guid id) {
            var dto = _activeEquipmentAppService.Get(id);
            //防止socket信息外露
            if (dto != null) {      
                var item = new {
                    EquipmentId = dto.EquipmentId,
                    DeviceId = dto.DeviceId,
                    Token = dto.Token,
                    OnlineTime = dto.OnlineTime,
                    LastConnectTime = dto.LastConnectTime,
                    Ip = dto.Ip.ToString(),
                    Port = dto.Port,
                    EquipmentInfo = dto.EquipmentInfo,
                    PlayInfo = dto.PlayInfo,
                    NetworkInfo = dto.NetworkInfo,
                    RealtimeInfo = dto.RealtimeInfo,
                    AppVersion = dto.AppVersion,
                };

                return Ok(item);
            }
            return Ok(dto);
        }

        public JsonResult GetActiveEquipmentInfo(Guid id) {
            var item = _activeEquipmentAppService.Get(id);
            if (item != null) {
                return Json(new {
                    Code = ResultCode.SUCCESS,
                    Data = new {
                        EquipmentId = item.EquipmentId,
                        DeviceId = item.DeviceId,
                        Token = item.Token,
                        OnlineTime = item.OnlineTime,
                        LastConnectTime = item.LastConnectTime,
                        Ip = item.Ip.ToString(),
                        Port = item.Port
                    }
                });
            }
            return Json(new {
                Code = ResultCode.ACTIVE_EQUIPMENT_NONE
            });
        }

        public JsonResult GetActiveEquipmentList() {
            var items = _activeEquipmentAppService.GetAllList();
            return Json(new {
                Code = ResultCode.SUCCESS,
                Data = items.Select(item => new {
                    EquipmentId = item.EquipmentId,
                    DeviceId = item.DeviceId,
                    Token = item.Token,
                    OnlineTime = item.OnlineTime,
                    LastConnectTime = item.LastConnectTime,
                    Ip = item.Ip.ToString(),
                    Port = item.Port
                }).ToList()
            });
        }
        
    }

}