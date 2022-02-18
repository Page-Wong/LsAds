using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.PersonConfirmApp;
using LsAdmin.Application.EnterpriseConfirmApp;
using LsAdmin.Application.PersonConfirmApp.Dtos;
using System.IO;
using LsAdmin.Application.EnterpriseConfirmApp.Dtos;
using LsAdmin.Application.UserApp;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.MVC.Models;
using LsAdmin.Utility.Convert;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class PersonConfirmController : LsAdminControllerBase
    {
        private readonly IPersonConfirmAppService _service;
        private readonly IEnterpriseConfirmAppService _companyservice;
        private readonly IUserAppService _userservice;
        public PersonConfirmController(IPersonConfirmAppService service, IUserAppService userservice, IEnterpriseConfirmAppService companyservice)
        {
            _service = service;
            _companyservice = companyservice;
            _userservice = userservice;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index(UserDto model)
        {
         
            return View();
        }   

        /// 新增实名认证申请

        public IActionResult Add(PersonConfirmDto dto)
        {        
           
            var filefront = Request.Form.Files["UploadIdFrontFile"];
            var fileback = Request.Form.Files["UploadIdBackFile"];

            if (dto.Name == null)
            {
                return Json(new { Result = "Faild", Message = "请填写姓名！" });
            }

            if (dto.IdNumber == null)
            {
                return Json(new { Result = "Faild", Message = "请填写证件号码！" });
            }

            if (filefront == null)
            {
                return Json(new { Result = "Faild", Message = "请上传证件！" });
            }

            if (fileback == null)
            {
                return Json(new { Result = "Faild", Message = "请上传证件！" });
            }

            if (dto.IdDuration == null)
            {
                return Json(new { Result = "Faild", Message = "请填写证件到期时间！" });
            }

            if (dto.Location== null)
            {
                return Json(new { Result = "Faild", Message = "请填写地址！" });
            }
          

            Stream stream = filefront.OpenReadStream();
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            Stream streamback = fileback.OpenReadStream();
            byte[] bytesback = new byte[streamback.Length];
            stream.Read(bytesback, 0, bytesback.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            dto.Id = Guid.Empty;
            dto.CreateTime = DateTime.Now;
            dto.CreateUserId = CurrentUser.Id;
            dto.UserName = CurrentUser.UserName;
            dto.UploadIdFront = bytes;
            dto.UploadIdBack = bytesback;

            CurrentUser.AuthStatus = 1;

            //校验重复上传认证
            var existPerson = _service.GetAllList().FirstOrDefault(f => f.UserName == dto.UserName);
            if (existPerson != null)
            {
                return Json(new { Result = "Faild", Message = "您已提交过个人实名认证申请，请勿重复提交！" });
            }

            var existEnterprise = _companyservice.GetAllList().FirstOrDefault(f => f.CreateUserId == dto.CreateUserId);
            if(existEnterprise!=null)
            {
                return Json(new { Result = "Faild", Message = "您已提交过企业实名认证申请，" +
                    "请勿重复提交！" });
            }

            if (_service.Insert(ref dto) )
            {
                //var udto = _userservice.Get(dto.CreateUserId);
               
               // var user = _userservice.Get(CurrentUser.Id);
               // user.AuthStatus = 1;
                if (_userservice.Update(CurrentUser))
                {
                    CurrentUser = CurrentUser;
                    //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                    return Json(new
                    {
                        Result = "Success"
                    });
                }
            }
            return Json(new { Result = "Faild" });

        }


    }

    public class EnterpriseConfirmController : LsAdminControllerBase
    {
        private readonly IEnterpriseConfirmAppService _service;
        private readonly IPersonConfirmAppService _personservice;
        private readonly IUserAppService _userservice;
        public EnterpriseConfirmController(IEnterpriseConfirmAppService service, IUserAppService userservice, IPersonConfirmAppService personservice)
        {
            _service = service;
            _personservice = personservice;
            _userservice = userservice;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// 新增实名认证申请

        public IActionResult Add(EnterpriseConfirmDto dto)
        {

            var scanfile = Request.Form.Files["DuplicateLicenseScanFile"];
            var sealfile = Request.Form.Files["DuplicateLicenseSealFile"];

            if (dto.Name == null)
            {
                return Json(new { Result = "Faild", Message = "请填写企业名称！" });
            }

            if (dto.RegisteredNumber == null)
            {
                return Json(new { Result = "Faild", Message = "请填写营业执照注册号！" });
            }

            if (dto.LicenseAddress == null)
            {
                return Json(new { Result = "Faild", Message = "请填写营业执照所在地地址！" });
            }

            if (dto.Period == null)
            {
                return Json(new { Result = "Faild", Message = "请填写营业期限！" });
            }

            if (dto.Location == null)
            {
                return Json(new { Result = "Faild", Message = "请填写常用地址！" });
            }

            if (scanfile == null)
            {
                return Json(new { Result = "Faild", Message ="请上传证件！" });
            }

            if (sealfile == null)
            {
                return Json(new { Result = "Faild", Message = "请上传证件！" });
            }


            Stream stream = scanfile.OpenReadStream();
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            Stream streamseal = sealfile.OpenReadStream();
            byte[] bytesseal = new byte[streamseal.Length];
            stream.Read(bytesseal, 0, bytesseal.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            dto.Id = Guid.Empty;
            dto.CreateTime = DateTime.Now;
            dto.CreateUserId = CurrentUser.Id;
            dto.DuplicateLicenseScan = bytes;
            dto.DuplicateLicenseSeal = bytesseal;

            //校验重复上传认证
            var existEnterprise = _service.GetAllList().FirstOrDefault(f => f.CreateUserId == dto.CreateUserId);
            if (existEnterprise != null)
            {
                return Json(new { Result = "Faild", Message = "您已提交过企业实名认证申请，请勿重复提交！" });
            }

            var existPerson = _personservice.GetAllList().FirstOrDefault(f => f.CreateUserId == dto.CreateUserId);
            if(existPerson!=null)
            {
                return Json(new { Result = "Faild", Message = "您已提交过个人实名认证申请，请勿重复提交！" });
            }


            CurrentUser.AuthStatus = 1;

            if (_service.Insert(ref dto))
            {
                if (_userservice.Update(CurrentUser))
                {
                    CurrentUser = CurrentUser;
                    //HttpContext.Session.Set("CurrentUser", ByteConvertHelper.Object2Bytes(CurrentUser));
                    return Json(new
                    {
                        Result = "Success"
                    });
                }
            }
            return Json(new { Result = "Faild" });

        }

    }

    public class ContractController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
