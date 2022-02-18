using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ActiveEquipment.Application.ActiveEquipmentApp;
using ActiveEquipment.Application.DataModel;
using ActiveEquipment.Application.InstructionApp;
using LsAdmin.Application.InstructionApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EquipmentService.WebAPI.Controllers
{
    public class HomeController : Controller {
        IActiveEquipmentAppHandler _activeEquipmentAppHandler;
        IActiveEquipmentAppService _activeEquipmentAppService;
        IInstructionAppHandler _instructionAppHandler;
        public HomeController(IActiveEquipmentAppHandler activeEquipmentAppHandler, IInstructionAppHandler instructionAppHandler, IActiveEquipmentAppService activeEquipmentAppService) {
            _activeEquipmentAppHandler = activeEquipmentAppHandler;
            _instructionAppHandler = instructionAppHandler;
            _activeEquipmentAppService = activeEquipmentAppService;
        }
        public IActionResult Index()
        {
            var d = _activeEquipmentAppService.GetAllList();
            return View(d);
        }

        public IActionResult TestInstructionDefaultNotify(InstructionResult dto) {

            var url = "https://localhost:44321/Instruction/InstructionDefaultNotify";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var param = new NameValueCollection();
            foreach (var m in dto.GetType().GetProperties()) {
                var n = m.Name;
                var v = m.GetValue(dto);
                param.Add(n, v?.ToString());
            }

            byte[] responseData = webClient.UploadValues(url, "POST", param);
            Console.WriteLine(responseData.ToString());

            return Json(new { Result = "Success" });
        }

        public IActionResult TestSendInstructionAsync(InstructionReceiverDto dto, bool isEnforce = false) {

            var url = "https://localhost:44321/WebService/SendInstructionAsync";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var param = new NameValueCollection();
            foreach (var m in dto.GetType().GetProperties()) {
                var n = m.Name;
                var v = m.GetValue(dto);
                param.Add(n,v?.ToString());
            }
            
            byte[] responseData = webClient.UploadValues(url, "POST", param);
            Console.WriteLine(responseData.ToString());

            return Json(new { Result = "Success" });
        }
    }
}
