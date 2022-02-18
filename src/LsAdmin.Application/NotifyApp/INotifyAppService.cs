using LsAdmin.Application.NotifyApp.Dtos;
using LsAdmin.Application.Imp;
using System.Collections.Generic;
using System;
using LsAdmin.Application.NotifyTypeApp.Dtos;

namespace LsAdmin.Application.NotifyApp {
    public interface INotifyAppService : IBaseAppService<NotifyDto>
    {
        #region 网页平台消息
        bool SendWebNotifyToUser(Guid userId, string typeName, string[] messageArg);
        bool SendWebNotifyTodDepartment(Guid departmentId, string typeName, string[] messageArg);
        bool CurrentUserReadWebNotify(NotifyDto notify);
        List<NotifyDto> GetCurrentUserWebNotifyAllPageList(int startPage, int pageSize, out int rowCount);
        List<NotifyDto> GetCurrentUserWebNotifyUnreadPageList(int startPage, int pageSize, out int rowCount);
        int GetCurrentUserWebNotifyUnreadCount();

        #endregion
    }
}
