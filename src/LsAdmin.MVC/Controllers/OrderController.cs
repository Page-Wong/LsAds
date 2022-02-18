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
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.LabelApp;
using LsAdmin.Domain.Entities;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.LabelApp.Dtos;
using LsAdmin.Application.MaterialApp;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using LsAdmin.Application.OrderMaterialApp;
using LsAdmin.Application.OrderTimeApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.OrderTimeApp.Dtos;
using LsAdmin.Application.PlayPriceApp;
using LsAdmin.Application.PlayPriceApp.Dtos;
using LsAdmin.Application.OrderEquipmentApp;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.AdministrativeAreaApp;
using LsAdmin.Utility.Service;
using LsAdmin.Application.ShoppingCartApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.MaterialApp.Dtos;
using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.PlayerApp;
using LsAdmin.Application.PlayerApp.Dtos;

namespace LsAdmin.MVC.Mediea.Controllers {
    public class OrderController : LsAdminControllerBase {

        private readonly IOrderAppService _service;
        private readonly ILabelAppService _labelservice;
        private readonly IMaterialAppService _materialservice;
        private readonly IOrderMaterialAppService _ordermaterialservice;
        private readonly IOrderTimeAppService _ordertimeservice;
        private readonly IPlaceAppService _placeservice;
        private readonly IPlayPriceAppService _playPriceService;
        private readonly IOrderEquipmentAppService _orderEquipmentService;
        private readonly IEquipmentAppService _equipmentService;
        private readonly IShoppingCartAppService _shoppingcartService;
        private readonly IPlayerAppService _playerService;


        public OrderController(IOrderAppService service,ILabelAppService labelservice,IMaterialAppService materialservice,
            IOrderMaterialAppService ordermaterialservice,IOrderTimeAppService ordertimeservice,IPlaceAppService placeservice,
            IPlayPriceAppService playPriceService,IOrderEquipmentAppService orderEquipmentService,
            IEquipmentAppService equipmentService,IShoppingCartAppService shoppingcartService,IPlayerAppService playerService) {
            _service = service;
            _labelservice = labelservice;
            _materialservice = materialservice;
            _ordermaterialservice = ordermaterialservice;
            _ordertimeservice = ordertimeservice;
            _placeservice = placeservice;
            _playPriceService = playPriceService;
            _orderEquipmentService = orderEquipmentService;
            _equipmentService = equipmentService;
            _shoppingcartService = shoppingcartService;
            _playerService = playerService;
        }

        // GET: /<controller>/
        public IActionResult Index() {
            if (CurrentUser.AuthStatus == 0) {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1) {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        // GET: /<controller>/
        public IActionResult Edit()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        //GET:/<controller>/
        public IActionResult IndoorOrder()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        public IActionResult Order()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }


        public IActionResult Equipments()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        public IActionResult List() {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        public IActionResult ShowEquipments()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        public IActionResult EquipmentInfo()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        public IActionResult TbDemo()
        {
            if (CurrentUser.AuthStatus == 0)
            {
                return new RedirectResult("/CurrentUser/Auth");
            }
            if (CurrentUser.AuthStatus == 1)
            {
                return new RedirectResult("/CurrentUser/AuthAuditing");
            }
            return View();
        }

        /// <summary>
        /// 获取广告标签
        /// </summary>
        /// <returns></returns>
        public List<LabelDto> AdsListOption()
        {
            List<LabelDto> asdtag = _labelservice.GetAdsTag();
            return asdtag;
        }

        ///<summary>
        ///获取省份
        ///</summary>
        public IActionResult GetProvinces()
        {
            var Province = _placeservice.GetProvince();
            List<AreaModel> data = new List<AreaModel>();
            var name = "";
            var code = "";
            for(int i=0;i<Province.Count();i++)
            {               
                if (Province[i] !=null && Province[i]!="")
                {
                    var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                    name =service.GetByCode(uint.Parse(Province[i]))?.Name;
                    code = Province[i];
                    AreaModel areamodel = new AreaModel(Province[i], name);
                    data.Add(areamodel);
                }                
            }            
            return Json(new
            {
                Result = "Success",
                Data = data.OrderBy(o=>o.Name),
            });
        }

        ///<summary>
        ///获取城市
        ///</summary>
        public IActionResult GetCitys(string province)
        {
            var City = _placeservice.GetCity(province);
            List<AreaModel> data = new List<AreaModel>();
            for (int i = 0; i < City.Count(); i++)
            {
                if (City[i] != null && City[i]!="")
                {
                    var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                    var name = service.GetByCode(uint.Parse(City[i]))?.Name;
                    var code = City[i];
                    AreaModel areamodel = new AreaModel(City[i], name);
                    data.Add(areamodel);
                }               
            }
            return Json(new
            {
                Result = "Success",
                Data = data.OrderBy(o => o.Name),
            });
        }

        ///<summary>
        ///获取地区
        ///</summary>
        public IActionResult GetDistricts(string province,string city)
        {
            var District = _placeservice.GetDistrict(province,city);
            List<AreaModel> data = new List<AreaModel>();
            for (int i = 0; i < District.Count(); i++)
            {
                if (District[i] != null && District[i]!="")
                {
                    var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                    var name = service.GetByCode(uint.Parse(District[i]))?.Name;
                    var code = District[i];
                    AreaModel areamodel = new AreaModel(District[i], name);
                    data.Add(areamodel);
                }             
            }
            return Json(new
            {
                Result = "Success",
                Data = data.OrderBy(o => o.Name),
            });
        }

        ///<summary>
        ///获取分页列表
        ///</summary>

        public IActionResult GetAllPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _service.GetCurrentUserAllPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        ///<summary>
        ///获取播放器列表
        ///</summary>
        public IActionResult GetPlayerExceptBlackList(int startPage,int pageSize)
        {
            int rowCount = 0;
            var players = _playerService.GetAllPageList(startPage, pageSize, out rowCount);
            _playerService.GetPlayersAllInfo(ref players);

            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = players,
            });
        }


