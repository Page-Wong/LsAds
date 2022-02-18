using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.MVC.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NLog;
using System.IO;
using LsAdmin.Utility;
using Microsoft.AspNetCore.Http;
using LsAdmin.Utility.Auth;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class SystemController : LsAdminControllerBase
    {

        [Route("/System/Error/{errorCode}")]
        public IActionResult Error(string errorCode) {
            if (errorCode == "500" | errorCode == "404" | errorCode == "Auth") {
                return View($"{errorCode}");
            }

            return View("Error");
        }

        public IActionResult Error() {
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Logger.Error($"<h1>Error: {error.Error.Message}</h1>{error.Error.StackTrace }");
            return View();
        }

        public void KeepAlive() {

        }

        [HttpGet]
        public IActionResult GetAuthCode() {
            MemoryStream ms = AuthCodeHelper.CreateImg(out string authCode);
            HttpContext.Session.SetString("AuthCode", authCode);
            Response.Body.Dispose();
            return File(ms.ToArray(), @"image/png");
        }
    }
}
