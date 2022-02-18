using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistEquipment.Application.RegistEquipmentApp;
using LsAdmin.Application.EquipmentModelApp;
using LsAdmin.Application.EquipmentApp;
using Microsoft.AspNetCore.Authorization;
using LsAdmin.Application.RegistEquipmentApp.Dto;

namespace EquipmentService.WebAPI.Controllers
{
    [Route("[controller]/[action]")]  
    public class RegistController : Controller
    {
        private readonly IRegistEquipmentAppService _registEquipmentAppService;
        private readonly IRegistEquipmentAppHandler _registEquipmentAppHandler;
        private readonly IEquipmentModelAppService _equipmentModelService;
        private readonly IEquipmentAppService _equipmentService;
        public RegistController(IRegistEquipmentAppService registEquipmentAppService, IRegistEquipmentAppHandler registEquipmentAppHandler,
                                       IEquipmentModelAppService equipmentModelService, IEquipmentAppService equipmentService)
        {
            _registEquipmentAppService = registEquipmentAppService;
            _registEquipmentAppHandler = registEquipmentAppHandler;
            _equipmentModelService = equipmentModelService;
            _equipmentService = equipmentService;
        }

        [HttpPost]
        public IActionResult SendRegisterSucceedAsync(Guid id) {
            _registEquipmentAppHandler.SendRegisterSucceedAsync(id);
            return Ok(new { Result = "Success" });
        }

        [HttpPost]
        public IActionResult PostRegistEquipmentState(Guid id, ushort status) {
            return Ok(_registEquipmentAppService.UpdateStatusById(id, status));
        }

        [HttpGet]
        public IActionResult GetRegistEquipment(Guid id) {
            var dto = _registEquipmentAppService.Get(id);
            //防止socket信息外露
            if (dto!=null) {
                var item = new RegistEquipmentDto() {
                    Id= dto.Id,
                    DeviceId = dto.DeviceId,
                    ApplyTime = dto.ApplyTime,
                    Status = dto.Status,
                    AeadMinutes = dto.AeadMinutes,
                    Token = dto.Token
                };

                return Ok(item);
            }
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult ReceiveRegisterSucceed(string id)
        {
            var dto = _registEquipmentAppService.GetByDeviceId(id).FirstOrDefault();
            if (dto != null && dto.WebSocket != null) {
                _registEquipmentAppHandler.ReceiveRegisterSucceedAsync(dto.WebSocket);
                return Json(new { Result = "Success" });
            }
            else
            {
                return Json(new { Result = "Faild" });
            }
        }



    }
}