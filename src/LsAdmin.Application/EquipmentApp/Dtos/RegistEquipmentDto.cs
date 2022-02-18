using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace LsAdmin.Application.RegistEquipmentApp.Dto
{
     // <summary>
    /// 设备注册
    /// </summary>
    public class RegistEquipmentDto
    {
        [Key]
        public Guid Id { get; set; }


        public WebSocket WebSocket { get; set; }

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

        public double AeadMinutes { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        public  DateTime? AeadTime
        {
            get
            {
                try { return ApplyTime.AddMinutes(AeadMinutes); }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 已失效
        /// </summary>
        public  bool IsDefunct
        {
            get
            {
                try { return ApplyTime.AddMinutes(AeadMinutes) < DateTime.Now; }
                catch (Exception ex) { return false; }
            }
        }

        public  bool isCanRegisteredStatus
        {
            get
            {
                return (new ushort[] { STATUS_TO_BE_SENT, STATUS_SENTED, STATUS_SCANED }).Contains(Status);
            }
        }


        public const ushort STATUS_TO_BE_SENT = 0;
        public const ushort STATUS_SENTED = 1;
        public const ushort STATUS_SCANED = 2;
        public const ushort STATUS_FINISH = 3;

        public  string StatusString
        {
            get
            {
                if (Status == STATUS_TO_BE_SENT) return "待发送";
                if (Status == STATUS_SENTED) return "已发送";
                if (Status == STATUS_SCANED) return "已扫描";
                if (Status == STATUS_FINISH) return "注册成功";
                return "未知";
            }
        }

    }
}
