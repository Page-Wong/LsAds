using LsAdmin.Application.Imp;
using LsAdmin.Application.RoleApplyApp.Dtos;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.RoleApplyApp {
    public interface IRoleApplyAppService : IBaseAppService<RoleApplyDto> {

        List<RoleApplyDto> GetListByApplyUserId(Guid userId);
        bool PassApply(Guid id);
        bool UpPassApply(Guid id);
    }
}
