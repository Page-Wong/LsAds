using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.PlaceTypeApp;
using LsAdmin.Application.LabelApp;
using LsAdmin.Application.LabelApp.Dtos;
using LsAdmin.MVC.Models;
using LsAdmin.Application.FilesApp;
using System.IO;
using LsAdmin.Application.FilesApp.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using LsAdmin.Application.EquipmentApp.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class PlaceController : LsAdminControllerBase
    {
        private readonly IPlaceAppService _service;
        private readonly ILabelAppService _labelService;
        private readonly IFilesAppService _filesService;

        public PlaceController(IPlaceAppService service, ILabelAppService labelService, IFilesAppService filesService)
        {
            _service = service;
            _labelService = labelService;
            _filesService = filesService;
        }
            // GET: /<controller>/
            public IActionResult Index()
        {
            return View();
        }
       

        //加载场所类型
        public IActionResult GetPlaceByType(Guid typeId, int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetUserPagePlaceByType(typeId, startPage, pageSize, CurrentUser.Id, out rowCount);
            var adstags = _labelService.GetAdsTag();
            var placetags = _labelService.GetPlaceTag();
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                adstags = adstags,
                placetags= placetags
            });
        }
        //场所基本信息列表
        public IActionResult GetAllPageList(int startPage, int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetUserPagePlaces(startPage, pageSize, CurrentUser.Id, out rowCount);
            var adstags = _labelService.GetAdsTag();
            var placetags = _labelService.GetPlaceTag();
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
                adstags = adstags,
                placetags = placetags
            });
        }



        //新增场所
        public IActionResult Edit(PlaceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (dto.Id == Guid.Empty) {
                dto.OwnerUserId = CurrentUser.Id;
             }
            else
            {
                var place = _service.Get(dto.Id);
                if (place == null){
                    return Json(new { Result = "Faild", Message = "系统中不存在你修改的记录！" });
                }
                dto.OwnerUserId = place.OwnerUserId;
            }

            //校验重复上传场所
            var existPlace = _service.GetAllList().FirstOrDefault(f => f.Name == dto.Name && 
            f.Province == dto.Province && f.City==dto.City && f.Id!=dto.Id );
            if (existPlace != null)
            {
                return Json(new { Result = "Faild", Message = "您已添加过相同场所信息，请勿重复提交！" });
            }

            if (string.IsNullOrEmpty(dto.Name))
            { 
                return Json(new { Result = "Faild", Message = "场所名称不可为空！"});
            }

            if (string.IsNullOrEmpty(dto.Province)) 
            {
                return Json(new { Result = "Faild",Message = "省份不可为空！"});
            }

            if (string.IsNullOrEmpty(dto.City))
            {
                return Json(new { Result = "Faild",Message = "城市不可为空！"});
            }

            if (string.IsNullOrEmpty(dto.District)) 
            {
                return Json(new { Result = "Faild",Message = "区县不可为空！"});
            }

            if (string.IsNullOrEmpty(dto.Street)) 
            {
                return Json(new { Result = "Faild", Message = "街道地址不可为空！"});
            }

            if (_service.InsertOrUpdate(ref dto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }

        //删除场所
        public IActionResult Delete(Guid id)
        {
            try
            {
               string errorString = "";
               if(_service.DeletePlace(id,out errorString) == false){
                    return Json(new
                    {
                        Result = "Faild",
                        Message = errorString
                    });
                }

                //_service.Delete(id);
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

        //批量删除场所
        public IActionResult DeleteMuti(string ids)
        {
            try
            {
                string errorString = "";
                string[] idArray = ids.Split(',');
                List<Guid> delIds = new List<Guid>();
                foreach (string id in idArray)
                {
                    delIds.Add(Guid.Parse(id));
                }
                //_service.DeleteBatch(delIds);

                if (_service.DeleteBatchPlaces(delIds, out errorString)== false)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = errorString
                    });
                }

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

        //获得指定场所数据
        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }

        //获得指定素材数据
        public IActionResult GetMaterial(Guid id)
        {
            var dto = _filesService.Get(id);
            return Json(dto);
        }

        /// 获取场所类型

        [HttpGet]
        public IActionResult GetGridData()
        {
            var dtos = _service.GetAllList().OrderBy(f => f.Name);
            List<GridModel> gridModels = new List<GridModel>();
            foreach (var dto in dtos)
            {
                if (dto.CreateUserId == CurrentUser.Id)
                {
                    gridModels.Add(new GridModel() { Id = dto.Id.ToString(), Text = dto.Name });
                }
                
                
            }
            return Json(gridModels);
        }

        //新增素材
        public IActionResult AddMaterial()
        {
            var file = Request.Form.Files["dir_file"];
            if (file == null)
            {
                return Json(new { Result = "Faild" });
            }

            Request.Form.TryGetValue("ownerObjId", out var ownerObjId);

            Stream stream = file.OpenReadStream();
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            //var filenames = _filesService.GetAllList().Select(s => s.Name).Distinct();
            //if (filenames.Contains(file.FileName))
            //{
            //    return Json(new { Result = "Faild", Message = "该素材重复上传，请重新选择素材!" });
            //}
            //else
            //{
                FilesDto dto = new FilesDto
                {
                    Id = Guid.Empty,
                    Name = file.FileName,
                    FilenameExtension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1, (file.FileName.Length - file.FileName.LastIndexOf(".") - 1)),
                    Remarks = "",
                    //将选中的PlaceId存储为OwnerObjId,可通过识别PlaceId读取文件
                    OwnerObjId = Guid.Parse(ownerObjId),
                    CreateTime = DateTime.Now,
                    CreateUserId = CurrentUser.Id,
             
                };

                if (_filesService.Insert(ref dto))
                {
                    Utility.FTP.FtpHelper.UploadFtpFile(dto.Id.ToString() + "." + dto.FilenameExtension, bytes);
                    return Json(new { Result = "Success", Data = Mapper.Map<FilesDto>(dto) });
                }
                return Json(new { Result = "Faild", Message = "上传失败" });
            //}
        }


        //编辑素材备注
        public IActionResult EditMaterial(FilesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }

            var mdto = _filesService.Get(dto.Id);
            mdto.CreateTime = DateTime.Now;          
            mdto.Name = dto.Name;
            mdto.Remarks = dto.Remarks;
          

            if (_filesService.InsertOrUpdate(ref mdto))
            {
                return Json(new { Result = "Success" });
            }
            return Json(new { Result = "Faild" });
        }



        //播放素材文件
        [AllowAnonymous]
        public async Task GetThumbnail(Guid id)
        {
            if (id == null) return;
            var dto = _filesService.Get(id);
            // TODO 增加其他图片格式
            if (dto == null || !new string[] { "jpg", "png" }.Contains(dto.FilenameExtension)) return;

            if (dto == null)
            {
                var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile("zbddx.jpg");
                await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
            }else
            { 
                var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile(id.ToString() + "." + dto.FilenameExtension);
                await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
            }

        }

        [AllowAnonymous]
        public async Task GetThumbnailByObjId(Guid id) {
            if (id == null) return;
            var dto = _filesService.GetOwnerObj(id)?.FirstOrDefault();
            // TODO 增加其他图片格式
            if (dto == null || !new string[] { "jpg", "png" }.Contains(dto.FilenameExtension)) return;

            if (dto == null) {
                var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile("zbddx.jpg");
                await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
            }
            else {
                var thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile(id.ToString() + "." + dto.FilenameExtension);
                await Response.Body.WriteAsync(thumbnail, 0, thumbnail.Length);
            }

        }

        //批量删除素材
        public IActionResult DeleteMutiMaterial(string ids)
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

                //防止非法删除别人数据，& 判断删除数据列表中的数据是否都存在，防止重复删除
                if (_filesService.GetAllList().Where(w => delIds.Contains(w.Id)).Count() != (idArray.Count()))
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "提交需要删除数据列表有误！",
                    });
                }

                _filesService.DeleteBatch(delIds);
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

        //删除素材
        public IActionResult DeleteMaterial(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "没有需要删除的数据！",
                    });
                }

                if (_filesService.Get(id)?.CreateUserId != CurrentUser.Id)
                {
                    return Json(new
                    {
                        Result = "Faild",
                        Message = "您的播放素材列表中不存在你需要删除的素材！",
                    });
                }

                _filesService.Delete(id);
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

        //加载素材列表
        public IActionResult GetMaterialPageList(Guid OwnerObjId, int startPage, int pageSize, ushort type = 0)
        {
            int rowCount = 0;
            List<FilesDto> result;

            if (type != 0)
            {
                result = _filesService.GetPageListByType(OwnerObjId, startPage, pageSize, out rowCount,type);
            }
            else
            {
                result = _filesService.GetOwnerObjPageList(OwnerObjId, startPage, pageSize, out rowCount);
            }
            return Json(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }

        public IActionResult WxEdit(PlaceEditModel model) {
            if (!ModelState.IsValid) {
                return Json(new {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var dto = new PlaceDto {
                OwnerUserId = CurrentUser.Id
            };
            if (model.Id != null) {
                dto = _service.Get(model.Id.Value);
                if (dto == null) {
                    return Json(new { Result = "Faild", Message = "系统中不存在你修改的记录！" });
                }
                if (dto.OwnerUserId != CurrentUser.Id) {
                    return Json(new { Result = "Faild", Message = "你不能修改此场所信息！" });
                }
            }
            dto.Name = model.Name;
            dto.Introduction = model.Introduction;
            dto.Phone = model.Phone;
            dto.Contact = model.Contact;
            dto.MapPointX = model.Latitude;
            dto.MapPointY = model.Longitude;
            dto.Street = model.Address;

            if (_service.InsertOrUpdate(ref dto)) {
                return Json(new { Result = "Success", id=dto.Id });
            }
            return Json(new { Result = "Faild" });
        }

        public class PlaceEditModel {
            public Guid? Id { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            [Required(ErrorMessage = "场所名称不能为空。")]
            public string Name { get; set; }

            //场所介绍
            public string Introduction { get; set; }

            //联系电话
            [Required(ErrorMessage = "联系电话不能为空。")]
            public string Phone { get; set; }

            //负责人
            [Required(ErrorMessage = "负责人不能为空。")]
            public string Contact { get; set; }

            [Required(ErrorMessage = "定位信息不能为空。")]
            public decimal? Latitude { get; set; }

            [Required(ErrorMessage = "定位信息不能为空。")]
            public decimal? Longitude { get; set; }

            [Required(ErrorMessage = "场所地址不能为空。")]
            public string Address { get; set; }
        }

        #region 场所设备相关
        public IActionResult GetEquiomentPageList(int startPage, int pageSize) {
            int rowCount = 0;
            var result = _service.GetUserPageEquipments(startPage, pageSize, CurrentUser.Id, out rowCount, new List<uint> { EquipmentDto.STATUS_INUSE });
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result
            });
        }
        #endregion
    }
}
