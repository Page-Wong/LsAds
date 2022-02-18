using Microsoft.AspNetCore.Mvc;
using System;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.TradeApp;
using System.Linq;
using LsAdmin.Application.TradeApp.Dtos;

namespace LsAdmin.MVC.Controllers
{
    public class AdminOrderController : LsAdminControllerBase {

        private readonly IOrderAppService _service;
        private readonly ITradeAppService _tradeService;


        public AdminOrderController(IOrderAppService service, ITradeAppService tradeService)
        {
            _service = service;
            _tradeService = tradeService;
        }

        // GET: /<controller>/
        public IActionResult Index() {
            return View();
        }

        ///<summary>
        ///获取分页列表
        ///</summary>

        public IActionResult GetAllPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _service.GetAllPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        [HttpPost]
        public IActionResult Audit(Guid orderId, int audit, string comment)
        {
            var order = _service.Get(orderId);
            if (order == null) {
                return Json(
                new { Result = "Faild", Message = "系统不存在此订单" });

            }
            switch (audit) {
                case 0:
                    order.Status = OrderDto.ORDER_STATUS_CANCEL;
                    break;
                case 1:
                    order.Status = OrderDto.ORDER_STATUS_PREPARING;
                    break;
                default:
                    return Json(
                            new { Result = "Faild", Message = "审核结果无效" });
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

        [HttpGet]
        public IActionResult RefundConfirm() {
            return View();
        }
        [HttpPost]
        public IActionResult OrderRefundConfirm(string orderNo) {
            var orderDto = _service.GetByOrderNo(orderNo);
            if (!new ushort[] { OrderDto.ORDER_STATUS_REFUNDAFTERAUDIT, OrderDto.ORDER_STATUS_REFUNDBEFOREAUDIT }.Contains(orderDto.Status))
            {
                return Json(new { Result = "Faild", Message = "订单状态有误" });
            }
            var trades = _service.GetTradesByOrderId(orderDto.Id);
            if (trades.Select(item => item.TradeStatus).Contains(TradeDto.TRADESTATUS_TRADING))
            {
                return Json(new { Result = "Faild", Message = "订单尚有交易未处理，请处理后再确认" });
            }
            orderDto.Status = OrderDto.ORDER_STATUS_CANCEL;
            if (_service.Update(orderDto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild", Message = "处理出错" });
        }
        public IActionResult TradeRefundConfirm(string orderNo, string tradeNo)
        {
            var orderDto = _service.GetByOrderNo(orderNo);
            if (!new ushort[] { OrderDto.ORDER_STATUS_REFUNDAFTERAUDIT, OrderDto.ORDER_STATUS_REFUNDBEFOREAUDIT }.Contains(orderDto.Status)){
                return Json(new { Result = "Faild", Message = "订单状态有误" });
            }
            if (!_service.GetTradesByOrderId(orderDto.Id).Select(item => item.TradeNo).Contains(tradeNo)) {
                return Json(new { Result = "Faild", Message = "交易与订单不匹配" });
            }
            var tradeDto = _tradeService.GetByTradeNo(tradeNo);
            if (tradeDto.TradeStatus != TradeDto.TRADESTATUS_TRADING)
            {
                return Json(new { Result = "Faild", Message = "交易状态有误" });
            }
            tradeDto.TradeStatus = TradeDto.TRADESTATUS_SUCCESS;
            if (_tradeService.Update(tradeDto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild", Message = "处理出错" });
        }

        public IActionResult GetAllRefundPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _service.GetAllRefundPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }
        public IActionResult GetOrderRefundTrade(string orderNo) {
            var order = _service.GetByOrderNo(orderNo);
            var result = _service.GetTradesWithBusinessTypeByOrderId(order.Id);
            return Json(new {
                Result = "Success",
                Rows = result
            });
        }

    }

}
