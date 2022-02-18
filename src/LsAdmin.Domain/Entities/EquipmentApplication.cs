using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.Entities
{
    public class EquipmentApplication : BaseEntity
    {
        //场所ID
        public Guid PlaceId { get; set; }

        //申请信息
        public string Reason { get; set; }

        /// 审核状态(0:待审核 1：审核中  2：审核通过 3：审核不通过 ）
        public ushort Status { get; set; }
    }
}
