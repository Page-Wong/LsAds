using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.MVC.Models;
using LsAdmin.Application.TradeApp;
using LsAdmin.Application.OrderApp;
using Alipay.AopSdk.AspnetCore;
using LsAdmin.Application.TradeApp.Dtos;
using LsAdmin.Application.OrderApp.Dtos;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public partial class SettlementCenterController : LsAdminControllerBase
    {
        private readonly IOrderAppService _orderService;
        private readonly ITradeAppService _tradeService;
        private readonly IAlipayService _alipayService;
        public SettlementCenterController(IOrderAppService orderService, ITradeAppService tradeService, IAlipayService alipayService) {
            _orderService = orderService;
            _tradeService = tradeService;
            _alipayService = alipayService;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        private bool CheckOrderNo(string orderNo, out string message) {
            message = "";
            var order = _orderService.GetByOrderNo(orderNo);
            if (order == null) {
                message = "订单号有误，请检查再支付";
                return false;
            }
            if (!(new ushort[] { OrderDto.ORDER_STATUS_PUBLISHED }).Contains(order.Status)) {
                message = "订单不在支付状态，请检查再支付";
                return false;
            }
            return true;
        }

        public void Pay(PayModel payModel) {

        }
    }
}
