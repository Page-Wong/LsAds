using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class ProfileModel {
        public string UserName { get; set; }
        public string Name { get; set; }
        

        [Required(ErrorMessage = "手机号码不能为空。")]
        public string MobileNumber { get; set; }
        public string EMail { get; set; }

        public byte[] Avatar { get; set; }
        public int AuthStatus { get; set; }

        public string[] RoleNames { get; set; }

    }
}
