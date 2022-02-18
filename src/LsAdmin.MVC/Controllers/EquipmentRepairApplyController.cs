using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.EquipmentRepairApp;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentRepairApp.Dtos;
using LsAdmin.Application.PlaceApp;
using LsAdmin.MVC.Models;
using System.IO;
using LsAdmin.Application.FilesApp;
using LsAdmin.Application.FilesApp.Dtos;
using AutoMapper;
using LsAdmin.Application.EquipmentApp.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class EquipmentRepairApplyController : LsAdminControllerBase
    {
        private readonly IEquipmentRepairAppService _service;
        private readonly IEquipmentAppService _eservice;
        private readonly IPlaceAppService _placeservice;
        private readonly IFilesAppService _filesService;
        private readonly IEquipmentRepairAppService _equipmentRepairAppService;

        public EquipmentRepairApplyController(IEquipmentRepairAppService equipmentRepairAppService, IEquipmentRepairAppService service, IPlaceAppService placeservice, IFilesAppService filesService, IEquipmentAppService eservice)
        {

            _service = service;
            _placeservice = placeservice;
            _filesService = filesService;
            _eservice = eservice;
            _equipmentRepairAppService = equipmentRepairAppService;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Json(dto);
        }

        public IActionResult GetPageList(int startPage, int pageSize, ushort status) {
            
            var places = _placeservice.GetUserAllPlaces(CurrentUser.Id);
            var result = _eservice.GetAllEquipmentByPlaces(places.Select(it => it.Id).ToList()).Where(it => it.Status == status);
            int rowCount = result.Count();
            for (int i = 0; i < result.Count(); i++) {
                result.ElementAtOrDefault(i).EquipmentRepairDto = _equipmentRepairAppService.GetByEquipmentId(result.ElementAtOrDefault(i).Id);
            }
            return Json(new {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        //新增维修记录
        public IActionResult SaveRepair(EquipmentRepairDto dto)
        {
           
            var guid= Guid.NewGuid();
            dto.WarningDate = DateTime.Now;
            dto.ProcessingMethod = 0;
            dto.Status = 0;
            dto.AfterMaterial = Guid.NewGuid();
            dto.BeforeMaterial = Guid.NewGuid();

            var edto = _eservice.Get(dto.EquipmentId);
            edto.Status = 2;


            //校验重复上传申请
            var existApply = _service.GetListByEquipmentId(dto.EquipmentId).Where(it => it.Status != EquipmentRepairDto.STATUS_COMPLETE);
            if (existApply.Count() > 0)
            {
                return Json(new { Result = "Faild", Message = "您已提交过报修申请，请勿重复提交！" });
            }

            if (string.IsNullOrEmpty(dto.ProblemDescription))
            {
                return Json(new { Result = "Faild", Message = "报修原因不可为空！" });
            }

            if (string.IsNullOrEmpty(dto.PlaceContact))
            {
                return Json(new { Result = "Faild", Message = "场所联系人不可为空！" });
            }

            if (string.IsNullOrEmpty(dto.PlaceContactPhone))
            {
                return Json(new { Result = "Faild", Message = "场所联系人联系信息不可为空！" });
            }

            if (_service.InsertOrUpdate(ref dto))
            {
                if (_eservice.Update(edto))
                {
                    return Json(new
                    {
                        Result = "Success"
                    });
                }
                             
            }
            return Json(new { Result = "Faild" });
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

            var filenames = _filesService.GetAllList().Select(s => s.Name).Distinct();
            if (filenames.Contains(file.FileName))
            {
                return Json(new { Result = "Faild", Message = "该素材重复上传，请重新选择素材!" });
            }
            else
            {
                FilesDto dto = new FilesDto
                {
                    Id = Guid.Empty,
                    Name = file.FileName,
                    FilenameExtension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1, (file.FileName.Length - file.FileName.LastIndexOf(".") - 1)),
                    Remarks = "",
                    //将选中的BeforeMaterial存储为OwnerObjId,可通过识别BeforeMaterial读取文件
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
            }
        }

    }
}
