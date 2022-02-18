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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class WxOpenController : Controller
    {
        //public static readonly string Token = Config.SenparcWeixinSetting.Token;//与微信公众账号后台的Token设置保持一致，区分大小写。
        //public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey;//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.WxOpenAppId;//与微信小程序账号后台的AppId设置保持一致，区分大小写。
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.WxOpenAppSecret;//与微信小程序账号后台的AppId设置保持一致，区分大小写。       

        private IUserAppService _userAppService;
        private IDepartmentAppService _departmentAppService;
        private IRoleAppService _roleAppService;
        public WxOpenController(IUserAppService userAppService, IDepartmentAppService departmentAppService, IRoleAppService roleAppService) {
            _userAppService = userAppService;
            _departmentAppService = departmentAppService;
            _roleAppService = roleAppService;
        }

        /*[HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterModel model) {
            var success = false;
            var msg = "";
            if (ModelState.IsValid) {
                DecodedUserInfo decodedEntity = ByteConvertHelper.Bytes2Object<DecodedUserInfo>(HttpContext.Session.Get("CurrentUserWxInfo"));
                if (decodedEntity == null) {
                    return Json(new { success = false, msg = "UserInfoInvalid" });
                }

                var user = _userAppService.CheckUser(model.UserName, model.Password);
                if (user != null) {
                    user.WxUnionId = decodedEntity.unionId;
                    success = _userAppService.InsertOrUpdate(ref user);
                    return Json(new { success, msg });
                }

                var bytes = GetAvatar(decodedEntity.avatarUrl);
                Guid userId = Guid.NewGuid();
                var userRoles = new List<UserRoleDto>();
                userRoles.Add(new UserRoleDto() { UserId = userId, RoleId = _roleAppService.GetByCode("User_Lv1").Id });
                user = new UserDto {
                    Id = userId,
                    UserRoles = userRoles,
                    UserName = model.UserName,
                    Password = model.Password,
                    MobileNumber = model.MobileNumber,
                    Name = decodedEntity.nickName,
                    DepartmentId = _departmentAppService.GetDefualt().Id,
                    Avatar = bytes,
                    AuthStatus = 0,
                    WxUnionId = decodedEntity.unionId
                };
                success = _userAppService.InsertOrUpdate(ref user);
                return Json(new { success, msg });
            }
            foreach (var item in ModelState.Values) {
                if (item.Errors.Count > 0) {
                    msg = item.Errors[0].ErrorMessage;
                    break;
                }
            }
            return Json(new { success = false, msg });
        }*/

        private DecodedUserInfo DecodeUserInfo(string sessionKey, string encryptedData, string iv) {
            var json = EncryptHelper.DecodeEncryptedData(
                        sessionKey,
                        encryptedData, iv);
            DecodedUserInfo decodedEntity = JsonConvert.DeserializeObject<DecodedUserInfo>(json, new LsJsonSerializerSettings());

            //检验水印
            var checkWartmark = false;
            if (decodedEntity != null) {
                checkWartmark = decodedEntity.CheckWatermark(WxOpenAppId);
            }            
            
            return checkWartmark ? decodedEntity : null;
        }

        /// <summary>
        /// wx.login登陆成功之后发送的请求
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult OnDirectLogin(string code) {
            HttpContext.Session.SetString("CurrentUserLoginType", "WxOpen");
            var jsonResult = SnsApi.JsCode2Json(WxOpenAppId, WxOpenAppSecret, code);
            if (jsonResult.errcode == ReturnCode.请求成功) {
                bool success = Login(jsonResult.unionid, jsonResult.session_key, out var msg);
                return Json(new { success, msg });
            }
            else {
                return Json(new { success = false, msg = jsonResult.errmsg });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult OnAuthLogin(string encryptedData, string iv) {
            HttpContext.Session.SetString("CurrentUserLoginType", "WxOpen");
            string sessionKey = HttpContext.Session.GetString("CurrentUserWxOpenSessionKey");
            if (string.IsNullOrEmpty(sessionKey)) {
                return Json(new { success = false, msg = "SessionInvalid" });
            }
            DecodedUserInfo decodedEntity = DecodeUserInfo(sessionKey, encryptedData, iv);
            if (decodedEntity == null) {
                return Json(new { success = false, msg = "UserInfoInvalid" });
            }
            string msg = "";
            bool success = false;
            success =  Login(decodedEntity.unionId, sessionKey, out msg);
            if (msg == "InvalidUser") {
                HttpContext.Session.Set("CurrentUserWxInfo", ByteConvertHelper.Object2Bytes(decodedEntity));
            }
            /*if (msg == "InvalidUser") {
                if (Register(decodedEntity)) {
                    success = Login(decodedEntity.openId, sessionKey, out msg);
                }
                else {
                    success = false;
                    msg = "RegisterFail";
                }
            }*/
            return Json(new { success, msg });
        }
        
        private bool Login(string unionid, string sessionKey, out string msg) {
            HttpContext.Session.SetString("CurrentUserWxOpenSessionKey", sessionKey);
            if (unionid == null) {
                msg = "AuthorizationFailure";
                return false;
            }
            var user = _userAppService.CheckWxUnionId(unionid);
            if (user != null) {
                HttpContext.Session.Clear();
                //记录Session
                HttpContext.Session.SetString("CurrentUserWxOpenSessionKey", sessionKey);
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

        /*private bool Register(DecodedUserInfo decodedEntity) {
            var bytes = GetAvatar(decodedEntity.avatarUrl);
            Guid userId = Guid.NewGuid();
            var userRoles = new List<UserRoleDto>();
            userRoles.Add(new UserRoleDto() { UserId = userId, RoleId = _roleAppService.GetByCode("User_Lv1").Id });
            var user = new UserDto {
                Id = userId,
                UserRoles = userRoles,
                UserName = "user_" + TimeConvertHelper.GetTimeStamp(),
                Password = "",
                MobileNumber = "",
                Name = decodedEntity.nickName,
                DepartmentId = _departmentAppService.GetDefualt().Id,
                Avatar = bytes,
                AuthStatus = 0,
                WxUnionId = decodedEntity.unionId
            };
            return _userAppService.InsertOrUpdate(ref user);
        }*/

        
    }
}

