using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using LsAdmin.MVC.Controllers;
using LsAdmin.Application.OrderApp;
using Microsoft.AspNetCore.Http;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.AspnetCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Newtonsoft.Json.Linq;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.TradeApp;
using LsAdmin.Application.TradeApp.Dtos;
using LsAdmin.MVC.Models;

namespace LsAdmin.MVC.Controllers {
    public partial class SettlementCenterController {
        

        #region 发起支付

        /// <summary>
        /// 发起支付请求
        /// <returns></returns>
        public IActionResult AlipayPayRequest(PayModel payModel) {
            if (!CheckOrderNo(payModel.OrderNo, out string message)) {
                return Json(new {
                    Result = "Fail",
                    Message = message
                });
            }
            var orderDto = _orderService.GetByOrderNo(payModel.OrderNo);
            if (!_orderService.PayByAlipay(CurrentUser.Id, orderDto, out var tradeDto)) {
                return Json(new {
                    Result = "Fail",
                    Message = "支付出错了，请联系客服"
                });
            }            

            // 组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel {
                Body = payModel.ItemBody,
                Subject = payModel.Subject,
                TotalAmount = payModel.TotalAmout.ToString(),
                OutTradeNo = payModel.OrderNo + "_" + tradeDto.TradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(this.Request.Scheme + "://" + this.Request.Host.Value + "/SettlementCenter/AlipayPayCallback");
            // 设置异步通知接收地址
            request.SetNotifyUrl(this.Request.Scheme + "://" + this.Request.Host.Value + "/SettlementCenter/AlipayPayNotify");
            // 将业务model载入到request
            request.SetBizModel(model);

            var response = _alipayService.SdkExecute(request);
            //跳转支付宝支付            
            //Response.Redirect(_alipayService.Options.Gatewayurl + "?" + response.Body);
            return Json(new {
                Result = "Success",
                Url =  _alipayService.Options.Gatewayurl + "?" + response.Body
            });
        }

        #endregion

        #region 支付异步回调通知

        /// <summary>
        /// 支付异步回调通知 需配置域名 因为是支付宝主动post请求这个action 所以要通过域名访问或者公网ip
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public void AlipayPayNotify() {
            /* 实际验证过程建议商户添加以下校验。
			1、商户需要验证该通知数据中的out_trade_no是否为商户系统中创建的订单号，
			2、判断total_amount是否确实为该订单的实际金额（即商户订单创建时的金额），
			3、校验通知中的seller_id（或者seller_email) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id/seller_email）
			4、验证app_id是否为该商户本身。
			*/
            Dictionary<string, string> sArray = GetRequestPost();
            if (sArray.Count == 0) {
                return;
            }
            bool flag = _alipayService.RSACheckV1(sArray);
            if (!flag) {
                return;
            }
            string orderNo = sArray["out_trade_no"].Split("_").GetValue(0).ToString();
            string tradeNo = sArray["out_trade_no"].Split("_").GetValue(1).ToString();
            var order = _orderService.GetByOrderNo(orderNo);
            var trade = _tradeService.GetByTradeNo(tradeNo);
            if (order == null || trade ==null ||
                !_orderService.GetTradesByOrderId(order.Id).Select(item => item.Id).Contains(trade.Id) ||
                order.Amount != float.Parse(sArray["total_amount"]) || 
                _alipayService.Options.Uid != sArray["seller_id"] || 
                _alipayService.Options.AppId != sArray["app_id"]) {
                return;
            }
            _orderService.AfterPay(order, trade);
        }

        #endregion

        #region 支付同步回调

