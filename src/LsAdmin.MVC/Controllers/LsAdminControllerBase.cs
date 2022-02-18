using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using LsAdmin.Utility.Convert;
using LsAdmin.Application.UserApp.Dtos;
using Microsoft.Extensions.Caching.Memory;
using LsAdmin.Domain.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using LsAdmin.EntityFrameworkCore.Repositories;
using LsAdmin.Application.UserApp;
using LsAdmin.Domain.Entities;
using System.Collections.Generic;
using LsAdmin.Application.MenuApp;
using LsAdmin.Application.MenuApp.Dtos;
using Microsoft.Extensions.Logging;
using NLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using LsAdmin.Application.NotifyApp;

namespace LsAdmin.MVC.Controllers {
    public abstract class LsAdminControllerBase : Controller {

        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private UserDto _currentUser;
        protected UserDto CurrentUser {
            get {
                if (_currentUser == null) {
                    _currentUser = ByteConvertHelper.Bytes2Object<UserDto>(HttpContext.Session.Get("CurrentUser"));
                }
                return _currentUser;
            }
            set {
                HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(value));
            }
        }
        private UserDto _currentUserWithRoles;
        protected UserDto CurrentUserWithRoles {
            get {
                if (_currentUserWithRoles == null) {
                    if (HttpContext.Session.Get("CurrentUserWithRoles") == null && CurrentUser != null) {
                        IUserAppService userAppService = (IUserAppService)HttpContext.RequestServices.GetService(typeof(IUserAppService));
                        HttpContext.Session.Set("CurrentUserWithRoles", ByteConvertHelper.Object2Bytes(userAppService.Get(CurrentUser.Id)));
                    }
                    _currentUserWithRoles = ByteConvertHelper.Bytes2Object<UserDto>(HttpContext.Session.Get("CurrentUserWithRoles"));
                }
                return _currentUserWithRoles;
            }
        }

        protected INotifyAppService Notify => (INotifyAppService)HttpContext.RequestServices.GetService(typeof(INotifyAppService));

        private List<MenuDto> _currentUserAllMenus;
        protected List<MenuDto> CurrentUserMenus {
            get {
                if (CurrentUser == null) {
                    _currentUserAllMenus = null;
                    return null;
                }
                if (_currentUserAllMenus == null) {
                    IMenuAppService menuAppService = (IMenuAppService)HttpContext.RequestServices.GetService(typeof(IMenuAppService));
                    _currentUserAllMenus = menuAppService.GetAllMenusByUser(CurrentUser.Id);
                }
                return _currentUserAllMenus;
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allowAnonymous = (filterContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.
                        GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();
            if (allowAnonymous == null) {            
                byte[] result; 
                filterContext.HttpContext.Session.TryGetValue("CurrentUser",out result);
                if (result == null)
                {
                    filterContext.Result = new RedirectResult("/Account/Index");
                    return;
                }
                string controller = filterContext.ActionDescriptor.RouteValues["controller"];
                string action = filterContext.ActionDescriptor.RouteValues["action"];
                string url = "/" + controller + "/" + action;
                /*if (!(controller == "System" && action == "Error") && CurrentUserMenus.Find(m => m.Url == url) == null) {
                    filterContext.Result = new RedirectResult("/System/Error/Auth");
                    return;
                }*/
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 获取服务端验证的第一条错误信息
        /// </summary>
        /// <returns></returns>
        public string GetModelStateError()
        {
            foreach (var item in ModelState.Values)
            {
                if (item.Errors.Count > 0)
                {
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
