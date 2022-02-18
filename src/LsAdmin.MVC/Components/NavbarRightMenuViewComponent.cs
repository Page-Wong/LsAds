using LsAdmin.Application.NotifyApp;
using LsAdmin.Application.NotifyApp.Dtos;
using LsAdmin.Application.NotifyTypeApp;
using LsAdmin.Application.UserApp;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Utility.Convert;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LsAdmin.MVC.Components {
    [ViewComponent(Name = "NavbarRightMenu")]
    public class NavbarRightMenuViewComponent : ViewComponent {
        private readonly INotifyAppService _notifyAppService;
        private readonly INotifyTypeAppService _notifyTypeAppService;
        private readonly IUserAppService _userAppService;
        public NavbarRightMenuViewComponent(INotifyAppService notifyAppService, INotifyTypeAppService notifyTypeAppService, IUserAppService userAppService) {
            _notifyAppService = notifyAppService;
            _notifyTypeAppService = notifyTypeAppService;
            _userAppService = userAppService;
        }

        public IViewComponentResult Invoke()
        {          
            var user = ByteConvertHelper.Bytes2Object<UserDto>(HttpContext.Session.Get("CurrentUserWithRoles"));
            ViewBag.unreadNotifyCount = _notifyAppService.GetCurrentUserWebNotifyUnreadCount();
            ViewBag.notifyList = _notifyAppService.GetCurrentUserWebNotifyAllPageList(1,10, out int rowCount);
            /*if (rowCount == 0) {
                _notifyAppService.SendWebNotifyToUser(user.Id, "System_Notify", new string[] { "测试系统通知1", "#" });
                _notifyAppService.SendWebNotifyToUser(user.Id, "System_Notify", new string[] { "测试系统通知2", "#" });
                _notifyAppService.SendWebNotifyToUser(user.Id, "System_Notify", new string[] { "测试系统通知3", "#" });
                _notifyAppService.SendWebNotifyToUser(user.Id, "System_Notify", new string[] { "测试系统通知4", "#" });
                _notifyAppService.SendWebNotifyToUser(user.Id, "System_Notify", new string[] { "测试系统通知5", "#" });
            }*/
            return View(user);
        }
    }
}
