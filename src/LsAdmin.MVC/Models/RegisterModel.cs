using LsAdmin.Application.RoleApp;
using LsAdmin.Utility.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class RegisterModel {
        public const string MARKETER_ROLE_NAME = "Marketer";
        public const string PLACEOWNER_ROLE_NAME = "PlaceOwner";
        public const string EQUIPMENTOWNER_ROLE_NAME = "EquipmentOwner";

        [Required(ErrorMessage = "用户名不能为空。")]
        [RegularExpression("^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){5,17}$",ErrorMessage = "用户名以字母开头，长度在6~18之间，只能包含字符、数字、'_'和'.'")]

        public string UserName { get; set; }


        [DisplayName("密码")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{7,15}$", ErrorMessage = "密码必须要同时含有数字和字母，且长度要在8-16位之间")]
        [Required(ErrorMessage = "密码不能为空。")]
        public string Password { get; set; }


        [DisplayName("确认密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare("Password", ErrorMessage = "两次密码输入不一致")]
        public string ConfirmPassword {
            get;
            set;
        }

        [Required(ErrorMessage = "手机号码不能为空。")]
        public string MobileNumber { get; set; }

        public string SmsCode { get; set; }
        public string RegisterType { get; set; }
        public string RegisterTypeName {
            get {
                var service = (IRoleAppService)HttpHelper.ServiceProvider.GetService(typeof(IRoleAppService));
                return service?.GetByCode(RegisterType)?.Name;
            }
        }

    }
}
