using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.EquipmentManagementApp;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.EquipmentModelApp;

namespace LsAdmin.MVC.Controllers
{
    public class EquipmentOwnerController : LsAdminControllerBase
    {
        private readonly IEquipmentAppService _service;
        private readonly IPlaceAppService _placeService;
        private readonly IEquipmentModelAppService _equipmentModelService;

        public EquipmentOwnerController(IEquipmentAppService service, IPlaceAppService placeService,IEquipmentModelAppService equipmentModelService)
        {
            _service = service;
            _placeService = placeService;
            _equipmentModelService = equipmentModelService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOwnerEquipmentsWithStatusRowCount(int startPage, int pageSize)
        {
            int rowCount = 0;
            Dictionary<uint, int> statusRowCount = new Dictionary<uint, int>();
            var result = _service.GetGetOwnerEquipmentsWithStatusRowCount(startPage, pageSize, out rowCount, out statusRowCount);
            int allrowCount = 0;
            foreach (var count in statusRowCount){
                allrowCount += count.Value;
            }
        
            return Json(new{
                rowCount = rowCount,
                allrowCount = allrowCount,
                statusRowCount = statusRowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                
                rows = result,
            });
        }

        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            var placeDtoList = _placeService.GetAllList().ToList();
            var equipmentModeList = _equipmentModelService.GetAllList().ToList();

            return Json(new
            {
                placelist = placeDtoList,
                equipmentModeList=equipmentModeList,
                dto = dto,                
            });
        }


        public IActionResult GetOwnerEquipmentsByStatus(uint status, int startPage, int pageSize)
        {
            int rowCount = 0;

            Dictionary<uint, int> statusRowCount = new Dictionary<uint, int>();
            var result = _service.GetOwnerEquipmentsByStatus(status, startPage, pageSize, out rowCount, out statusRowCount);

            int allrowCount = 0;
            foreach (var count in statusRowCount)
            {
                allrowCount += count.Value;
            }

            return Json(new
            {
                rowCount = rowCount,
                allrowCount = allrowCount,
                statusRowCount = statusRowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });

        }


        /// <summary>
        /// 保存设备的场所信息
        /// </summary>
        /// <param name="equipmentId">设备编码</param>
        /// <param name="placeId">产所编码</param>
        /// <returns></returns>
        public IActionResult SavePlace(Guid equipmentId,string placeId){
            try{

                if (equipmentId == null){
                    return Json(new{
                        Result = "Faild",
                        Message = "你提交的信息有误，请尝试重新操作！",
                    });
                }

                var equipment = _service.Get(equipmentId);
                if (equipment == null){
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你提交的信息有误，请尝试重新操作！",
                    });
                }

                if (string.IsNullOrEmpty(placeId)|| placeId== "undefined")
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "场所信息有误！",
                    });
                }

                equipment.PlaceId = Guid.Parse(placeId);
                equipment.Status = 1;
                if (equipment.StartDate == null)
                    equipment.StartDate = DateTime.Now;

                if (_service.Update(equipment))
                {
                    return Json(new { Result = "Success" });
                }
                return Json(new
                {
                    Result = "Faild",
                    Message = "数据更新失败！",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.ToString() + GetModelStateError()
                });
            }
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="equipmentId">设备编码</param>
        /// <param name="Status">新状态</param>
        /// <returns></returns>
        public IActionResult UpdateEquipmentStats(Guid equipmentId , uint Status)
        {
            try
            {
                if (equipmentId == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你提交的信息有误，请尝试重新操作！",
                    });
                }

                var equipment = _service.Get(equipmentId);
                if (equipment == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你提交的信息有误，请尝试重新操作！",
                    });
                }


                if(!(new uint[] { 0,1,2,3}).Contains(Status) )
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你提交的设备状态信息有误！",
                    });
                }

                switch(Status)
                {
                    case 0:
                        equipment.Status = Status;
                        equipment.PlaceId = null;
                        break;
                    case 3:
                        equipment.Status = Status;
                        equipment.PlaceId = null;
                        equipment.DiscontinuationDate = DateTime.Now;
                        break;
     
                    default:                
                        break;
                }

                if (_service.Update(equipment))
                {
                    return Json(new { Result = "Success" });
                }
                return Json(new
                {
                    Result = "Faild",
                    Message = "数据更新失败！",
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.ToString() + GetModelStateError()
                });
            }


        }

        /// <summary>
        /// 更新设备型号和名称信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="equipmentModelId"></param>
        /// <returns></returns>
        public IActionResult SaveEquipmentInfo(Guid equipmentId, string name,string equipmentModelId)
        {
            try
            {

                if (equipmentId == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你提交的信息有误，请尝试重新操作！",
                    });
                }

                var equipment = _service.Get(equipmentId);
                if (equipment == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你提交的信息有误，请尝试重新操作！",
                    });
                }

                if (string.IsNullOrEmpty(name) || name == "undefined")
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "设备名称信息有误！",
                    });
                }

                if (string.IsNullOrEmpty(equipmentModelId) || equipmentModelId == "undefined")
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "设备型号信息有误！",
                    });
                }
   
                equipment.EquipmentModelId = Guid.Parse(equipmentModelId);
                equipment.Name = name;
               
                if (_service.Update(equipment))
                {
                    return Json(new { Result = "Success" });
                }
                return Json(new
                {
                    Result = "Faild",
                    Message = "数据更新失败！",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.ToString() + GetModelStateError()
                });
            }
        }

    }
}
