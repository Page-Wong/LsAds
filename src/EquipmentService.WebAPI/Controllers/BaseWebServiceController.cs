using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using NLog;
using System.Linq;
using EquipmentService.WebAPI.Common;

namespace EquipmentService.WebAPI.Controllers {
    public abstract class BaseWebServiceController : Controller {

        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var ip = filterContext.HttpContext.Connection.LocalIpAddress.ToString();
            if (!WebServiceIpWhitelist.Ips.Contains(ip)) {
                //TODO G 写入拦截日志
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 获取服务端验证的第一条错误信息
        /// </summary>
        /// <returns></returns>
        public string GetModelStateError() {
            foreach (var item in ModelState.Values) {
                if (item.Errors.Count > 0) {
                    return item.Errors[0].ErrorMessage;
                }
            }
            return "";
        }

        #region 解析请求参数

        protected Dictionary<string, string> GetRequestGet() {
            Dictionary<string, string> sArray = new Dictionary<string, string>();

            ICollection<string> requestItem = Request.Query.Keys;
            foreach (var item in requestItem) {
                sArray.Add(item, Request.Query[item]);

            }
            return sArray;

        }

        protected Dictionary<string, string> GetRequestPost() {
            Dictionary<string, string> sArray = new Dictionary<string, string>();

            ICollection<string> requestItem = Request.Form.Keys;
            foreach (var item in requestItem) {
                sArray.Add(item, Request.Form[item]);

            }
            return sArray;

        }

        #endregion
    }
}