        /// <summary>
        /// 支付同步回调
        /// </summary>
        [HttpGet]
        public IActionResult AlipayPayCallback() {
            /* 实际验证过程建议商户添加以下校验。
			1、商户需要验证该通知数据中的out_trade_no是否为商户系统中创建的订单号，
			2、判断total_amount是否确实为该订单的实际金额（即商户订单创建时的金额），
			3、校验通知中的seller_id（或者seller_email) 是否为out_trade_no这笔单据的对应的操作方（有的时候，一个商户可能有多个seller_id/seller_email）
			4、验证app_id是否为该商户本身。
			*/
            ViewData["Msg"] = "支付失败";
            ViewData["Success"] = false;
            Dictionary<string, string> sArray = GetRequestGet();
            if (sArray.Count == 0) {
                return View();
            }
            bool flag = _alipayService.RSACheckV1(sArray);
            if (!flag) {
                return View();
            }

            string orderNo = sArray["out_trade_no"].Split("_").GetValue(0).ToString();
            string tradeNo = sArray["out_trade_no"].Split("_").GetValue(1).ToString();
            var order = _orderService.GetByOrderNo(orderNo);
            var trade = _tradeService.GetByTradeNo(tradeNo);
            if (order == null || trade == null ||
                !_orderService.GetTradesByOrderId(order.Id).Select(item => item.Id).Contains(trade.Id) ||
                order.Amount != float.Parse(sArray["total_amount"]) ||
                _alipayService.Options.Uid != sArray["seller_id"] ||
                _alipayService.Options.AppId != sArray["app_id"]) {
                return View();
            }
            if (_orderService.AfterPay(order, trade)) {
                ViewData["Msg"] = "支付成功";
                ViewData["Success"] = true;
                return View();
            }
            return View();
        }

        #endregion

        #region 订单查询

        [HttpGet]
        public IActionResult AlipayPayQuery() {
            return View();
        }

        [HttpPost]
        public JsonResult AlipayPayQuery(string tradeno, string alipayTradeNo) {
            /*DefaultAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", "2.0",
			    Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);*/
            AlipayTradeQueryModel model = new AlipayTradeQueryModel {
                OutTradeNo = tradeno,
                TradeNo = alipayTradeNo
            };

            AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
            request.SetBizModel(model);

            var response = _alipayService.Execute(request);
            return Json(response.Body);
        }

        #endregion

        #region 订单退款
        /// <summary>
        /// 订单退款
        /// </summary>
        /// <param name="tradeno">商户订单号</param>
        /// <param name="alipayTradeNo">支付宝交易号</param>
        /// <param name="refundAmount">退款金额</param>
        /// <param name="refundReason">退款原因</param>
        /// <param name="refundNo">退款单号</param>
        /// <returns></returns>
        [HttpPost]
        private JsonResult AlipayPayRefund(string tradeno, string alipayTradeNo, string refundAmount, string refundReason, string refundNo) {
            /*DefaultAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", "2.0",
			    Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);*/

            AlipayTradeRefundModel model = new AlipayTradeRefundModel();
            model.OutTradeNo = tradeno;
            model.TradeNo = alipayTradeNo;
            model.RefundAmount = refundAmount;
            model.RefundReason = refundReason;
            model.OutRequestNo = refundNo;

            AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
            request.SetBizModel(model);

            var response = _alipayService.Execute(request);
            return Json(response.Body);
        }

        #endregion

        #region 退款查询
        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="tradeno">商户订单号</param>
        /// <param name="alipayTradeNo">支付宝交易号</param>
        /// <param name="refundNo">退款单号</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AlipayPayRefundQuery(string tradeno, string alipayTradeNo, string refundNo) {
            /*DefaultAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", "2.0",
			    Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);*/

            if (string.IsNullOrEmpty(refundNo)) {
                refundNo = tradeno;
            }

            AlipayTradeFastpayRefundQueryModel model = new AlipayTradeFastpayRefundQueryModel();
            model.OutTradeNo = tradeno;
            model.TradeNo = alipayTradeNo;
            model.OutRequestNo = refundNo;

            AlipayTradeFastpayRefundQueryRequest request = new AlipayTradeFastpayRefundQueryRequest();
            request.SetBizModel(model);

            var response = _alipayService.Execute(request);
            return Json(response.Body);
        }

        #endregion

        #region 订单关闭
        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="tradeno">商户订单号</param>
        /// <param name="alipayTradeNo">支付宝交易号</param>
        /// <returns></returns>
        [HttpPost]
        private JsonResult AlipayPayOrderClose(string tradeno, string alipayTradeNo) {
            /*DefaultAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", "2.0",
			    Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);*/

            AlipayTradeCloseModel model = new AlipayTradeCloseModel();
            model.OutTradeNo = tradeno;
            model.TradeNo = alipayTradeNo;

            AlipayTradeCloseRequest request = new AlipayTradeCloseRequest();
            request.SetBizModel(model);

            var response = _alipayService.Execute(request);
            return Json(response.Body);
        }

        #endregion
    }

}
