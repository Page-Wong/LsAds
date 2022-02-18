using LsAdmin.Application.RoleApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.RoleApplyApp.Dtos
{
    public class RoleApplyDto {
        public Guid Id { get; set; }

        public Guid CreateUserId { get; set; }

        public DateTime? CreateTime { get; set; }

        public string Remarks { get; set; }

        public Guid ApplyUserId { get; set; }

        public virtual UserDto ApplyUser { get; set; }

        public DateTime ApplyTime { get; set; }

        public Guid RoleId { get; set; }
        public virtual RoleDto Role { get; set; }

        public ushort Status { get; set; }
        public const ushort STATUS_AUDITTING = 1;
        public const ushort STATUS_PASS = 2;
        public const ushort STATUS_UNPASS = 3;

        public virtual string StatusString {
            get {

                if (Status == STATUS_AUDITTING) return "审核中";
                if (Status == STATUS_PASS) return "通过";
                if (Status == STATUS_UNPASS) return "不通过";

                return "未知";
            }
        }
    }
}
