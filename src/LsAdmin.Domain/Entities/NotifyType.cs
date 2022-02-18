using System;

namespace LsAdmin.Domain.Entities {
    public class NotifyType : BaseEntity {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string WebMessageTemplate { get; set; }

        public string WechatMessageTemplate { get; set; }

        public string SmsMessageTemplate { get; set; }      

    }
}
