using System;

namespace LsAdmin.Domain.Entities {
    public class Notify : BaseEntity
    {
        /// <summary>
        /// 状态:99.发送错误 0.待发送 1.正在发送 2.已发送 3.已接收
        /// </summary>
        public ushort Status { get; set; }

        /// <summary>
        /// 消息类型，例如：公告通知、系统提醒等
        /// </summary>
        public NotifyType Type { get; set; }

        /// <summary>
        /// 消息内容类型，例如：1.text 2.image 3.voice 4.video 5.file 6.news 7.mpnews  
        /// </summary>
        public ushort MessageType { get; set; }

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

    }
}
