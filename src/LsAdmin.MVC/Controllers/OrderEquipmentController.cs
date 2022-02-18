using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Web;
using System.Collections.Generic;
using LsAdmin.MVC.Models;
using System.Text;
using System;
using LsAdmin.MVC.Controllers;
using LsAdmin.Application.MenuApp;
using LsAdmin.Application.MenuApp.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections;
using LsAdmin.Application.PlayHistoryApp;
using LsAdmin.Application.PlayHistoryApp.Dtos;
using Newtonsoft.Json;
using LsAdmin.Application.OrderEquipmentApp;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.OrderPlaceApp.Dtos;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderPlaceApp;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using LsAdmin.Application.OrderMaterialApp;
using Microsoft.AspNetCore.Authorization;
using LsAdmin.Application.MaterialApp;
using LsAdmin.Application.MaterialApp.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    [AllowAnonymous]
    public class OrderEquipmentController : LsAdminControllerBase
    {
        private readonly IOrderEquipmentAppService _OrderEquipmentAppService; 
        private readonly IEquipmentAppService _EquipmentAppService;
        private readonly IPlaceAppService _PlaceAppService;
        private readonly IOrderAppService _OrderAppService;
        private readonly IOrderPlaceAppService _OrderPlaceAppService;
        private readonly IOrderMaterialAppService _OrderMaterialAppService;
        private readonly IMaterialAppService _MaterialAppService;
        private List<PlayListResultModel> playListResults;

        public OrderEquipmentController(IOrderEquipmentAppService service, IOrderAppService _OrderAppService, 
            IMaterialAppService _MaterialAppService,IOrderPlaceAppService _OrderPlaceAppService, IEquipmentAppService _EquipmentAppService,  
            IPlaceAppService _PlaceAppService, IOrderMaterialAppService _OrderMaterialAppService)
        {
            _OrderEquipmentAppService = service;
            this._EquipmentAppService = _EquipmentAppService;
            this._PlaceAppService = _PlaceAppService;
            this._OrderPlaceAppService = _OrderPlaceAppService;
            this._MaterialAppService = _MaterialAppService;
            this._OrderAppService = _OrderAppService;
            this._OrderMaterialAppService = _OrderMaterialAppService;

            playListResults = new List<PlayListResultModel>();
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddOrEdit()
        {
            return View();
        }


 

        

        public IActionResult GetAllPageList(int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _OrderEquipmentAppService.GetAllPageList(startPage, pageSize, out rowCount);
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
                _OrderEquipmentAppService.DeleteBatch(delIds);
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
                _OrderEquipmentAppService.Delete(id);
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
            var dto = _OrderEquipmentAppService.Get(id);
            return Json(dto);
        }

       public int GetPlayType(MaterialDto material)
        {
            //图片类型 BMP、JPG、JPEG、PNG、GIF
            return 0;
        }


    }

}
