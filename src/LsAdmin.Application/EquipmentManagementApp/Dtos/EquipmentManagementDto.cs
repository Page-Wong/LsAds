using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.EquipmentManagementApp.Dtos
{
    public class EquipmentManagementDto
    {
        public Guid Id { get; set; }

        //报修设备ID
        public Guid EquipmentId { get; set; }

        //是否维修
        public int IsRepair { get; set; }

        //维修状态
        public string RepairStatus { get; set; }

        
        /// 备注
        public string Remarks { get; set; }


        /// 创建人
        public Guid CreateUserId { get; set; }

        /// 创建时间
        public DateTime? CreateTime { get; set; }
    }

}
