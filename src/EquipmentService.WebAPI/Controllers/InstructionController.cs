using Microsoft.AspNetCore.Mvc;
using System;
using ActiveEquipment.Application.DataModel;
using ActiveEquipment.Application.InstructionApp;
using ActiveEquipment.Application.ActiveEquipmentApp;
using System.Linq;
using ActiveEquipment.Application.Common;
using LsAdmin.Application.PlayerApp;
using EquipmentService.WebAPI.Models;
using LsAdmin.Application.AndroidapkApp;
using System.IO;
using System.Web;
using System.Collections.Generic;
using LsAdmin.Utility.Security;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.FilesApp;
using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Application.ProgramApp;
using LsAdmin.Domain.Entities;
using static LsAdmin.Application.EquipmentApp.Dtos.ActiveEquipmentDto;
using LsAdmin.Utility.Convert;

namespace EquipmentService.WebAPI.Controllers
{
    // For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

    public class InstructionController : Controller, IInstructionController {

        private IInstructionAppHandler _instructionAppHandler { get; set; }
        private IActiveEquipmentAppService _activeEquipmentAppService { get; set; }
        private IInstructionMethodAppService _instructionMethodAppService { get; set; }
        private IPlayerAppService _equipmentPlayerAppService { get; set; }
        private IAndroidapkAppService _androidapkAppService { get; set; }
        private IEquipmentAppService _equipmentAppService { get; set; }
        private IFilesAppService _filesAppService { get; set; }
        private IProgramAppService _programAppService { get; set; }

        public InstructionController(IAndroidapkAppService androidapkAppService, IPlayerAppService equipmentPlayerAppService, IInstructionAppHandler instructionAppHandler,
                                     IActiveEquipmentAppService activeEquipmentAppService, IInstructionMethodAppService instructionMethodAppService,
                                     IEquipmentAppService equipmentAppService, IFilesAppService filesAppService, IProgramAppService programAppService) {
            _instructionAppHandler = instructionAppHandler;
            _activeEquipmentAppService = activeEquipmentAppService;
            _instructionMethodAppService = instructionMethodAppService;
            _equipmentPlayerAppService = equipmentPlayerAppService;
            _androidapkAppService = androidapkAppService;
            _equipmentAppService = equipmentAppService;
            _filesAppService = filesAppService;
            _programAppService = programAppService;
        }
        private Result CheckParams(out ActiveEquipmentDto activeEquipment, string token, long timestamp, string sign, IDictionary<string, object> paramMaps, bool hasFile = false) {
            var result = new Result { Code = Result.ResultCode.SUCCESS };
            activeEquipment = _activeEquipmentAppService.GetByToken(token);
            if (activeEquipment == null) {
                result.Code = Result.ResultCode.ACTIVE_EQUIPMENT_NONE;
                return result;
            }
            paramMaps.TryAdd("timestamp", timestamp.ToString());

            if (Request.Query.ContainsKey("instructionId")) {
                paramMaps.TryAdd("instructionId", Request.Query["instructionId"].ToString());
            }
            else if (Request.Method == "POST" && Request.Form.TryGetValue("instructionId", out var instructionId)) {
                paramMaps.TryAdd("instructionId", instructionId.ToString());
            }
            if (hasFile) {
                paramMaps.TryAdd("fileMd5", Md5Helper.MD5Encrypt(paramMaps["file"] as byte[], activeEquipment.DeviceId));
                paramMaps.Remove("file");
            }
            if (sign != SignHelper.Sign(paramMaps, token, activeEquipment.DeviceId)) {
                result.Code = Result.ResultCode.SIGN_MISMATCH;
                return result;
            }
            return result;
        }

        //[HttpPost]
        public JsonResult InstructionDefaultNotify(InstructionResultDto dto) {
            var result = new Result();
            try {
                result = _instructionAppHandler.ReceiveInstructionResult(dto);
            }
            catch (Exception e) {
                result.Code = Result.ResultCode.RECEIVE_INSTRUCTION_RESULT_ERROR;
                result.Exception = e;
            }
            return Json(result);
        }

