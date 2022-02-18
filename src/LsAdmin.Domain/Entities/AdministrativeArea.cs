using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class AdministrativeArea : BaseEntity {
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
