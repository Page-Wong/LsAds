using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using System;

namespace LsAdmin.Application.AndroidapkApp.Dtos {
    public class AndroidapkDto {
        public Guid Id { get; set; }

        public String AppName { get; set; }
        public String PackageName { get; set; }
        public String VersionName { get; set; }
        public int VersionCode { get; set; }
        public int EquipmentType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
