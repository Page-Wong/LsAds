using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.EquipmentManagementApp;
using LsAdmin.MVC.Models;
using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Application.FilesApp;
using LsAdmin.Application.CollectionsBlacklistsApp.Dtos;
using LsAdmin.Application.InstructionApp.Dto;
using System.Net;
using System.Collections.Specialized;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using LsAdmin.Utility.Json;
using LsAdmin.Application.PlayerApp;
using LsAdmin.Utility.Convert;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class EquipmentController : LsAdminControllerBase
    {
        private readonly IEquipmentAppService _service;
        private readonly IFilesAppService _filesService;
        private readonly IInstructionMethodAppService _instructionMethodService;
        private readonly ICollectionsBlacklistsAppService _collectionsblacklistsService;
        private readonly IOptions<WebAPIAddressModel> _webAPIAddressModel;
        private readonly IPlayerAppService _playerService;

        public EquipmentController(IOptions<WebAPIAddressModel> webAPIAddressModel, IInstructionMethodAppService instructionMethodService, IEquipmentAppService service, IFilesAppService filesService,ICollectionsBlacklistsAppService collectionsblacklistsService,IPlayerAppService playerService)
        {
            _service = service;
            _filesService = filesService;
            _instructionMethodService = instructionMethodService;
            _collectionsblacklistsService = collectionsblacklistsService;
            _webAPIAddressModel = webAPIAddressModel;
            _playerService = playerService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetEquipment(int startPage,int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetEquipmentsExceptBlack(startPage, pageSize, out rowCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        public IActionResult GetCategoryEquipments(string category,int startPage,int pageSize)
        {
            int rowCount = 0;
            var result = new List<EquipmentDto>();
            if (category == "all")
            {
                result = _service.GetEquipmentsExceptBlack(startPage, pageSize, out rowCount);
            }
            else if (category == "collection")
            {
                result = _service.GetCollectionsPageList(startPage, pageSize, out rowCount);
            }
            else if (category == "blacklist")
            {
                result = _service.GetBlacklistsPageList(startPage, pageSize, out rowCount);
            }
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                category=category,
            });
        }

        public IActionResult GetEquipmentByQuery(string category,Guid placetype, string province, string city, string district, string price,ushort favorite, int startPage,int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetEquipmentByQuery(placetype,province,city,district,price,favorite,startPage, pageSize,out rowCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                category=category,
            });
        }

        public IActionResult GetEquipmentPageList(int startPage, int pageSize, uint status)
        {
            int rowCount = 0;         
        
            var result = _service.GetEquipmentByUserPageList( startPage, pageSize,  status, out  rowCount, out int unInuseCount, out int inuseCount, out int repairingCount, out int scrapCount);
            
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                statusUnInuse = unInuseCount,
                statusInuse = inuseCount,
                statusRepairing = repairingCount,
                statusScrap= scrapCount

            });
        }

        public IActionResult GetEquipmentByPlace(Guid placeId, int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetEquipmentByPlace(placeId, startPage, pageSize, out rowCount);
           
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                
            });
        }

        public IActionResult GetImagePageList(Guid equipmentId,Guid placeId,int startPage,int pageSize)
        {
            int rowCount = 0;
            List<FilesDto> result;
            result = _filesService.GetPageListByEquipment(equipmentId, placeId, startPage, pageSize,out rowCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        /*获取收藏列表*/
        public IActionResult GetCollectionsPageList(int startPage,int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetCollectionsPageList(startPage, pageSize, out rowCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        /*获取黑名单列表*/
        public IActionResult GetBlacklistsPageList(int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetBlacklistsPageList(startPage, pageSize, out rowCount);
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        /*获取该设备的所有播放器*/
        public IActionResult GetPlayersByEquipmentId(Guid equipmentId)
        {
            var players = _playerService.GetByEquipmentId(equipmentId);
            return Json(new
            {
                rows = players,
            });
        }


        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }

        public IActionResult Edit(EquipmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var odto = _service.Get(dto.Id);
            
            odto.Remarks = dto.Remarks;

            if (odto.Remarks != null)
            {
                odto.Status=2;
                //odto.Status = "维修中，请耐心等待！";
            }
            else
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "维修原因不能为空！"
                });
            }
                            
            if (_service.InsertOrUpdate(ref odto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

        /*获取指令列表列表*/
        public IActionResult GetInstructionList(Guid id) {            
            var rows = _instructionMethodService.GetAllList();
            return Json(new {
                result = "Success",
                rows = rows
            });
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="dto">指令接收器</param>
        /// <param name="isEnforce">是否强制发送</param>
        /// <returns>执行结果</returns>
        public IActionResult SendInstruction(InstructionReceiverDto dto, bool isEnforce = false) {
            dto.CreateUserId = CurrentUser.Id;
            var url = $"http://{_webAPIAddressModel.Value.Ip}:{_webAPIAddressModel.Value.Port}/WebService/SendInstructionAsync";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var param = new NameValueCollection();
            foreach (var m in dto.GetType().GetProperties()) {
                var n = m.Name;
                var v = m.GetValue(dto);
                var tmp = v?.ToString();
                param.Add(n, v?.ToString());
            }

            byte[] responseData = webClient.UploadValues(url, "POST", param);
            var resultStr = System.Text.Encoding.Default.GetString(responseData);
            return Json(JsonConvert.DeserializeObject<Models.ActionResult>(resultStr, new LsJsonSerializerSettings()));
        }

        public IActionResult Screen() {
            return View();
        }

        public IActionResult GetScreen(Guid id) {
            var url = $"http://{_webAPIAddressModel.Value.Ip}:{_webAPIAddressModel.Value.Port}/WebService/GetScreenshotAsync?id={id.ToString()}";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/octet-stream");

            try {
                var responseData = webClient.OpenRead(url);
                return File(responseData, @"image/png");
            }
            catch (Exception e) {
                return null;
            }
        }

        public IActionResult GetActiveEquipment(Guid id) {
            var url = $"http://{_webAPIAddressModel.Value.Ip}:{_webAPIAddressModel.Value.Port}/WebService/GetActiveEquipment?id={id.ToString()}";
            WebClient webClient = new WebClient();
            //webClient.Headers.Add("Content-Type", "application/json");
            byte[] responseData = webClient.DownloadData(url);
            var item = ByteConvertHelper.Bytes2Object<ActiveEquipmentDto>(responseData);
            return Json(item);
        }

        public IActionResult GetActiveEquipmentInfo(Guid id) {
            var url = $"http://{_webAPIAddressModel.Value.Ip}:{_webAPIAddressModel.Value.Port}/WebService/GetActiveEquipmentInfo?id={id.ToString()}";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            
            byte[] responseData = webClient.DownloadData(url);
            var resultStr = System.Text.Encoding.Default.GetString(responseData);
            return Json(resultStr);
        }

        public IActionResult GetActiveEquipmentList() {
            var url = $"http://{_webAPIAddressModel.Value.Ip}:{_webAPIAddressModel.Value.Port}/WebService/GetActiveEquipmentList";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            byte[] responseData = webClient.DownloadData(url);
            var resultStr = System.Text.Encoding.Default.GetString(responseData);
            return Json(resultStr);
        }
    }
    public class EquipmentManagementController : LsAdminControllerBase
    {
        private readonly IEquipmentManagementAppService _service;
        public EquipmentManagementController(IEquipmentManagementAppService service)
        {
            _service = service;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
