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
using Microsoft.AspNetCore.Http;
using LsAdmin.Utility.Convert;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public partial class SettlementCenterController {
        private string ERROR_PAYMENTPASSWORD_COUNT => "ERROR_PAYMENTPASSWORD_COUNT_" + CurrentUser.Id.ToString();
        private string ERROR_PAYMENTPASSWORD_TIME => "ERROR_PAYMENTPASSWORD_TIME_" + CurrentUser.Id.ToString();
        private const int MAX_ERROR_PAYMENTPASSWORD_COUNT = 5;
        private const int ERROR_PAYMENTPASSWORD_LOCK_MINUTES = 5;

        [HttpGet]
        public IActionResult PointTradeList() {
            return View();
        }

        public IActionResult GetPointTradePageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _tradeService.GetCurrentUserPointAllPageList(startPage, pageSize, out rowCount);
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        [HttpGet]
        public IActionResult GetPointTradeListBalance() {            
            return Json(new {
                Result = "Success",
                Data = new {
                    AllBalance= _tradeService.GetCurrentUserPointAllBalance(),
                    Balance = _tradeService.GetCurrentUserPointBalance(),
                    Expense = _tradeService.GetCurrentUserPointExpenseByDate(DateTime.Now.AddMonths(-1).AddDays(1), DateTime.Now.AddMonths(1).AddDays(-1))
                }
            });
        }

        #region 积分支付
        private bool CheckPointPayModel(PayModel model, out string message) {
            message = "";
            if (!CheckOrderNo(model.OrderNo, out message)) {
                return false;
            }
            var pointBalance = _tradeService.GetCurrentUserPointBalance();
            if (pointBalance < model.TotalAmout) {
                message = "你的积分不足，请选择其他支付方式";
                return false;
            }
            return true;
        }

        public IActionResult PointPayRequest(PayModel model) {
            if (!CheckPointPayModel(model, out string message)) {
                return Json(new {
                    Result = "Fail",
                    Message = message
                });
            }
            return Json(new {
                Result = "Success"
            });
        }

        public IActionResult PointPay(PayModel model, string paymentPassword) {
            if (!CheckPointPayModel(model, out string message)) {
                return Json(new {
                    Result = "Fail",
                    Message = message
                });
            }
            if (CurrentUser.PaymentPassword == null) {
                return Json(new {
                    Result = "Fail",
                    Message = "尚未设置支付密码，<a href='/CurrentUser/ChangePaymentPassword' class='btn-link' target='_blank'>点击设置支付密码</a>"
                });
            }
            //检测输错密码次数，超过最大次数禁止继续支付
            int? errorPaymentpasswordCount = (int?)CacheHelper.GetCacheValue(ERROR_PAYMENTPASSWORD_COUNT);
            string errorPaymentpasswordTimeStr = (string)CacheHelper.GetCacheValue(ERROR_PAYMENTPASSWORD_TIME);
            if (errorPaymentpasswordCount != null &&
                errorPaymentpasswordCount > MAX_ERROR_PAYMENTPASSWORD_COUNT &&
                !String.IsNullOrEmpty(errorPaymentpasswordTimeStr)) {
                if (DateTime.Parse(errorPaymentpasswordTimeStr).AddMinutes(ERROR_PAYMENTPASSWORD_LOCK_MINUTES) > DateTime.Now) {
                    return Json(new {
                        Result = "Fail",
                        Message = "积分支付已锁定，锁定时间剩余 " + Math.Round((DateTime.Parse(errorPaymentpasswordTimeStr).AddMinutes(ERROR_PAYMENTPASSWORD_LOCK_MINUTES) - DateTime.Now).TotalSeconds) + " 秒"
                    });
                }
                else {
                    CacheHelper.RemoveCache(ERROR_PAYMENTPASSWORD_COUNT);
                    CacheHelper.RemoveCache(ERROR_PAYMENTPASSWORD_TIME);
                    errorPaymentpasswordCount = null;
                    errorPaymentpasswordTimeStr = null;
                }
            }

            if (PasswordConvertHelper.Create(paymentPassword) != CurrentUser.PaymentPassword) {
                errorPaymentpasswordCount = (errorPaymentpasswordCount ?? 0) +1;
                CacheHelper.SetChacheValue(ERROR_PAYMENTPASSWORD_COUNT, errorPaymentpasswordCount);
                if ((errorPaymentpasswordCount >= MAX_ERROR_PAYMENTPASSWORD_COUNT)) {
                    errorPaymentpasswordTimeStr = DateTime.Now.ToString("f");
                    CacheHelper.SetChacheValue(ERROR_PAYMENTPASSWORD_TIME, errorPaymentpasswordTimeStr); 
                }
                return Json(new {
                    Result = "Fail",
                    Message = "支付密码错误，剩余错误次数 " + (MAX_ERROR_PAYMENTPASSWORD_COUNT - errorPaymentpasswordCount) + " 次" + (errorPaymentpasswordCount == MAX_ERROR_PAYMENTPASSWORD_COUNT ? "，积分支付锁定" + ERROR_PAYMENTPASSWORD_LOCK_MINUTES + "分钟" : "") + "，<a href='/CurrentUser/ChangePaymentPassword' class='btn-link' target='_blank'>点击重置支付密码</a>"
                });
            }

            CacheHelper.RemoveCache(ERROR_PAYMENTPASSWORD_COUNT);
            CacheHelper.RemoveCache(ERROR_PAYMENTPASSWORD_TIME);
            errorPaymentpasswordCount = null;
            errorPaymentpasswordTimeStr = null;

            var order = _orderService.GetByOrderNo(model.OrderNo);
            if (_orderService.PayByPoint(CurrentUser.Id, order, out var tradeDto)) {
                return Json(new {
                    Result = "Success"
                });
            }
            return Json(new {
                Result = "Fail",
                Message = "支付异常，请联系管理员"
            });
        }

        [HttpGet]
        public IActionResult GetPointPayBalanceByAmout(float amout) {
            //TODO G 改为积分现金比例后的需求数
            var expense = amout;
            return Json(new {
                Result = "Success",
                Data = new {
                    Expense = expense,
                    Balance = _tradeService.GetCurrentUserPointBalance()
                }
            });
        }
        #endregion
    }
}
