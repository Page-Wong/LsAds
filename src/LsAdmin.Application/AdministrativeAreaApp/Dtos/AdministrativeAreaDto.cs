using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using System;

namespace LsAdmin.Application.AdministrativeAreaApp.Dtos {
    public class AdministrativeAreaDto {
        public Guid Id { get; set; }

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

        /// <summary>
        /// 上级区域编码
        /// </summary>
        public uint ParentCode { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public uint Code { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { get; set; }
    }
}
