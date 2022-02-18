using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    /// <summary>
    /// 设备预注册记录
    /// </summary>
    public class PreRegisteredDevice : BaseEntity
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ushort Status { get; set; }

        /// <summary>
        /// 预注册记录有效时长（分钟）
        /// </summary>
        public double AeadMinutes { get; set; }

    }
}
