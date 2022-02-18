using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class Role : BaseEntity {
        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RoleMenu> RoleMenus { get; set; }
    }
}
