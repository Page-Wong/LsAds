using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.UserApp;
using LsAdmin.Application.RoleApp;
using LsAdmin.MVC.Models;
using System.IO;
using LsAdmin.Utility.Convert;
using LsAdmin.Utility.SMS;
using LsAdmin.Utility;
using System;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.RoleApplyApp.Dtos;
using LsAdmin.Application.RoleApplyApp;
using LsAdmin.Utility.Auth;
using LsAdmin.Utility.Mail;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers {
    public class CurrentUserController : LsAdminControllerBase
    {
        #region 变量和构造函数
        private readonly IUserAppService _service;
        private readonly IRoleAppService _roleService;
        private readonly IRoleApplyAppService _roleApplyService;
        public CurrentUserController(IUserAppService service, IRoleAppService roleService, IRoleApplyAppService roleApplyService)
        {
            _service = service;
            _roleService = roleService;
            _roleApplyService = roleApplyService;
        }
        #endregion

        #region 实名认证
        [HttpGet]
        public IActionResult Auth() {
            return View();
        }

        [HttpGet]
        public IActionResult AuthAuditing() {
            return View();
        }
        #endregion

        #region 修改手机号码
        [HttpGet]
        public IActionResult ChangeMobileNumber() {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeMobileNumber(string phoneNumber, string password, string smsCode) {
            var SendSmsCode_SmsCode = HttpContext.Session.GetString("SendSmsCode_SmsCode");
            var SendSmsCode_Number = HttpContext.Session.GetString("SendSmsCode_Number");
            HttpContext.Session.Remove("SendSmsCode_SmsCode");
            HttpContext.Session.Remove("SendSmsCode_Number");
            if (String.IsNullOrEmpty(password) || PasswordConvertHelper.Create(password) != CurrentUser.Password) {
                return Json(new {
                    Result = "Faild",
                    Message = "密码错误"
                });
            }
            if (_service.CheckMobileNumber(phoneNumber) != null) {
                return Json(new {
                    Result = "Faild",
                    Message = phoneNumber + " 已被用户注册"
                });
            }
            //TODO 20171220 G 上线前取消注释，启用手机短信验证
            if (String.IsNullOrEmpty(smsCode) || String.IsNullOrEmpty(SendSmsCode_SmsCode) || smsCode != SendSmsCode_SmsCode) {
                return Json(new {
                    Result = "Faild",
                    Message = "手机验证码错误或过期"
                });
            }
            if (String.IsNullOrEmpty(phoneNumber) || String.IsNullOrEmpty(SendSmsCode_Number) || phoneNumber != SendSmsCode_Number) {
                return Json(new {
                    Result = "Faild",
                    Message = "手机号码与验证的手机号码不符"
                });
            }
            CurrentUser.MobileNumber = phoneNumber;
            if (_service.Update(CurrentUser)) {
                CurrentUser = CurrentUser;
                //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                return Json(new {
                    Result = "Success"
                });
            }
            return Json(new {
                Result = "Faild",
                Message = "更新出错"
            });
        }

        [HttpPost]
        public IActionResult SendChangeMobileNumberSmsCode(string phoneNumber, string authCode, string password) {
            var code = HttpContext.Session.GetString("AuthCode");
            HttpContext.Session.Remove("AuthCode");
            if (String.IsNullOrEmpty(authCode) || String.IsNullOrEmpty(code) || authCode.ToLower() != code.ToLower()) {
                return Json(new {
                    Result = "Faild",
                    Message = "图形验证码错误"
                });
            }
            if (String.IsNullOrEmpty(password) || PasswordConvertHelper.Create(password) != CurrentUser.Password) {
                return Json(new {
                    Result = "Faild",
                    Message = "密码错误"
                });
            }
            if (_service.CheckMobileNumber(phoneNumber) != null) {
                return Json(new {
                    Result = "Faild",
                    Message = phoneNumber + " 已被用户注册"
                });
            }
            string smsCode = new Random().Next(100000, 1000000).ToString();
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
                }); ;
            }
            return Json(new {
                Result = "Success"
            }); ;

        }
        #endregion

        #region 修改邮箱地址
        [HttpGet]
        public IActionResult ChangeEMail() {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeEMail(string email, string password, string mailCode) {
            var SendMailCode_MailCode = HttpContext.Session.GetString("SendMailCode_MailCode");
            var SendMailCode_Address = HttpContext.Session.GetString("SendMailCode_Address");
            HttpContext.Session.Remove("SendMailCode_MailCode");
            HttpContext.Session.Remove("SendMailCode_Address");
            if (String.IsNullOrEmpty(password) || PasswordConvertHelper.Create(password) != CurrentUser.Password) {
                return Json(new {
                    Result = "Faild",
                    Message = "密码错误"
                });
            }
            if (_service.CheckEMail(email) != null) {
                return Json(new {
                    Result = "Faild",
                    Message = email + " 已被用户注册"
                });
            }
            if (String.IsNullOrEmpty(mailCode) || String.IsNullOrEmpty(SendMailCode_MailCode) || mailCode.ToLower() != SendMailCode_MailCode.ToLower()) {
                return Json(new {
                    Result = "Faild",
                    Message = "邮箱验证码错误"
                });
            }
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(SendMailCode_Address) || email != SendMailCode_Address) {
                return Json(new {
                    Result = "Faild",
                    Message = "邮箱地址与验证的邮箱地址不符"
                });
            }
            CurrentUser.EMail = email;
            if (_service.Update(CurrentUser)) {
                CurrentUser = CurrentUser;
                //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                return Json(new {
                    Result = "Success"
                });
            }
            return Json(new {
                Result = "Faild",
                Message = "更新出错"
            });
        }

        [HttpPost]
        public IActionResult SendMailCode(string email, string password) {
            if (String.IsNullOrEmpty(password) || PasswordConvertHelper.Create(password) != CurrentUser.Password) {
                return Json(new {
                    Result = "Faild",
                    Message = "密码错误"
                });
            }

            if (_service.CheckEMail(email) != null) {
                return Json(new {
                    Result = "Faild",
                    Message = email + " 已被用户注册"
                });
            }
            var mailCode = AuthCodeHelper.RndNum(6);
            var tips = "<div style='font - size:14px; font - family:arial,SimSun; font - weight:bold; padding: 40px 0 25px; '>亲爱的用户： "+CurrentUser.Name+" 你好！</div><h4>你的邮箱验证码为：" + mailCode + "</h4>";
            var info = @"<hr><div style = 'clear:both;' ><div style = 'margin:.0px;padding:.0px;border:.0px;outline:.0px;color:#000000;font-family:Tahoma,Arial;font-size:14.0px;font-style:normal;font-variant-ligatures:normal;font-variant-caps:normal;font-weight:normal;text-align:start;text-indent:.0px;text-transform:none;widows:1;line-height:2.0em;clear:both;' ><span style = 'margin:.0px;padding:.0px;border:.0px;outline:.0px;color:#7030a0;font-family:楷体,楷体_GB2312;font-size:32.0px;' >东莞泷晟信息科技有限公司 </span ></div ><div class='Normal_text' style='margin:.0px;padding:.0px;border:.0px;outline:.0px;color:#000000;font-family:Tahoma,Arial;font-size:14.0px;font-style:normal;font-variant-ligatures:normal;font-variant-caps:normal;font-weight:normal;line-height:23.3px;text-align:start;text-indent:.0px;text-transform:none;widows:1;'><span style = 'margin:.0px;padding:.0px;border:.0px;outline:.0px;font-family:宋体;font-size:15.0px;' > 地址：东莞市东城区东纵路万达广场B区6栋2210号</span></div><div class='Normal_text' style='margin:.0px;padding:.0px;border:.0px;outline:.0px;color:#000000;font-family:Tahoma,Arial;font-size:14.0px;font-style:normal;font-variant-ligatures:normal;font-variant-caps:normal;font-weight:normal;line-height:23.3px;text-align:start;text-indent:.0px;text-transform:none;widows:1;'><span style = 'margin:.0px;padding:.0px;border:.0px;outline:.0px;font-family:宋体;font-size:15.0px;' > 电话：0769-86610866</span><br><span style = 'margin:.0px;padding:.0px;border:.0px;outline:.0px;font-family:宋体;font-size:15.0px;' > 邮编：523219</span></div><div class='Normal_text' style='margin:.0px;padding:.0px;border:.0px;outline:.0px;color:#000000;font-family:Tahoma,Arial;font-size:14.0px;font-style:normal;font-variant-ligatures:normal;font-variant-caps:normal;font-weight:normal;line-height:23.3px;text-align:start;text-indent:.0px;text-transform:none;widows:1;'><span style = 'margin:.0px;padding:.0px;border:.0px;outline:.0px;font-family:宋体;font-size:15.0px;' > 网址：www.lsinfo.com.cn</span></div></div>";
            MailHelper.Send(email, "邮箱验证", tips + info);
            HttpContext.Session.SetString("SendMailCode_MailCode", mailCode);
            HttpContext.Session.SetString("SendMailCode_Address", email);

            return Json(new {
                Result = "Success"
            }); ;

        }
        #endregion

        #region 修改登录密码
        [HttpGet]
        public IActionResult ChangePassword() {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model) {
            var code = HttpContext.Session.GetString("AuthCode");
            HttpContext.Session.Remove("AuthCode");
            if (ModelState.IsValid) {
                if (String.IsNullOrEmpty(model.AuthCode) || String.IsNullOrEmpty(code) || model.AuthCode.ToLower() != code.ToLower()) {
                    return Json(new {
                        Result = "Faild",
                        Message = "图形验证码错误"
                    });
                }
                if (PasswordConvertHelper.Create(model.OldPassword) != CurrentUser.Password) {
                    return Json(new {
                        Result = "Faild",
                        Message = "旧密码错误"
                    });
                }
                if (_service.ChangePassword(CurrentUser, model.NewConfirmPassword)) {
                    HttpContext.Session.Clear();
                    //跳转到系统登录页
                    return RedirectToAction("Login", "Account");
                }
            }
            return Json(new {
                Result = "Faild",
                Message = GetModelStateError()
            });
        }
        #endregion

        #region 修改支付密码
        [HttpGet]
        public IActionResult ChangePaymentPassword() {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePaymentPassword(ChangePaymentPasswordViewModel model) {
            var SendSmsCode_SmsCode = HttpContext.Session.GetString("SendSmsCode_SmsCode");
            var SendSmsCode_Number = HttpContext.Session.GetString("SendSmsCode_Number");
            HttpContext.Session.Remove("SendSmsCode_SmsCode");
            HttpContext.Session.Remove("SendSmsCode_Number");
            if (ModelState.IsValid) {
                //TODO 20171220 G 上线前取消注释，启用手机短信验证
                if (String.IsNullOrEmpty(model.SmsCode) || String.IsNullOrEmpty(SendSmsCode_SmsCode) || model.SmsCode != SendSmsCode_SmsCode) {
                    return Json(new {
                        Result = "Faild",
                        Message = "手机验证码错误或过期"
                    });
                }
                if (String.IsNullOrEmpty(CurrentUser.MobileNumber) || String.IsNullOrEmpty(SendSmsCode_Number) || CurrentUser.MobileNumber != SendSmsCode_Number) {
                    return Json(new {
                        Result = "Faild",
                        Message = "手机号码与验证的手机号码不符"
                    });
                }
                if (_service.ChangePaymentPassword(CurrentUser, model.NewConfirmPassword)) {
                    CurrentUser = CurrentUser;
                    //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                    return Json(new {
                        Result = "Success"
                    });
                }
            }
            return Json(new {
                Result = "Faild",
                Message = GetModelStateError()
            });
        }

        [HttpPost]
        public IActionResult SendChangePaymentPasswordSmsCode(string authCode) {
            var code = HttpContext.Session.GetString("AuthCode");
            HttpContext.Session.Remove("AuthCode");
            if (String.IsNullOrEmpty(authCode) || String.IsNullOrEmpty(code) || authCode.ToLower() != code.ToLower()) {
                return Json(new {
                    Result = "Faild",
                    Message = "图形验证码错误"
                });
            }
            string smsCode = new Random().Next(100000, 1000000).ToString();
            HttpContext.Session.SetString("SendSmsCode_SmsCode", smsCode);
            HttpContext.Session.SetString("SendSmsCode_Number", CurrentUser.MobileNumber);

            //TODO 上线前取消注释，启用手机短信验证
            SmsSingleSenderResult result = new SmsSingleSenderResult();
            SmsSingleSender sender = new SmsSingleSender();
            result = sender.SendCode(CurrentUser.MobileNumber, smsCode);
            if (result.result != 0) {
                return Json(new {
                    Result = "Faild",
                    Message = "验证码发送出错"
                }); ;
            }
            return Json(new {
                Result = "Success"
            });
        }
        #endregion

        #region 修改姓名
        [HttpPost]
        public IActionResult ChangeName(string name) {
            CurrentUser.Name = name;
            if (_service.Update(CurrentUser)) {
                CurrentUser = CurrentUser;
                //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                return Json(new {
                    Result = "Success"
                });
            }
            return Json(new {
                Result = "Faild",
                Message = "更新出错"
            });
        }
        #endregion

        #region 个人资料
        [HttpGet]
        public IActionResult Profile() {
            var user = _service.Get(CurrentUser.Id);
            return View(new ProfileModel {
                Name = user.Name,
                UserName = user.UserName,
                EMail = user.EMail,
                MobileNumber = user.MobileNumber,
                Avatar = user.Avatar,
                AuthStatus = user.AuthStatus,
                RoleNames = user.UserRoles.Select(item => item.Role.Name).ToArray()
            });
        }

        [HttpPost]
        public IActionResult Profile(ProfileModel user) {
            CurrentUser.Name = user.Name;
            CurrentUser.EMail = user.EMail;
            if (_service.Update(CurrentUser)) {
                CurrentUser = CurrentUser;
                //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
            }
            return Profile();
        }

        [HttpPost]
        public IActionResult UploadAvatar() {
            string[] allowExtension = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };
            var file = HttpContext.Request.Form.Files["file"];
            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowExtension.Contains(extension)) {
                return Json(new {
                    Result = "Faild",
                    Message = "仅支持JPG、GIF、PNG、JPEG、BMP格式"
                });
            }
            var avatar = file.OpenReadStream();
            if (avatar.Length > 1024*1024*4) {
                return Json(new {
                    Result = "Faild",
                    Message = "图片不得大于4M"
                });
            }
            byte[] bytes = new byte[avatar.Length];
            avatar.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            avatar.Seek(0, SeekOrigin.Begin);

            CurrentUser.Avatar = bytes;
            if (_service.Update(CurrentUser)) {
                CurrentUser = CurrentUser;
                //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                return Json(new {
                    Result = "Success"
                });
            }
            return Json(new {
                Result = "Faild"
            });
        }
        #endregion

        #region 验证码处理
        /*[HttpGet]
        public IActionResult GetAuthCode() {
            MemoryStream ms = AuthCodeHelper.CreateImg(out string authCode);
            HttpContext.Session.SetString("AuthCode", authCode);
            Response.Body.Dispose();
            return File(ms.ToArray(), @"image/png");
        }  */

        #endregion


        #region 角色申请、审核
        public IActionResult ApplyRole(string role) {
            var roleDto = _roleService.GetByCode(role);
            if (roleDto == null) {
                return Json(new { Result = "Faild", Message = "申请失败" });
            }
            var roleApply = _roleApplyService.GetListByApplyUserId(CurrentUser.Id)?.Where(item => item.RoleId == roleDto.Id && item.Status != RoleApplyDto.STATUS_UNPASS).ToList();
            if (roleApply?.Count > 0) {
                if (roleApply.Where(item => item.Status == RoleApplyDto.STATUS_AUDITTING).Count() > 0) {
                    return Json(new { Result = "Faild", Message = "正在审核中，请耐心等待" });
                }
                if (roleApply.Where(item => item.Status == RoleApplyDto.STATUS_PASS).Count() > 0) {
                    _service.AddRole(CurrentUser.Id, roleDto.Id);
                    return Json(new { Result = "Faild", Message = "你已拥有此权限，请重新登录后使用" });
                }
            }
            var roleApplyDto = new RoleApplyDto {
                RoleId = roleDto.Id,
                ApplyUserId = CurrentUser.Id,
                ApplyTime = DateTime.Now,
                Status = RoleApplyDto.STATUS_AUDITTING
            };
            if (_roleApplyService.Insert(ref roleApplyDto)) {
                return Json(new { Result = "Success", Message = "申请已提交，稍后会有专人与你联系，如需帮助，请联系客服" });
            }
            return Json(new { Result = "Faild", Message = "申请失败，请联系客服" });
        }
        #endregion
    }
}