        public JsonResult InstructionOriginalNotify(OriginalInstructionNotifyDto dto) {
            var result = new Result();
            try {
                result = _instructionAppHandler.ReceiveOriginalInstructionNotify(dto);
            }
            catch (Exception e) {
                result.Code = Result.ResultCode.RECEIVE_ORIGINAL_INSTRUCTION_NOTIFY_ERROR;
                result.Exception = e;
            }
            return Json(result);
        }

        public JsonResult SystemErrorNotify(string log, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() { { "log", log } });
            if (result.IsSuccess()) {
                LogHelper.Error(new ActiveEquipmentLog {
                    Token = token,
                    Type = ActiveEquipmentLog.ActiveEquipmentLogType.Received,
                    Ip = activeEquipment.Ip,
                    Port = activeEquipment.Port,
                    Message = log
                });
                return Json(new EquipmentResult { Code = "1"});
            }
            return Json(new EquipmentResult { Code = "0" });
        }

        public JsonResult SyncPlayInfoList(string token, long timestamp, string sign) {

            try { 
                var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>());
                if (!result.IsSuccess())
                {
                    return Json(new EquipmentResult
                    {
                        Code = result.Code.ToString(),
                        Msg = result.Msg
                    });
                }

                var playerPlayInfos = _equipmentAppService.GetDowmloadPlayerProgramByEquipmentId(activeEquipment.EquipmentId).OrderBy(o => o.CreateTime); ;

                List<ProgramDto> programs = new List<ProgramDto>();

                //播放器的节目排序
                foreach (var playerPlayInfo in playerPlayInfos){
                    //播放器的节目排序
                    playerPlayInfo.Sort = playerPlayInfos.Where(w => w.PlayerId == playerPlayInfo.PlayerId).Max(m => m.Sort) + 1;

                    //将设备节目清单装载到 programs里。
                    if (programs.Where(w => w.Id == playerPlayInfo.Program.Id).FirstOrDefault() == null)
                        programs.Add(playerPlayInfo.Program);
                }

                programs.Distinct();

                programs.ForEach(item => item.Launcher = _programAppService.GetLauncher(item.Id));
       
                return Json(new
                {
                    code = result.Code,
                    playInfo = programs.Select(s => new { playInfoId = s.Id, launcher=s.Launcher, duration = s.Duration, fileMd5 = Md5Helper.MD5Encrypt(_filesAppService.GetOwnerOneObj(s.Id)?.MD5, activeEquipment.DeviceId) , type = s.Type.ToString() }),
                    playerPlayInfo= playerPlayInfos.Select(s => new { playInfoId = s.ProgramId, playerId =s.PlayerId, sort = s.Sort } )
                });
            }catch(Exception ex)
            {
                return Json(new EquipmentResult
                {
                    Code = Result.ResultCode.NONE.ToString(),
                    Msg = ex.Message
                });
            }
        }

        public JsonResult SyncPlayerList(string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>());
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            var items = _equipmentPlayerAppService.GetByEquipmentId(activeEquipment.EquipmentId)?.Select(it => new {
                playerId = it.Id,
                width = it.Width,
                height = it.Height,
                x = it.X,
                y = it.Y,
                sort = it.Sort
            });
            return Json(new {
                code = result.Code,
                dataList = items
            });
        }

        public JsonResult SyncPlayInfoResourcesList(string token, long timestamp, string sign) {
           try { 
                var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>());
                if (!result.IsSuccess())
                {
                    return Json(new EquipmentResult
                    {
                        Code = result.Code.ToString(),
                        Msg = result.Msg
                    });
                }
                return Json(new
                {
                    code = result.Code,
                    dataList = _equipmentAppService.GetDowmloadPlayInfoByEquipmentId(activeEquipment.EquipmentId).Select(s => new { playInfoId = s.Id, fileMd5 = Md5Helper.MD5Encrypt(_filesAppService.GetOwnerOneObj(s.Id)?.MD5, activeEquipment.DeviceId) }),
                });
            }
            catch (Exception ex)
            {
                return Json(new EquipmentResult
                {
                    Code = Result.ResultCode.NONE.ToString(),
                    Msg = ex.Message
                });
            }

        }

        public JsonResult SyncOperationDictionary(string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>());

            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }

            var methods = _instructionMethodAppService.GetAllList()?.Select(it => new { key = it.Id, method = it.Method, type = 1 });

            return Json(new {
                code = result.Code,
                version = "1.0",
                dataList = methods
            });
        }

        public JsonResult SyncAlarm(string token, long timestamp, string sign) {
            throw new NotImplementedException();
        }

        public JsonResult PostAppVersion(string versionName, string packageName, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() { { "versionName", versionName }, { "packageName", packageName } });
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            if (activeEquipment.AppVersion == null) {
                activeEquipment.AppVersion = new AppVersionDto();
            }
            activeEquipment.AppVersion.VersionName = versionName;
            activeEquipment.AppVersion.PackageName = packageName;
            _activeEquipmentAppService.Update(activeEquipment);
            return Json(result);
        }

        public JsonResult PostSystemInfo(EquipmentInfoDto equipmentInfo, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, ObjectToDictionaryHelper.ToDictionary(equipmentInfo));
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            activeEquipment.EquipmentInfo = equipmentInfo;
            _activeEquipmentAppService.Update(activeEquipment);
            return Json(result);
        }

        public JsonResult PostNetworkInfo(EquipmentNetworkInfoDto equipmentNetworkInfo, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, ObjectToDictionaryHelper.ToDictionary(equipmentNetworkInfo));
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            activeEquipment.NetworkInfo = equipmentNetworkInfo;
            _activeEquipmentAppService.Update(activeEquipment);
            return Json(result);
        }

        public JsonResult PostSDCardInfo(string sdCardTotalMemory, string sdCardAvailableMemory, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() { { "sdCardTotalMemory", sdCardTotalMemory }, { "sdCardAvailableMemory", sdCardAvailableMemory } });
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            if (activeEquipment.EquipmentInfo == null) {
                activeEquipment.EquipmentInfo = new EquipmentInfoDto();
            }
            if (activeEquipment.RealtimeInfo == null) {
                activeEquipment.RealtimeInfo = new RealtimeInfoDto();
            }
            activeEquipment.EquipmentInfo.SdCardTotalMemory = sdCardTotalMemory;
            activeEquipment.RealtimeInfo.SdCardAvailableMemory = sdCardAvailableMemory;
            _activeEquipmentAppService.Update(activeEquipment);
            return Json(result);
        }

        public JsonResult PostRamInfo(string totalRam, string availableRam, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() { { "totalRam", totalRam }, { "availableRam", availableRam } });
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            if (activeEquipment.EquipmentInfo == null) {
                activeEquipment.EquipmentInfo = new EquipmentInfoDto();
            }
            if (activeEquipment.RealtimeInfo == null) {
                activeEquipment.RealtimeInfo = new RealtimeInfoDto();
            }
            activeEquipment.EquipmentInfo.TotalRam = totalRam;
            activeEquipment.RealtimeInfo.AvailableRam = availableRam;
            _activeEquipmentAppService.Update(activeEquipment);
            return Json(result);
        }

        public JsonResult CheckUpgrade(string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>());

            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            var apps = _androidapkAppService.GetAllList().OrderByDescending(a => a.VersionCode).GroupBy(a => a.PackageName).Select(a => new AppInfoModel {
                Id = a.FirstOrDefault().Id.ToString(),
                VersionName = a.FirstOrDefault().VersionName,
                VersionCode = a.FirstOrDefault().VersionCode,
                PackageName = a.FirstOrDefault().PackageName,
                AppName = a.FirstOrDefault().AppName
            });

            if (apps == null) {
                result.Code = Result.ResultCode.APP_NONE;
                return Json(new {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }

            return Json(new {
                Code = result.Code,
                DataList = apps
            });
        }

        public JsonResult UpgradeNotify(string appId, bool success, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() {  { "appId", appId },{ "success", success.ToString() } });

            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            return Json(result);
        }

        public FileStreamResult DownloadApp(string id, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() { { "id", id } });

            if (!result.IsSuccess()) {
                return null;
            }
            var app = _androidapkAppService.Get(Guid.Parse(id));
            if (app == null) {
                return null;
            }
            string fileName = app.Id + ".apk";
            var file = new FileStream("Files\\Androidapp\\" + fileName, FileMode.Open, FileAccess.Read);
            return File(file, "application/octet-stream", HttpUtility.UrlEncode(fileName));
        }

        [HttpPost]
        public JsonResult ScreenshotUpload(string token, long timestamp, string sign) {
            var file = HttpContext.Request.Form.Files["file"];
            string extension = Path.GetExtension(file.FileName).ToLower();
            var avatar = file.OpenReadStream();
            byte[] bytes = new byte[avatar.Length];
            avatar.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            avatar.Seek(0, SeekOrigin.Begin);

            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object> { { "file", bytes } }, true);

            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }

            activeEquipment.LastScreenshot = bytes;
            activeEquipment.LastScreenshotTime = DateTime.Now;
            _activeEquipmentAppService.Update(activeEquipment);

            return Json(new {
                Code = result.Code
            });
        }

        public JsonResult LogUpload(string token, long timestamp, string sign) {
            var file = HttpContext.Request.Form.Files["file"];
            string extension = Path.GetExtension(file.FileName).ToLower();
            var avatar = file.OpenReadStream();
            byte[] bytes = new byte[avatar.Length];
            avatar.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            avatar.Seek(0, SeekOrigin.Begin);

            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object> { { "file", bytes } }, true);

            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            var logFile = new FilesInfoDto {
                FilenameExtension = "zip",
                MD5 = Md5Helper.MD5Encrypt(bytes),
                Name = file.FileName,
                OwnerObjId = activeEquipment.EquipmentId,
                File = bytes
            };
            if (!_equipmentAppService.AddLogFile(activeEquipment.EquipmentId, logFile)) {                
                result.Code = 0;
            }
            return Json(new {
                Code = result.Code
            });
        }

        public FileStreamResult DownloadResources(string playInfoId, string token, long timestamp, string sign) {
           try
            {
                var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object> { { "playInfoId", playInfoId } });
                if (!result.IsSuccess()){
                    return null;
                }

                var zipFile = _programAppService.GetResourcesById(new Guid(playInfoId));

                if (zipFile == null || zipFile.File == null) return null;
                        
                return File(new MemoryStream(zipFile.File), "application/octet-stream", zipFile.Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public JsonResult PostSystemPlayInfo(string playInfo, string playerPlayInfo, string player, string token, long timestamp, string sign) {
            var result = CheckParams(out var activeEquipment, token, timestamp, sign, new Dictionary<string, object>() { { "playInfo", playInfo }, { "playerPlayInfo", playerPlayInfo }, { "player", player } });
            if (!result.IsSuccess()) {
                return Json(new EquipmentResult {
                    Code = result.Code.ToString(),
                    Msg = result.Msg
                });
            }
            var info = new PlayInfoDto {
                PlayInfo = playInfo,
                PlayerPlayInfo = playerPlayInfo,
                Player = player
            };
            activeEquipment.PlayInfo = info;
            _activeEquipmentAppService.Update(activeEquipment);
            return Json(result);
        }
    }

}