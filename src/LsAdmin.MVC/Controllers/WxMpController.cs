using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.UserApp;
using Microsoft.AspNetCore.Http;
using LsAdmin.Utility.Convert;
using LsAdmin.MVC.Models;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Application.DepartmentApp;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Collections.Generic;
using LsAdmin.Application.RoleApp;
using LsAdmin.Utility.SMS;
using LsAdmin.Utility.Auth;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen.Entities.Request;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;
using Senparc.Weixin.WxOpen.Containers;
using LsAdmin.Utility.Json;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.Exceptions;
using System.Threading.Tasks;
using System.Threading;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class WxMpController : Controller {
        public readonly string appId = Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
        private readonly string appSecret = Config.SenparcWeixinSetting.WeixinAppSecret;//与微信公众账号后台的AppId设置保持一致，区分大小写。    
        public static readonly string Token = Config.SenparcWeixinSetting.Token;//与微信公众账号后台的Token设置保持一致，区分大小写。   

        private IUserAppService _userAppService;
        private IDepartmentAppService _departmentAppService;
        private IRoleAppService _roleAppService;

        private Dictionary<string, ISession> waitingSet;

        public WxMpController(IUserAppService userAppService, IDepartmentAppService departmentAppService, IRoleAppService roleAppService) {
            _userAppService = userAppService;
            _departmentAppService = departmentAppService;
            _roleAppService = roleAppService;

            if (waitingSet == null) {
                waitingSet = new Dictionary<string, ISession>();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult WxOauth2Login(string code) {
            var count = 0;
            while (count++<100) {
                Dictionary<string, object> item = (Dictionary<string, object>)CacheHelper.GetCacheValue($"WxMpLogin_{code}");
                
                if (item != null && item.TryGetValue("userInfo", out var userInfo) && item.TryGetValue("accessToken", out var accessToken)) {
                    var msg = "";
                    bool success = Login((userInfo as OAuthUserInfo).unionid, accessToken.ToString(), out msg);
                    if (msg == "InvalidUser") {
                        if (Register((userInfo as OAuthUserInfo))) {
                            success = Login((userInfo as OAuthUserInfo).unionid, accessToken.ToString(), out msg);
                        }
                        else {
                            success = false;
                            msg = "RegisterFail";
                        }
                    }
                    return Json(new { success = success });
                }
                Thread.Sleep(500);
            }
            return Json(new { success = false });
        }

        [HttpGet]
        [ActionName("Index")]
        [AllowAnonymous]
        public IActionResult Get(PostModel postModel, string echostr) {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token)) {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        private byte[] GetAvatar(string url) {
            WebRequest webRequest = WebRequest.Create(url);
            WebResponse webResponse = webRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();
            MemoryStream mem = new MemoryStream();
            BufferedStream bfs = new BufferedStream(stream);
            int len = 0;
            byte[] buffer = new byte[4096];
            do {
                len = bfs.Read(buffer, 0, buffer.Length);
                if (len > 0) mem.Write(buffer, 0, len);
            } while (len > 0);
            bfs.Close();
            byte[] picbytes = mem.ToArray();
            mem.Close();
            return picbytes;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetWxOauth2Url(string unionid, string returnUrl) {
            if (returnUrl.IsNullOrEmpty()) {
                returnUrl = "/Home";
            }
            var state = $"web,{unionid}";
            //HttpContext.Session.SetString("State", state);//储存随机数到Session
            var url = OAuthApi.GetAuthorizeUrl(appId,
                "http://" + HttpContext.Request.Host + "/WxMp/UserInfoCallback?returnUrl=" + returnUrl.UrlEncode(),
                state, OAuthScope.snsapi_userinfo);
            return Json(new { url });
        }

        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">用户最初尝试进入的页面</param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult UserInfoCallback(string code, string state, string returnUrl) {
            var msg = "";
            if (string.IsNullOrEmpty(code)) {
                return Json(new { success = false, msg = "您拒绝了授权！" });
            }

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try {
                result = OAuthApi.GetAccessToken(appId, appSecret, code);
            }
            catch (Exception ex) {
                return Json(new { success = false, msg = "错误：" + ex.Message });
            }
            if (result.errcode != ReturnCode.请求成功) {
                return Json(new { success = false, msg = "错误：" + result.errmsg });
            }
            
            try {
                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);


                CacheHelper.SetChacheValue($"WxMpLogin_{state.Split(",").GetValue(1).ToString()}", new Dictionary<string, object> { { "userInfo", userInfo }, { "accessToken", result.access_token } });

                if (state.Split(",").GetValue(0).ToString() == "webapp") {
                    bool success = Login(userInfo.unionid, result.access_token, out msg);
                    if (msg == "InvalidUser") {
                        if (Register(userInfo)) {
                            success = Login(userInfo.unionid, result.access_token, out msg);
                        }
                        else {
                            success = false;
                            msg = "RegisterFail";
                        }
                    }
                }
                return Redirect(returnUrl);
            }
            catch (ErrorJsonResultException ex) {
                return Content(ex.Message);
            }
        }
        
        private bool Login(string unionid, string accessToken, out string msg) {
            if (unionid == null) {
                msg = "AuthorizationFailure";
                return false;
            }
            var user = _userAppService.CheckWxUnionId(unionid);
            if (user != null) {
                HttpContext.Session.Clear();
                //记录Session
                HttpContext.Session.SetString("CurrentUserLoginType", "WxMp");
                HttpContext.Session.SetString("CurrentUserWxMpAccessToken", accessToken);
                HttpContext.Session.SetString("CurrentUserId", user.Id.ToString());
                HttpContext.Session.SetString("CurrentUserName", user.Name);
                HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(user));
                HttpContext.Session.Set("CurrentUserWithRoles", ByteConvertHelper.Object2Bytes(_userAppService.Get(user.Id)));
                msg = "OK";
                return true;

            }
            msg = "InvalidUser";
            return false;
        }

        private bool Register(OAuthUserInfo userInfo) {
            var bytes = GetAvatar(userInfo.headimgurl);
            Guid userId = Guid.NewGuid();
            var userRoles = new List<UserRoleDto>();
            userRoles.Add(new UserRoleDto() { UserId = userId, RoleId = _roleAppService.GetByCode("User_Lv1").Id });
            var user = new UserDto {
                Id = userId,
                UserRoles = userRoles,
                UserName = "user_" + TimeConvertHelper.GetTimeStamp(),
                Password = "",
                MobileNumber = "",
                Name = userInfo.nickname,
                DepartmentId = _departmentAppService.GetDefualt().Id,
                Avatar = bytes,
                AuthStatus = 0,
                WxUnionId = userInfo.unionid
            };
            return _userAppService.InsertOrUpdate(ref user);
        }
    }
}

