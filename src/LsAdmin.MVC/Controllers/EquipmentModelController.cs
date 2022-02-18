using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentModelApp;
using LsAdmin.Application.EquipmentModelApp.Dtos;
using LsAdmin.Application.FilesApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LsAdmin.MVC.Controllers
{

    public class EquipmentModelController : LsAdminControllerBase
    {
        private readonly IEquipmentModelAppService _service;
        private readonly IEquipmentAppService _Equipmentservice;
        private readonly IFilesAppService      _Filesservice;
        
        public EquipmentModelController(IEquipmentModelAppService service,IEquipmentAppService Equipmentservice, IFilesAppService Filesservice)
        {
            _service = service;
            _Equipmentservice = Equipmentservice;
            _Filesservice = Filesservice;
        }

        // GET: /<controller>/
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllPageList(int startPage, int pageSize, ushort materialType = 0)
        {
            int rowCount = 0;
            var result = _service.GetAllPageList(startPage, pageSize, out rowCount).OrderByDescending(o =>o.CreateTime);
            var ApplyTos = new[] { "停车场出入口", "楼宇出入口", "楼宇外场", "电梯", "灯箱","公共服务", "酒店宾馆", "医院", "政务工程" };
            var ScreenMaterials = new[] { "LED", "IPS", "ADS", "PLS", "VA", "TN", "LCD", "CRT", "其他" };

            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                applyTos= ApplyTos,
                screenMaterials= ScreenMaterials,
                rows = result,
            });
        }


        public IActionResult Edit(EquipmentModelInfoDto dto){
            try{
                /*  if (!ModelState.IsValid){
                        return Json(new{
                            Result = "Faild",
                            Message ="操作失败！" +GetModelStateError()
                        });
                        }
                */
            
                if (string.IsNullOrEmpty(dto.Model)){
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "型号信息不能为空！",
                        ErrorField = "Model"
                    });
                }

                if (dto.ScreenSize <= 0) {
                    return Json(new{
                        Result = "Faild",
                        Message = "屏幕尺寸需要大于0！",
                        ErrorField = "ScreenSize"
                    });
                }

                if (string.IsNullOrEmpty(dto.ScreenMaterial))
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "屏幕材质不能为空！",
                        ErrorField = "ScreenMaterial"
                    });
                }

                
                if (_service.ExistSameModel(dto.Model,dto.Manufacturer,dto.Id) )
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "系统已经存在同型号同生产厂家记录!",
                        ErrorField = "Model"
                    });
                }
                
                var filethumbnail = Request.Form.Files["InputThumbnail"];

                if (filethumbnail != null)
                {
                    Stream streamthumbnail = filethumbnail.OpenReadStream();
                    byte[] thumbnail = new byte[streamthumbnail.Length];
                    streamthumbnail.Read(thumbnail, 0, thumbnail.Length);
                    // 设置当前流的位置为流的开始
                    streamthumbnail.Seek(0, SeekOrigin.Begin);
                    dto.Thumbnail = thumbnail;
                }
                else
                {
                    dto.Thumbnail = null;
                }
         

                if (dto.Id.ToString() != "00000000-0000-0000-0000-000000000000" && dto.Id != null && _service.Get(dto.Id) != null){
                    dto.CreateTime = DateTime.Now;
                    if (_service.UpdateInfo(dto)){
                        return Json(new { Result = "Success" });
                    }
                    return Json(new { Result = "Faild" });
                }
                else
                {
                    if (_service.InsertInfo(dto))
                    {
                        return Json(new { Result = "Success" });
                    }
                    return Json(new { Result = "Faild" });
                }
            }
            catch (Exception ex){
                return Json(new{
                    Result = "Faild",
                    Message = ex.ToString()+GetModelStateError()
                });
            }
        }

        public IActionResult DeleteMuti(string ids)
        {
            try
            {

                if (string.IsNullOrEmpty(ids))
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "没有需要删除的数据！",
                    });
                }

                string[] idArray = ids.Split(',');

                if (idArray.Count() == 0)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "没有需要删除的数据！",
                    });
                }

                List<Guid> delIds = new List<Guid>();

                foreach (string id in idArray)
                {
                    delIds.Add(Guid.Parse(id));
                }

                if (_Equipmentservice.GetAllList().Where(w => delIds.Contains(w.Id)).Count() > 0)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你选定的设备型号已使用过，删除操作失败！",
                    });
                }

                _service.DeleteBatcheInfo(delIds);
                return Json(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        public IActionResult Delete(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "没有需要删除的数据，删除操作失败！",
                    });
                }

                if (_service.Get(id)==null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "记录列表中没有你所选定要删除的记录！",
                    });
                }

               if (_Equipmentservice.GetAllList().Where(w => w.EquipmentModelId ==id).Count()>0)
               {
                   return Json(new
                   {
                       Result = "Faild",
                       Message = "你选定的设备型号已使用过，删除操作失败！",
                   });
               }
               
              _service.DeleteInfo(id);

                return Json(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }


        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }

        [AllowAnonymous]
        public async System.Threading.Tasks.Task GetThumbnail(Guid id)
        {
            if (id == null) return;
            var filedto = _Filesservice.GetOwnerObj(id)?.FirstOrDefault();
            
            if (filedto == null) { var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile("zbddx.jpg");
                await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
            } else {
                var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile(filedto.Id+"."+ filedto.FilenameExtension);
                await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);

                /* var thumbnail= Utility.FTP.FtpHelper.DownloadFtpFile(id.ToString() + ".jpg");
                if (equipmentModelInfo?.Thumbnail != null)
                {
                    var thumbnail = equipmentModelInfo?.Thumbnail;
                    await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
                }
                else
                {
                    var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile("zbddx.jpg");
                    await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
                }*/
            }
        }

    }


}