        /// <summary>
        /// 批量删除播放素材
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IActionResult DeleteMulti(string ids) {
            try {
                string[] idArray = ids.Split(',');
                List<Guid> delIds = new List<Guid>();
                foreach (string id in idArray) {
                    delIds.Add(Guid.Parse(id));
                }
                _service.DeleteBatch(delIds);
                return Json(new {
                    Result = "Success"
                });
            }
            catch (Exception ex) {
                return Json(new {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        
        /// <summary>
        ///删除单一order记录 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(Guid id) {
            try {
                _service.Delete(id);
                return Json(new {
                    Result = "Success"
                });
            }
            catch (Exception ex) {
                return Json(new {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }

        ///<summary>
        ///删除单一order记录（包括playerprogram）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteWithPlayerProgram(Guid id)
        {
            try
            {
                _service.RemoveAllPlayerProgram(id);
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

        /// <summary>
        /// 删除单一播放素材
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteSelectedSingleTR(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "没有需要删除的数据！",
                    });
                }

                MaterialDto dto=_materialservice.Get(id);

                //防止非法删除别人数据
                if (dto == null || dto.OwnerUserId != CurrentUser.Id)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "您的播放素材列表中不存在您要删除的素材！",
                    });
                }

                return Json(new
                {
                    Result = "Success",
                    row = dto,
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

        /// <summary>
        /// 根据id获取order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Get(Guid id) {
            var dto = _service.Get(id);
            return Json(dto);
        }

        /// <summary>
        ///加载订单详情
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public IActionResult GetAmount(List<CalculationModel> dtos)
        {
            float amount = 0;
            for(int i = 0; i < dtos.Count; i++)
            {
                amount += dtos[i].Price * dtos[i].Time * dtos[i].Count;
            }
            return Json(new
            {
                Result = "Success",
                amount =amount.ToString("F2"),
            });
        }


        /// <summary>
        /// 获取区域的播放单价
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="street"></param>
        /// <returns></returns>
        public List<PlayPriceDto> GetAreaPlayPrices(string province = "", string city = "", string district = "", string street = "")
        {
            return _playPriceService.GetAreaPlayPrices(province, city, district, street);
        }


        /// <summary>
        /// 获取套餐的单价
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public List<PlayPriceDto> GetComboPlayPrices(string combo)
        {
            return _playPriceService.GetComboPlayPrices(combo);
        }

        /// <summary>
        /// 获取设备的播放单价
        /// </summary>
        /// <param name="equipmentid"></param>
        /// <returns></returns>
        public List<PlayPriceDto> GetEquipmentPlayPrices(Guid equipmentid)
        {
            return _playPriceService.GetEquipmentPlayPrices(equipmentid);
        }


        /// <summary>
        /// 保存方案
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderMaterials"></param>
        /// <param name="orderTimes"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveOrder(OrderDto order, Guid programID, List<OrderAreaModel> orderAreas,List<OrderTimeDto> orderTimes)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (order == null)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "不能保存空订单！",
                });
            }
            if (programID == null)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "方案为空！",
                });
            }
            if (orderAreas == null || orderAreas.Count == 0)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "投放区域为空！",
                });
            }
            if (orderTimes == null || orderTimes.Count == 0)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "投放时间为空！",
                });
            }
            if (_service.SaveOrder(ref order, programID,orderAreas,orderTimes))
            {
                return Json(
                    new
                    {
                        Result = "Success",
                        id = order.Id,
                        name = order.Name,
                        orderNo = order.OrderNo,
                        amount = order.Amount,
                    });
            }
            else
            {
                return Json(
                new { Result = "Faild", Message = "数据保存失败！请确认所输入数据无误后重新再试！" });
            }
        }

        /// <summary>
        /// 保存方案(设备)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderMaterials"></param>
        /// <param name="orderTimes"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(OrderDto order, List<OrderMaterialDto> orderMaterials,List<OrderEquipmentDto> orderEquipments,ushort shoppingCartType)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (order == null)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "不能保存空订单！",
                });
            }
            if (orderMaterials == null || orderMaterials.Count == 0)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "播放素材为空！",
                });
            }
            if (orderEquipments== null || orderEquipments.Count == 0)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "投放媒体位为空！",
                });
            }
            if (_service.Save(ref order, orderMaterials, orderEquipments))
            {

                ShoppingCartController shoppingCartController=new ShoppingCartController(_service,_shoppingcartService,_equipmentService,_orderEquipmentService,_playerService);
                shoppingCartController.CartAD("","DelAll", shoppingCartType);
                return Json(
                    new
                    {
                        Result = "Success",
                        id = order.Id,
                        name = order.Name,
                        orderNo = order.OrderNo,
                        amount = order.Amount,
                    });
            }
            else
            {
                return Json(
                new { Result = "Faild", Message = "数据保存失败！请确认所输入数据无误后重新再试！" });
            }
        }

        /// <summary>
        /// 保存方案(设备)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderMaterials"></param>
        /// <param name="orderTimes"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveAll(OrderDto order, Guid programID ,List<Guid> playIds, ushort shoppingCartType)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (order == null)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "不能保存空订单！",
                });
            }
            if (programID == null)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "方案为空！",
                });
            }
            if (playIds == null || playIds.Count == 0)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = "投放播放器为空！",
                });
            }
            if (_service.SaveAll(ref order,programID, playIds))
            {

                ShoppingCartController shoppingCartController = new ShoppingCartController(_service,_shoppingcartService, _equipmentService, _orderEquipmentService,_playerService);
                shoppingCartController.CartAD("", "DelAll", shoppingCartType);
                return Json(
                    new
                    {
                        Result = "Success",
                        id = order.Id,
                        name = order.Name,
                        orderNo = order.OrderNo,
                        amount = order.Amount,
                    });
            }
            else
            {
                return Json(
                new { Result = "Faild", Message = "数据保存失败！请确认所输入数据无误后重新再试！" });
            }
        }

        //获取订单信息
        public IActionResult GetOrder(Guid id)
        {
            var order = _service.GetWithPlayerProgram(id);
            return Json(new
            {
                Result = "Success",
                order = order,
            });
        }

        //获取订单的播放器的地址
        public IActionResult GetOrderPlayers(Guid id)
        {
            /*var orderPlayers = _service.GetWithPlayer(id);
            return Json(new
            {
                Result="Success",
                orderPlayers=orderPlayers,
            });*/

            List<PlayerDto> players = new List<PlayerDto>();
            var orderplayers = _service.GetWithPlayer(id).OrderPlayers.Select(s => s.PlayerId).ToList();
            for (int i = 0; i < orderplayers.Count(); i++)
            {
                var player = _playerService.Get(orderplayers[i]);
                players.Add(player);
            }
            _playerService.GetPlayersAllInfo(ref players);
            return Json(new
            {
                Result = "Success",
                orderplayers = players,
            });
        }

        //获取订单时间段
        public IActionResult GetOrderTimes(Guid id)
        {
            var ordertimes = _ordertimeservice.GetByOrderId(id);
            var areaArr = ordertimes[0].Area.Split(",");
            List<AreaViewModel> areas = new List<AreaViewModel>();
            for(int i = 0; i < areaArr.Length; i++)
            {
                var service = (IAdministrativeAreaAppService)HttpHelper.ServiceProvider.GetService(typeof(IAdministrativeAreaAppService));
                AreaViewModel areaViewModel = new AreaViewModel();
                areaViewModel.ProvinceCode = (areaArr[i].Split("_"))[0];
                areaViewModel.Province = service.GetByCode(uint.Parse(areaViewModel.ProvinceCode))?.Name;
                areaViewModel.CityCode = (areaArr[i].Split("_"))[1] != "null" ? (areaArr[i].Split("_"))[1] : null;
                areaViewModel.DistrictCode = (areaArr[i].Split("_"))[2] != "null" ? (areaArr[i].Split("_"))[2] : null;
                if (areaViewModel.CityCode != null)
                {
                    areaViewModel.City = service.GetByCode(uint.Parse(areaViewModel.CityCode))?.Name;
                    if (areaViewModel.DistrictCode != null)
                    {
                        areaViewModel.District = service.GetByCode(uint.Parse(areaViewModel.DistrictCode))?.Name;
                    }
                    else
                    {
                        areaViewModel.District = "";
                    }
                }
                else
                {
                    areaViewModel.City = "";
                    areaViewModel.District = "";
                }
                areas.Add(areaViewModel);
            }

            return Json(new
            {
                Result = "Success",
                ordertimes = ordertimes,
                areas=areas,
            });
        }

        //获取订单素材
        public IActionResult GetOrderMaterials(Guid id)
        {
            List<MaterialDto> materials = new List<MaterialDto>();
            var ordermaterials = _ordermaterialservice.GetByOrderId(id);
            for(int i = 0; i < ordermaterials.Count(); i++)
            {
                var material = _materialservice.GetInfo(ordermaterials[i].MaterialId);
                materials.Add(material);
            }
            return Json(new
            {
                Result = "Success",
                ordermaterials = materials,
            });
        }

        //获取订单设备
        public IActionResult GetOrderEquipments(Guid id)
        {
            List<EquipmentDto> equipments = new List<EquipmentDto>();
            var orderequipments = _orderEquipmentService.GetByOrderId(id);
            for(int i = 0; i < orderequipments.Count(); i++)
            {
                var equipment = _equipmentService.Get(orderequipments[i].EquipmentId);
                equipments.Add(equipment);
            }
            return Json(new
            {
                Result = "Success",
                orderequipments = equipments,
            });
        }


        //申请退款
        public IActionResult ApplyRefund(Guid orderId, string applyRefundReason) 
        {
            var order = _service.Get(orderId);
            if (order == null)
            {
                return Json(
                new { Result = "Faild", Message = "系统不存在此订单" });
            }

            if (!((new ushort[] { OrderDto.ORDER_STATUS_AUDITING, OrderDto.ORDER_STATUS_PREPARING }).Contains(order.Status))) {
                return Json(new {
                    Result = "Faild",
                    Message = "订单不在退款状态"
                });

            }
            if (_service.ApplyRefund(CurrentUser.Id, order, applyRefundReason)) {
                return Json(new {
                    Result = "Success"
                });

            }
            return Json(new {
                Result = "Faild",
                Message = "申请退款失败"
            });
        }

        //取消退款
        public IActionResult CancelRefund(Guid orderId)
        {
            var order = _service.Get(orderId);
            if (order == null)
            {
                return Json(
                new { Result = "Faild", Message = "系统不存在此订单" });
            }
            switch (order.Status)
            {
                case 7:
                    order.Status = OrderDto.ORDER_STATUS_AUDITING;
                    break;
                case 8:
                    order.Status = OrderDto.ORDER_STATUS_PREPARING;
                    break;
                default:
                    return Json(
                            new { Result = "Faild", Message = "此订单不允许退款，若有疑问，请联系客服" });
            }
            if (_service.Update(order))
            {
                return Json(
                    new
                    {
                        Result = "Success"
                    });
            }
            else
            {
                return Json(
                new { Result = "Faild", Message = "数据保存失败！" });
            }
        }

    }

}
