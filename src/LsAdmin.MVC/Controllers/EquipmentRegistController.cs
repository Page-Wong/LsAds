using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using LsAdmin.Application.EquipmentModelApp;
using Microsoft.AspNetCore.Authorization;
using LsAdmin.Application.EquipmentApp;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using LsAdmin.MVC.Models;
using LsAdmin.Utility.Convert;
using LsAdmin.Application.RegistEquipmentApp.Dto;
using System.ComponentModel.DataAnnotations;
using LsAdmin.Application.EquipmentApp.Dtos;

namespace LsAdmin.MVC.Controllers
{
    public class EquipmentRegistController : LsAdminControllerBase
    { 
        private readonly  IEquipmentModelAppService _equipmentModelService;
        private readonly  IEquipmentAppService _equipmentService;
        private readonly string apiRegistControl;
        public EquipmentRegistController(IEquipmentModelAppService equipmentModelService, IEquipmentAppService equipmentService, IOptions<WebAPIAddressModel> settings)
        {
            _equipmentModelService = equipmentModelService;
            _equipmentService = equipmentService;
            apiRegistControl = "http://"+ settings.Value.Ip+":"+ settings.Value.Port + "/Regist/";
        }
        /*public IActionResult GetRegisteredPage(string registeid)
        {
            try { 
                string serviceAddress = apiRegistControl+"GetRegisteredPage?registeid="+registeid;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                JObject jobj = JObject.Parse(retString);
                if (jobj.GetValue("result").ToString() == "Success") { 
                    ViewBag.registeid = registeid;
                    return View();
                }
                else{
                    return Content(string.Format("注册信息出错：{0}", jobj.GetValue("errormessage").ToString())); // 可以指定文本类型  
                }
            }
            catch(Exception ex)
            {
                return Content(string.Format("注册信息出错")); 
            }
        }*/

        private RegistEquipmentDto GetRegistEquipment(Guid id) {
            var url = $"{apiRegistControl}/GetRegistEquipment?id={id.ToString()}";
            WebClient webClient = new WebClient();
            //webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] responseData = webClient.DownloadData(url);
            return ByteConvertHelper.Bytes2Object<RegistEquipmentDto>(responseData);
        }

        private bool PostRegisterSucceed(Guid id) {
            var url = $"{apiRegistControl}/SendRegisterSucceedAsync";
            WebClient webClient = new WebClient();            
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string responseData = webClient.UploadString(url, $"id={id.ToString()}");
            var jo = JObject.Parse(responseData);
            var result = jo.GetValue("result");
            return result.ToString() == "Success";
        }

        public IActionResult GetEquipmentModeList()
        {
            var equipmentModeList = _equipmentModelService.GetAllList().ToList();
            return Json(new{ equipmentModeList = equipmentModeList,});
        }

        public  IActionResult  Registered(EquipmentModal modal)
        {
            if (ModelState.IsValid) {

                try {
                    var dto = GetRegistEquipment(modal.RegisteId);
                    if (dto == null) {
                        return Json(new { Result = "Faild", errormessage = "设备注册信息有误" });
                    }

                    EquipmentDto item = new EquipmentDto();
                    if (modal.EquipmentId != null) {
                        item = _equipmentService.Get(modal.EquipmentId.Value);
                    }
                    if (item == null) {
                        return Json(new { Result = "Faild", errormessage = "原始设备信息有误" });
                    }
                    item.EquipmentModelId = modal.EquipmentModeId;
                    item.OwnerUserId = CurrentUser.Id;
                    item.DeviceId = dto.DeviceId;
                    item.Name = modal.EquipmentName;
                    if (!_equipmentService.Registered(ref item, out string errormessage)) {
                        return Json(new { Result = "Faild", errormessage = errormessage });
                    }
                    PostRegisterSucceed(modal.RegisteId);
                    return Json(new { Result = "Success", errormessage = "注册成功" });
                }
                catch (Exception ex) {
                    return Json(new { Result = "Faild", errormessage = "设备注册失败" });
                }
            }
            return Json(new { Result = "Faild", errormessage = GetModelStateError() });

            /*try
            {
                string serviceAddress = apiRegistControl + "/Registered?registeid=" + registeid+ "&equipmentName="+ equipmentName+
                                       "&currentUserId="+ CurrentUser.Id+ "&equipmentModeId="+ equipmentModeId;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                JObject jobj = JObject.Parse(retString);
                if (jobj.GetValue("result").ToString() == "Success"){
                    return Json(new { Result = "Success" });
                }
                else{
                    return Json(new { Result = "Faild", Message = string.Format("设备注册失败：{0}", jobj.GetValue("errormessage").ToString()) });
                }
            }
            catch (Exception ex){
                return Json(new { Result = "Faild", Message = "设备注册失败" });
            }*/
        }        

        [HttpPost]
        public IActionResult Scaned(Guid id, ushort status) {
            var url = $"{apiRegistControl}/PostRegistEquipmentState";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string responseData = webClient.UploadString(url, $"id={id.ToString()}&status={status}");
            bool success = JObject.Parse(responseData).GetValue("Result").ToObject<bool>();            
            return Json(new { Result = success ? "Success": "Faild" });
        }

        public class EquipmentModal {

            [Required(ErrorMessage = "预注册ID为空")]
            public Guid RegisteId { get; set; }
            [Required(ErrorMessage = "设备名称为空")]
            public string EquipmentName { get; set; }
            [Required(ErrorMessage = "设备型号为空")]
            public Guid EquipmentModeId { get; set; }

            
            public Guid? EquipmentId { get; set; }
        }
    }
}