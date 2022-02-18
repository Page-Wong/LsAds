using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class ChangePaymentPasswordViewModel {        

        [Display(Name = "新密码")]
        [Required(ErrorMessage = "密码不能为空")]
        [RegularExpression("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{7,15}$", ErrorMessage = "密码必须要同时含有数字和字母，且长度要在8-16位之间")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        [DisplayName("确认密码")]
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare("NewPassword", ErrorMessage = "两次密码输入不一致")]
        [DataType(DataType.Password)]
        public string NewConfirmPassword { get; set; }

        [Required(ErrorMessage = "必须填写手机验证码")]
        public string SmsCode { get; set; }
    }
}
