using System;

namespace LsAdmin.Application.NotifyTypeApp.Dtos {
    public class NotifyTypeDto {
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public Guid? CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string WebMessageTemplate { get; set; }

        public string WechatMessageTemplate { get; set; }

        public string SmsMessageTemplate { get; set; }
    }
}
