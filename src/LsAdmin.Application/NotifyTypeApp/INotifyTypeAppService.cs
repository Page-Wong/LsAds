using LsAdmin.Application.NotifyTypeApp.Dtos;
using LsAdmin.Application.Imp;
using System.Collections.Generic;
using System;

namespace LsAdmin.Application.NotifyTypeApp {
    public interface INotifyTypeAppService : IBaseAppService<NotifyTypeDto>
    {
        NotifyTypeDto GetByName(string name);
    }
}
