using LsAdmin.Application.NotifyTypeApp.Dtos;
using System;

namespace LsAdmin.Application.NotifyApp.Dtos {
    public class NotifyDto {
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public Guid? CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 状态:99.发送错误 0.待发送 1.正在发送 2.已发送 3.已接收
        /// </summary>
        public ushort Status { get; set; }

        public const ushort STATUS_UNSEND = 0;
        public const ushort STATUS_SENDING = 1;
        public const ushort STATUS_SENT = 2;
        public const ushort STATUS_READ = 3;
        public const ushort STATUS_SEND_ERROR = 99;

        public virtual string StatusString {
            get {

                if (Status == STATUS_UNSEND) return "待发送";
                if (Status == STATUS_SENDING) return "发送中";
                if (Status == STATUS_SENT) return "未读";
                if (Status == STATUS_READ) return "已读";
                if (Status == STATUS_SEND_ERROR) return "发送错误";

                return "未知";
            }
        }

        /// <summary>
        /// 消息类型，例如：公告通知、系统提醒等
        /// </summary>
        public Guid TypeId { get; set; }
        public NotifyTypeDto Type { get; set; }
        /*public const ushort TYPE_SYSTEM_ANNOUNCEMENT = 1;
        public const ushort TYPE_SYSTEM_NOTIFY = 2;

        public virtual string TypeString {
            get {

                if (Status == TYPE_SYSTEM_ANNOUNCEMENT) return "公告通知";
                if (Status == TYPE_SYSTEM_NOTIFY) return "系统提醒";

                return "未知";
            }
        }*/
        /// <summary>
        /// 消息内容类型，例如：1.text 2.image 3.voice 4.video 5.file 6.news 7.mpnews  
        /// </summary>
        public ushort MessageType { get; set; }

        public const ushort MESSAGETYPE_TEXT = 1;
        public const ushort MESSAGETYPE_IMAGE = 2;
        public const ushort MESSAGETYPE_VOICE = 3;
        public const ushort MESSAGETYPE_VIDEO = 4;
        public const ushort MESSAGETYPE_FILE = 5;
        public const ushort MESSAGETYPE_NEWS = 6;
        public const ushort MESSAGETYPE_MPNEWS = 7;

        /// <summary>
        /// 发送人用户ID
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SendTime { get; set; }
        /// <summary>
        /// 接收人用户ID
        /// </summary>
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime? ReceiveTime { get; set; }

        /// <summary>
        /// 信息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 接收平台，1.网页平台 2.微信 3.短信 等  
        /// </summary>
        public ushort Agent { get; set; }
        public const ushort AGENT_WEB = 1;
        public const ushort AGENT_WECHAT = 2;
        public const ushort AGENT_SMS = 3;
        public virtual string AgentString {
            get {

                if (Status == AGENT_WEB) return "网页平台";
                if (Status == AGENT_WECHAT) return "微信";
                if (Status == AGENT_SMS) return "短信";

                return "未知";
            }
        }
    }
}
