using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.AndroidapkApp;
using System.IO;
using System.Web;
using LsAdmin.Application.AndroidapkApp.Dtos;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.EquipmentApp;
using EquipmentService.WebAPI.Models;

namespace EquipmentService.WebAPI.Controllers {
    [Route("[controller]/[action]")]
    public class AndroidappController : Controller
    {
        private readonly string rootPath = "Files\\Androidapp\\";
        private readonly IAndroidapkAppService _service;
        private readonly IEquipmentAppService _equipmentService;
        public AndroidappController(IAndroidapkAppService service, IEquipmentAppService equipmentService) {
            _service = service;
            _equipmentService = equipmentService;
        }

        //[HttpPost]
        public JsonResult CheckAppUpgrade(string driverId) {
            var apps = _service.GetAllList().OrderByDescending(a => a.VersionCode).GroupBy(a => a.PackageName).Select(a => new AppInfoModel {
                VersionName = a.FirstOrDefault().VersionName,
                VersionCode = a.FirstOrDefault().VersionCode,
                PackageName = a.FirstOrDefault().PackageName,
                AppName = a.FirstOrDefault().AppName
            });
            if (apps == null) {
                return Json(new {
                    Success = false
                });
            }

            return Json(new {
                Success = true,
                Apps = apps
            });
        }
        
        public FileStreamResult DownloadApp(string driverId, string appPackageName) {
            var app = _service.GetAllList().Where(a => a.PackageName.Equals(appPackageName)).OrderByDescending(a => a.VersionCode).FirstOrDefault();
            if (app == null) {
                return null;
            }
            string fileName = app.Id + ".apk";
            var file = new FileStream(rootPath + fileName, FileMode.Open, FileAccess.Read);
            return File(file, "application/octet-stream", HttpUtility.UrlEncode(fileName));
        }

        public JsonResult AddVersion() {
            var item = new AndroidapkDto {
                VersionName = "0.0",
                VersionCode = 0,
                PackageName = "lsinfo.com.cn.lsmediaplayer",
                AppName = "播"
            };
            _service.InsertOrUpdate(ref item);
            return Json(new {
                Success = false
            });
        }
        
        public JsonResult RegisterDevice(string deviceId, string deviceName) {
            EquipmentDto dto = _equipmentService.GetByDeviceId(deviceId);
            if (dto == null) {
                dto = new EquipmentDto {
                    DeviceId = deviceId,
                    Name = deviceName
                };
                return Json(new  {
                    Success = _equipmentService.InsertOrUpdate(ref dto)
                });
            }
            return Json(new  {
                Success = true
            });
        }
    }
}
