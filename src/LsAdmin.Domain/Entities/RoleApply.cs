using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class RoleApply : BaseEntity {
        public Guid ApplyUserId { get; set; }

        public DateTime ApplyTime { get; set; }

        public Guid RoleId { get; set; }

        public ushort Status { get; set; }

        public virtual User ApplyUser { get; set; }
        public virtual Role Role { get; set; }

    }
}
