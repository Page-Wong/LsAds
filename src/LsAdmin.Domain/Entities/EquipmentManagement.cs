using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.Entities
{
   public class EquipmentManagement: BaseEntity
    {
        //报修设备ID
        public Guid EquipmentId { get; set; }

        //是否维修
        public int IsRepair { get; set; }

        //维修状态
        public string RepairStatus { get; set; }


        
    }
}
