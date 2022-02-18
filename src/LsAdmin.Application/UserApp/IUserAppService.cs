using LsAdmin.Application.Imp;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.UserApp
{
    public interface IUserAppService : IBaseAppService<UserDto> {
        UserDto CheckUser(string userName, string password);

        UserDto CheckUser(string userName);
        UserDto CheckMobileNumber(string mobileNumber);
        UserDto CheckEMail(string email);
        UserDto CheckWxUnionId(string wxUnionId);

        List<UserDto> GetUserByDepartment(Guid departmentId, int startPage, int pageSize, out int rowCount);

        bool ChangePassword(UserDto dto, string password);
        bool ChangePaymentPassword(UserDto dto, string password);
        bool AddRole(Guid id, Guid roleId);
        bool RemoveRole(Guid id, Guid roleId);
    }
}
