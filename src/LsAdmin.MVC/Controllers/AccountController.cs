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
using Senparc.Weixin.WxOpen.Entities;
using System.Net;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class AccountController : Controller
    {
        private IUserAppService _userAppService;
        private IDepartmentAppService _departmentAppService;
        private IRoleAppService _roleAppService;
        public AccountController(IUserAppService userAppService, IDepartmentAppService departmentAppService, IRoleAppService roleAppService) {
            _userAppService = userAppService;
            _departmentAppService = departmentAppService;
            _roleAppService = roleAppService;
        }

        public IActionResult Index() {
            return View("Login");
        }

        // GET: /<controller>/
        public IActionResult Login() {
            HttpContext.Session.SetString("CurrentUserLoginType", "Web");
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //检查用户信息
                var user = _userAppService.CheckUser(model.UserName, model.Password);
                if (user != null)
                {
                    HttpContext.Session.Clear();
                    //记录Session
                    HttpContext.Session.SetString("CurrentUserId", user.Id.ToString());
                    HttpContext.Session.SetString("CurrentUserName", user.Name);
                    HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(user));
                    HttpContext.Session.Set("CurrentUserWithRoles", ByteConvertHelper.Object2Bytes(_userAppService.Get(user.Id)));
                    //跳转到系统首页
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ErrorInfo = "用户名或密码错误。";
                return View();
            }
            foreach (var item in ModelState.Values)
            {
                if (item.Errors.Count > 0)
                {
                    ViewBag.ErrorInfo = item.Errors[0].ErrorMessage;
                    break;
                }
            }
            return View(model);
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return Index();
        }

        #region 注册
        public IActionResult Register(string type) {
            return View(new RegisterModel { RegisterType = type});
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model) {
            var isValid = CheckRegister(model, out var msg, true);
            if (!isValid) {
                ViewBag.ErrorInfo = msg;
                return View(model);
            }
            var avatar = new FileStream("wwwroot\\images\\default-avatar.png", FileMode.Open, FileAccess.Read);
            byte[] bytes = new byte[avatar.Length];
            avatar.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            avatar.Seek(0, SeekOrigin.Begin);

            Guid userId = Guid.NewGuid();
            var userRoles = new List<UserRoleDto>();
            userRoles.Add(new UserRoleDto() { UserId = userId, RoleId = _roleAppService.GetByCode(model.RegisterType).Id });

            var user = new UserDto {
                Id = userId,
                UserRoles = userRoles,
                UserName = model.UserName,
                Password = model.Password,
                MobileNumber = model.MobileNumber,
                Name = model.UserName,
                DepartmentId = _departmentAppService.GetDefualt().Id,
                Avatar = bytes,
                AuthStatus = 0
            };
            if (_userAppService.InsertOrUpdate(ref user)) {
                user = _userAppService.CheckUser(user.UserName, user.Password);
                HttpContext.Session.Clear();
                //记录Session
                //HttpContext.Session.SetString("CurrentUserLoginType", "Web");
                HttpContext.Session.SetString("CurrentUserId", user.Id.ToString());
                HttpContext.Session.SetString("CurrentUserName", user.Name);
                HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(user));
                HttpContext.Session.Set("CurrentUserWithRoles", ByteConvertHelper.Object2Bytes(_userAppService.Get(user.Id)));
                //跳转到系统首页
                return RedirectToAction("Index", "Home");
            }
            else {
                ViewBag.ErrorInfo = "注册失败";
                return View(model);
            }

            /*var SendSmsCode_SmsCode = HttpContext.Session.GetString("SendSmsCode_SmsCode");
            var SendSmsCode_Number = HttpContext.Session.GetString("SendSmsCode_Number");
            HttpContext.Session.Remove("SendSmsCode_SmsCode");
            HttpContext.Session.Remove("SendSmsCode_Number");
            if (ModelState.IsValid) {
                //TODO 上线前取消注释，启用手机短信验证
                if (String.IsNullOrEmpty(model.SmsCode) || String.IsNullOrEmpty(SendSmsCode_SmsCode) || model.SmsCode != SendSmsCode_SmsCode) {
                    ViewBag.ErrorInfo = "手机验证码错误或过期";
                    return View(model);
                }
                if (String.IsNullOrEmpty(model.MobileNumber) || String.IsNullOrEmpty(SendSmsCode_Number) || model.MobileNumber != SendSmsCode_Number) {
                    ViewBag.ErrorInfo = "手机号码与验证的手机号码不符";
                    return View(model);
                }
                //检查用户信息
                var user = _userAppService.CheckUser(model.UserName);
                if (user != null) {
                    ViewBag.ErrorInfo = "用户 " + model.UserName + " 已存在";
                    return View();
                }
                user = _userAppService.CheckMobileNumber(model.MobileNumber);
                if (user != null) {
                    ViewBag.ErrorInfo = "手机号码 " + model.MobileNumber + " 已注册";
                    return View();
                }

                var avatar = new FileStream("wwwroot\\images\\default-avatar.png", FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[avatar.Length];
                avatar.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始
                avatar.Seek(0, SeekOrigin.Begin);

                Guid userId = Guid.NewGuid();
                var userRoles = new List<UserRoleDto>();
                //var type = ViewBag.RegisterType;
                userRoles.Add(new UserRoleDto() { UserId = userId, RoleId = _roleAppService.GetByCode(model.RegisterType).Id });
                
                user = new UserDto {
                    Id = userId,
                    UserRoles = userRoles,
                    UserName = model.UserName,
                    Password = model.Password,
                    MobileNumber = model.MobileNumber,
                    Name = model.UserName,
                    DepartmentId = _departmentAppService.GetDefualt().Id,
                    Avatar = bytes,
                    AuthStatus = 0                
                };
                if (_userAppService.InsertOrUpdate(ref user)) {
                    user = _userAppService.CheckUser(user.UserName, user.Password);
                    HttpContext.Session.Clear();
                    //记录Session
                    HttpContext.Session.SetString("CurrentUserLoginType", "Web");
                    HttpContext.Session.SetString("CurrentUserId", user.Id.ToString());
                    HttpContext.Session.SetString("CurrentUserName", user.Name);
                    HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(user));
                    HttpContext.Session.Set("CurrentUserWithRoles", ByteConvertHelper.Object2Bytes(_userAppService.Get(user.Id)));
                    //跳转到系统首页
                    return RedirectToAction("Index", "Home");
                }
                else {
                    ViewBag.ErrorInfo = "注册失败";
                    return View(model);
                }
            }
            foreach (var item in ModelState.Values) {
                if (item.Errors.Count > 0) {
                    ViewBag.ErrorInfo = item.Errors[0].ErrorMessage;
                    break;
                }
            }
            return View(model);*/
        }
        [HttpPost]
        public IActionResult WxRegister(RegisterModel model) {
            var isValid = CheckRegister(model, out var msg, false);
            if (!isValid) {
                return Json(new { success = false, msg });
            }

            var success = false;
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
            userRoles.Add(new UserRoleDto() { UserId = userId, RoleId = _roleAppService.GetByCode(model.RegisterType).Id });

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

        private bool CheckRegister(RegisterModel model, out string msg, bool isOnlyAdded) {
            msg = "";
            var SendSmsCode_SmsCode = HttpContext.Session.GetString("SendSmsCode_SmsCode");
            var SendSmsCode_Number = HttpContext.Session.GetString("SendSmsCode_Number");
            HttpContext.Session.Remove("SendSmsCode_SmsCode");
            HttpContext.Session.Remove("SendSmsCode_Number");
            if (ModelState.IsValid) {
                //TODO 上线前取消注释，启用手机短信验证
                if (String.IsNullOrEmpty(model.SmsCode) || String.IsNullOrEmpty(SendSmsCode_SmsCode) || model.SmsCode != SendSmsCode_SmsCode) {
                    msg = "手机验证码错误或过期";
                    return false;
                }
                if (String.IsNullOrEmpty(model.MobileNumber) || String.IsNullOrEmpty(SendSmsCode_Number) || model.MobileNumber != SendSmsCode_Number) {
                    msg = "手机号码与验证的手机号码不符";
                    return false;
                }

                if (isOnlyAdded || _userAppService.CheckUser(model.UserName, model.Password) == null ) {
                    var user = _userAppService.CheckUser(model.UserName);
                    if (user != null) {
                        msg = "用户 " + model.UserName + " 已存在";
                        return false;
                    }
                    user = _userAppService.CheckMobileNumber(model.MobileNumber);
                    if (user != null) {
                        msg = "手机号码 " + model.MobileNumber + " 已注册";
                        return false;
                    }
                }
            }
            else {
                foreach (var item in ModelState.Values) {
                    if (item.Errors.Count > 0) {
                        msg = item.Errors[0].ErrorMessage;
                        break;
                    }
                }
                return false;
            }
            return true;
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

        [HttpPost]
        public IActionResult CheckMobileNumber(string mobileNumber) {
            var user = _userAppService.CheckMobileNumber(mobileNumber);
            if (user != null) {
                return Json(new {
                    Result = "Faild",
                    Message = "手机号码 " + mobileNumber + " 已注册"
                });
            }
            return Json(new {
                Result = "Success"
            });
        }
        #endregion

        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ForgetPassword(ForgetPasswordViewModel model) {
            var SendSmsCode_SmsCode = HttpContext.Session.GetString("SendSmsCode_SmsCode");
            var SendSmsCode_Number = HttpContext.Session.GetString("SendSmsCode_Number");
            HttpContext.Session.Remove("SendSmsCode_SmsCode");
            HttpContext.Session.Remove("SendSmsCode_Number");
            if (ModelState.IsValid)
            {
                //TODO 上线前取消注释，启用手机短信验证
                if (String.IsNullOrEmpty(model.SmsCode) || String.IsNullOrEmpty(SendSmsCode_SmsCode) || model.SmsCode != SendSmsCode_SmsCode) {
                    ViewBag.ErrorInfo = "手机验证码错误或过期";
                    return View(model);
                }
                if (String.IsNullOrEmpty(model.MobileNumber) || String.IsNullOrEmpty(SendSmsCode_Number) || model.MobileNumber != SendSmsCode_Number) {
                    ViewBag.ErrorInfo = "手机号码与验证的手机号码不符";
                    return View(model);
                }
                var user = _userAppService.CheckMobileNumber(model.MobileNumber);
                if (user == null) {
                    ViewBag.ErrorInfo = "此手机号码未注册";
                    return View(model);
                }               
                if (!_userAppService.ChangePassword(user, model.Password)) {
                    ViewBag.ErrorInfo = "密码重置失败";
                    return View(model);
                }
                //跳转到系统登录页
                return RedirectToAction("Login", "Account");
            }
            foreach (var item in ModelState.Values) {
                if (item.Errors.Count > 0) {
                    ViewBag.ErrorInfo = item.Errors[0].ErrorMessage;
                    break;
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult GetAuthCode() {
            MemoryStream ms = AuthCodeHelper.CreateImg(out string authCode);
            HttpContext.Session.SetString("AuthCode", authCode);
            Response.Body.Dispose();
            return File(ms.ToArray(), @"image/png");
        }

        [HttpPost]
        public IActionResult SendSmsCode(string phoneNumber, string authCode) {
            var code = HttpContext.Session.GetString("AuthCode");
            HttpContext.Session.Remove("AuthCode");
            if (HttpContext.Session.GetString("CurrentUserLoginType") == "Web" && (String.IsNullOrEmpty(authCode) || String.IsNullOrEmpty(code) || authCode.ToLower() != code.ToLower())) {
                return Json(new {
                    Result = "Faild",
                    Message = "图形验证码错误"
                });
            }
            if (String.IsNullOrEmpty(phoneNumber)) {
                return Json(new {
                    Result = "Faild",
                    Message = "电话号码不能为空"
                });
            }
            string smsCode =  new Random().Next(100000, 1000000).ToString();
            HttpContext.Session.SetString("SendSmsCode_SmsCode", smsCode);
            HttpContext.Session.SetString("SendSmsCode_Number", phoneNumber);

            //TODO 上线前取消注释，启用手机短信验证
            SmsSingleSenderResult result = new SmsSingleSenderResult();
            SmsSingleSender sender = new SmsSingleSender();
            result = sender.SendCode(phoneNumber.Replace(" ", ""), smsCode);
            if (result.result != 0) {
                return Json(new {
                    Result = "Faild",
                    Message = "验证码发送出错"
                });
            }
            return Json(new {
                Result = "Success"
            }); ;
        }
    }
}

