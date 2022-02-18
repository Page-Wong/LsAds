using LsAdmin.Application.UserApp;
using LsAdmin.Utility.Convert;
using LsAdmin.Utility.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.MVC.Models
{
    public class NotifyViewModel {
        private IUserAppService _userAppService;
        private IUserAppService UserAppService {
            get {
                if (_userAppService == null) {
                    _userAppService = (IUserAppService)HttpHelper.ServiceProvider.GetService(typeof(IUserAppService));
                }
                return _userAppService;
            }
        }
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public ushort Status { get; set; }
        public string StatusString { get; set; }
        public string TypeDisplayName { get; set; }
        public string Icon { get; set; }
        public DateTime? SendTime { get; set; }
        public string SendTimeString {
            get {
                return TimeConvertHelper.TimeDiffString(DateTime.Now, SendTime.Value);
            } }
        public DateTime? ReceiveTime { get; set; }
        public string Message { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderName {
            get {
                return UserAppService?.Get(SenderId)?.Name;
            }
        }
        public string ReceiverName { get {
                return UserAppService?.Get(ReceiverId)?.Name;
            }
        }
    }
}
