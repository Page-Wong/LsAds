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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class HomeController : LsAdminControllerBase
    {

        private readonly IOrderAppService _orderService;
        private readonly IOrderTimeAppService _orderTimeService;
        private readonly IPlayHistoryAppService _playHistoryService;
        public HomeController(IOrderAppService orderService, IOrderTimeAppService orderTimeService, IPlayHistoryAppService playHistoryService) {
            _orderService = orderService;
            _orderTimeService = orderTimeService;
            _playHistoryService = playHistoryService;
        }
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult InitData() {
            var orders = _orderService.GetCurrentUserAllList();
            ushort[] notOrderTotalStatus = { OrderDto.ORDER_STATUS_UNPUBLISHED, OrderDto.ORDER_STATUS_CANCEL };
            var orderIds = orders.Where(o => o.Status == OrderDto.ORDER_STATUS_RUNNING || o.Status == OrderDto.ORDER_STATUS_COMPLETE).Select(o => o.Id).ToArray();
            var orderTimes = _orderTimeService.GetByOrderIds(orderIds);
            return Json(new {
                Result = "Success",
                Data = new {
                    OrderTotal = orders.Where(o => !notOrderTotalStatus.Contains(o.Status)).Count(),
                    OrderRunningCount = orders.Where(o => o.Status == OrderDto.ORDER_STATUS_RUNNING).Count(),
                    OrderCompleteCount = orders.Where(o => o.Status == OrderDto.ORDER_STATUS_COMPLETE).Count(),
                    PlayTotal = _playHistoryService.GetCountByOrderTimeIds(orderTimes.Select(item =>item.Id)?.ToArray())
                }
            });
        }        
    }
}
