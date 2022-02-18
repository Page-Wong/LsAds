using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.ShoppingCartApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Utility.Service;
using LsAdmin.Application.ShoppingCartApp.Dtos;
using LsAdmin.Application.OrderEquipmentApp;
using LsAdmin.Application.PlayerApp.Dtos;
using LsAdmin.Application.PlayerApp;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Application.OrderApp;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class ShoppingCartController : LsAdminControllerBase
    {
        private readonly IOrderAppService _orderService;
        private readonly IShoppingCartAppService _service;
        private readonly IEquipmentAppService _equipmentService;
        private readonly IOrderEquipmentAppService _orderEquipmentService;
        private readonly IPlayerAppService _playerService;

        public ShoppingCartController(IOrderAppService orderService,IShoppingCartAppService service,IEquipmentAppService equipmentService,IOrderEquipmentAppService orderEquipmentService, IPlayerAppService playerService)
        {
            _orderService = orderService;
            _service = service;
            _equipmentService = equipmentService;
            _orderEquipmentService = orderEquipmentService;
            _playerService = playerService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        //添加、删除购物车操作数据库
        public IActionResult CartAD(string playerIds, string cartType,ushort type)
        {
            ShoppingCartDto cart = _service.GetCurrentUserByType(type).FirstOrDefault();
            if (cartType == "add")
            {               
                if (cart == null)
                {
                    ShoppingCartDto newcart = new ShoppingCartDto();
                    newcart.UserId = CurrentUser.Id;
                    newcart.EquipmentIds = playerIds;
                    newcart.CreateTime = DateTime.Now;
                    newcart.Type = type;
                    _service.InsertOrUpdate(ref newcart);
                    return Json(new
                    {
                        Result = "Success",
                        accountIds = newcart.EquipmentIds,
                    });
                }
                else
                {
                    if (playerIds.Split(",").Length>1)
                    {
                        cart.EquipmentIds = AddIds(cart.EquipmentIds, playerIds);
                    }
                    else
                    {
                        cart.EquipmentIds = AddId(cart.EquipmentIds, playerIds);
                    }
                    _service.Update(cart);
                }
                return Json(new
                {
                    Result = "Success",
                    accountIds = cart.EquipmentIds,
                });
            }
            else
            {
                if (cartType == "del")
                {
                    cart.EquipmentIds = RemoveIds(cart.EquipmentIds, playerIds);
                    if (cart.EquipmentIds == "" || cart.EquipmentIds == null)
                    {
                        _service.Delete(cart.Id);
                    }
                    else
                    {
                        _service.Update(cart);
                    }
                    return Json(new
                    {
                        Result = "Success",
                        accountIds = cart.EquipmentIds,
                    });
                }
                else
                {
                    if (cart != null)
                    {
                        _service.Delete(cart.Id);
                    }                 
                    return Json(new
                    {
                        Result = "Success",
                        accountIds = "",
                    });
                }
            }                         
        }

        //增加一组ID
        public string AddIds(string source,string ids)
        {
            List<string> list = new List<string>(source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> add = new List<string>(ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            add.RemoveAll(x => list.Contains(x));
            if (add.Count() > 0)
            {
                return string.Join(",", list.ToArray()) + "," + string.Join(",", add.ToArray());
            }
            else
            {
                return string.Join(",", list.ToArray());
            }
            
        }

        //增加一组ID
        public string AddId(string source, string id)
        {
            return source + "," + id;
        }

        //删除一组ID
        public string RemoveIds(string source,string ids)
        {
            List<string> list = new List<string>(source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> del = new List<string>(ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            list.RemoveAll(x => del.Contains(x));
            return string.Join(",", list.ToArray());
        }

        //删除一个ID
        public string RemoveId(string source,string id)
        {
            return AddComma(source).Replace(AddComma(id), ",").Trim(',');
        }

        private static string AddComma(string s)
        {
            return "," + s + ",";
        }

        //加载购物车
        /*public IActionResult GetCartList(ushort type)
        {
            var result = _service.GetCurrentUserByType(type).ToList();
            if (result.Count>0)
            {
                var equipmentIds = result.FirstOrDefault().EquipmentIds;//应在shoppingcarts的
                var equipmentIdArr = equipmentIds.Split(",");
                List<EquipmentDto> equipmentlist = new List<EquipmentDto>();
                for (int i = 0; i < equipmentIdArr.Length; i++)
                {
                    var equipment = _equipmentService.Get(Guid.Parse(equipmentIdArr[i]));
                    equipmentlist.Add(equipment);
                }
                return Json(new
                {
                    Result = "Success",
                    accountIds = equipmentIds,
                    equipmentList = equipmentlist,

                });
            }
            else
            {
                return Json(new
                {
                    Result = "Success",
                    accountIds = "",
                    equipmentList = "",

                });
            }
            
        }*/

        //加载购物车
        public IActionResult GetCartList(ushort type)
        {
            var result = _service.GetCurrentUserByType(type).ToList();
            if (result.Count>0)
            {
                var playerIds = result.FirstOrDefault().EquipmentIds;//应将shoppingcarts的equipments改为players
                var playerIdArr = playerIds.Split(",");
                List<PlayerDto> playertlist = new List<PlayerDto>();
                for (int i = 0; i < playerIdArr.Length; i++)
                {
                    var player = _playerService.Get(Guid.Parse(playerIdArr[i]));
                    playertlist.Add(player);                 
                }
                _playerService.GetPlayersAllInfo(ref playertlist);
                return Json(new
                {
                    Result = "Success",
                    accountIds = playerIds,
                    playerList = playertlist,

                });
            }
            else
            {
                return Json(new
                {
                    Result = "Success",
                    accountIds = "",
                    playerList = "",

                });
            }
            
        }


        //加载订单中的媒体位到购物车       

        public IActionResult AddCartList(Guid orderId,ushort type)
        {
            var result = _orderService.GetOrderPlayers(orderId);
            //var result = _orderEquipmentService.GetByOrderId(orderId);
            var equipmentIds = "";
            for (int i = 0; i < result.Count(); i++)
            {
                equipmentIds = equipmentIds + result[i] + ",";
            }
            _service.DeleteBatch(_service.GetCurrentUserByType(type).Select(s=>s.Id).ToList());
            ShoppingCartDto newcart = new ShoppingCartDto();
            newcart.UserId = CurrentUser.Id;
            newcart.CreateTime = DateTime.Now;
            newcart.Type = type;
            newcart.EquipmentIds = equipmentIds.Trim(',');
            _service.InsertOrUpdate(ref newcart);
            return Json(new
            {
                Result = "Success",
            });
        }
    }
}
