using LsAdmin.Application.RoleApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.UserApp.Dtos
{
    public class UserRoleDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public virtual UserDto User { get; set; }
        public virtual RoleDto Role { get; set; }
    }
}
