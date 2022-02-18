using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Web;
using System.Collections.Generic;
using LsAdmin.MVC.Models;
using System.Text;
using System;
using LsAdmin.Application.MenuApp.Dtos;
using System.Linq;
using LsAdmin.Application.PlayHistoryApp;
using LsAdmin.Application.PlayHistoryApp.Dtos;
using Newtonsoft.Json;
using LsAdmin.Application.MaterialApp;
using AutoMapper;
using LsAdmin.Application.OrderMaterialApp;
using LsAdmin.Application.MaterialApp.Dtos;

namespace LsAdmin.MVC.Controllers {
    public class MaterialController : LsAdminControllerBase {

        private readonly IMaterialAppService _service;

        private readonly IOrderMaterialAppService _orderMateriaservice;
        
        public MaterialController(IMaterialAppService service, IOrderMaterialAppService orderMateriaservice) {
            _service = service;
            _orderMateriaservice = orderMateriaservice;
        }

        // GET: /<controller>/
        public IActionResult Index() {
            return View();
        }

        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult Add() {
            var file = Request.Form.Files["dir_file"];
            if (file == null) {
                return Json(new { Result = "Faild" });
            }

            long duration = 0;

            if(Request.Form.TryGetValue("duration", out var videoduration))
            {
                try
                {
                    duration = Convert.ToInt64(Math.Round(System.Convert.ToDouble(videoduration)));
                }catch(Exception e)
                {
                    duration = 0;
                }            
            }

            Stream stream = file.OpenReadStream();
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            
            var existDto = _service.GetAllList().Where(s => s.Name == file.FileName).FirstOrDefault();
            if (existDto != null)
            {
                return Json(new { Result = "Faild",Data = existDto, Message = "该素材重复上传，请在从素材库选择素材!" });
            }
            else
            {
                MaterialDto dto = new MaterialDto {
                    Id = Guid.Empty,
                    Name = file.FileName,
                    FilenameExtension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1, (file.FileName.Length - file.FileName.LastIndexOf(".") - 1)),
                    Remarks = "",
                    Duration = duration,
                    OwnerUserId = CurrentUser.Id,
                    CreateTime = DateTime.Now,
                    CreateUserId = CurrentUser.Id,
                    //      File = bytes
                };
                if (_service.Insert(ref dto))
                {
                    Utility.FTP.FtpHelper.UploadFtpFile(dto.Id.ToString() + "." + dto.FilenameExtension, bytes);
                    return Json(new { Result = "Success", Data = Mapper.Map<MaterialDto>(dto) });
                }
                return Json(new { Result = "Faild", Message = "上传失败" });
            }
        }

        /// <summary>
        /// 编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IActionResult Edit(MaterialDto dto) {
            if (!ModelState.IsValid) {
                return Json(new {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
             var odto = _service.Get(dto.Id);
            if(odto.OwnerUserId != CurrentUser.Id)
                return Json(new { Result = "Faild",
                                  Message = "你的素材列表中不存在你需要修改的素材！请刷新纪录后再试！",
                });
            odto.Remarks = dto.Remarks;
            //dto.OwnerUserId = null;

            if (_service.Update( odto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
/*
            if (_service.InsertOrUpdate(ref odto)) {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });*/
        }

        public IActionResult GetAllPageList(int startPage, int pageSize, ushort materialType = 0) {
            int rowCount = 0;
            List<MaterialDto> result;

            if (materialType != 0) {
                 result = _service.GetPageListByType(startPage, pageSize, out rowCount, materialType);
            }
            else
            {
                 result = _service.GetOwnerPageList(startPage, pageSize, out rowCount);
            }
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }


        /// <summary>
        /// 获取媒体列表
        /// </summary>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="materalType">媒体类型 1=图片类型 ， 2=视频类型 </param>
        /// <returns></returns>
        public IActionResult GetMateralTypeAllPageList(int startPage, int pageSize, ushort materalType)
        {
            //  materalType : 1=图片类型 ， 2=视频类型
            int rowCount = 0;
            var result = _service.GetPageListByType(startPage, pageSize, out rowCount, materalType);         
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        public IActionResult DeleteMuti(string ids) {
            try {

                if( string.IsNullOrEmpty(ids))
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

                if (_orderMateriaservice.GetAllList().Where(w => delIds.Contains(w.MaterialId)).Count() > 0)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你选定的素材中存在已被使用过的素材，删除操作失败！",
                    });
                }

                foreach (string id in idArray) {
                    delIds.Add(Guid.Parse(id));
                }

                //防止非法删除别人数据，& 判断删除数据列表中的数据是否都存在，防止重复删除
                if (_service.GetAllList().Where(w =>  delIds.Contains(w.Id)).Count() != (idArray.Count()))
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "提交需要删除数据列表有误！",
                    });
                }

                _service.DeleteBatch(delIds);
                return Json(new {
                    Result = "Success"
                });
            }
            catch (Exception ex) {
                return Json(new {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        public IActionResult Delete(Guid id) {
            try {
                if (id==null){
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "没有需要删除的数据，删除操作失败！",
                    });
                }

                if(_service.Get(id)?.OwnerUserId != CurrentUser.Id){
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "您的素材列表中不在你需要删除的素材，删除操作失败！",
                    });
                }
                var o1 = _orderMateriaservice.GetAllList();
                var o2 = _orderMateriaservice.GetAllList().Where(w => w.MaterialId == id);
                var o3 = _orderMateriaservice.GetAllList().Where(w => w.MaterialId == id).Count();

                if (_orderMateriaservice.GetAllList().Where(w => w.MaterialId== id).Count() > 0)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "你选定的素材已被使用过，删除操作失败！",
                    });
                }

                _service.Delete(id);
                return Json(new {
                    Result = "Success"
                });
            }
            catch (Exception ex) {
                return Json(new {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        public IActionResult Get(Guid id) {
            var dto = _service.GetInfo(id);
            //防止非法获取别人的播放素材
            if(dto.OwnerUserId != CurrentUser.Id){
                dto = null;
            }

            return Json(dto);
        }

        public async System.Threading.Tasks.Task PlayAsync(Guid id) {
            if (id == null) return;
            var dto = _service.GetInfo(id);
            if (dto == null || dto.OwnerUserId != CurrentUser.Id ) return;
            var bufferSize = 256 * 1024;
            var buffer = 0;
            var file = dto.File;
            while (true)
            {
                if (buffer >= file.Length)
                {
                    break;
                }

                if (buffer + bufferSize > file.Length)
                {
                    bufferSize = file.Length - buffer;
                }

                await Response.Body.WriteAsync(file, buffer, bufferSize);
                buffer += bufferSize;
            }

        }

    }

}
