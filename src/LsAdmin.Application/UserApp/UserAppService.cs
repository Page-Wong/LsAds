using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Application.UserApp.Dtos;
using AutoMapper;
using System.Text;
using System.Security.Cryptography;
using LsAdmin.Application.Imp;
using LsAdmin.Utility.Convert;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.RoleApp;

namespace LsAdmin.Application.UserApp
{
    /// <summary>
    /// 用户管理服务
    /// </summary>
    public class UserAppService : BaseAppService<User, UserDto>, IUserAppService
    {
        //用户管理仓储接口
        private readonly IUserRepository _repository;
        private readonly IRoleAppService _roleService;

        /// <summary>
        /// 构造函数 实现依赖注入
        /// </summary>
        /// <param name="userRepository">仓储对象</param>
        public UserAppService(IUserRepository repository, IRoleAppService roleService, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _roleService = roleService;
        }

        public UserDto CheckMobileNumber(string mobileNumber) {
            return Mapper.Map<UserDto>(_repository.GetEntities().FirstOrDefault(u => u.MobileNumber == mobileNumber));
        }

        public UserDto CheckUser(string userName) {
            return Mapper.Map<UserDto>(_repository.FindByUsername(userName));
        }

        public UserDto CheckUser(string userName, string password)
        {
            
            return Mapper.Map<UserDto>(_repository.CheckUser(userName, PasswordConvertHelper.Create(password)));
            //return _repository.CheckUser(userName, Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password))));
        }
        public List<UserDto> GetUserByDepartment(Guid departmentId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<UserDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.DepartmentId == departmentId, it => it.CreateTime));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public override bool Insert(ref UserDto dto) {
            dto.Password = PasswordConvertHelper.Create(dto.Password);
            return base.Insert(ref dto);
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public override UserDto Get(Guid id) {
            return Mapper.Map<UserDto>(_repository.GetWithRoles(id));
        }

        /// <summary>
        /// 修改登录密码
        /// </summary>
        /// <param name="dto">用户</param>
        /// <param name="password">登录密码</param>
        /// <returns></returns>
        public bool ChangePassword(UserDto dto, string password) {
            dto.Password = PasswordConvertHelper.Create(password);
            return Update(dto);
        }

        /// <summary>
        /// 修改支付密码
        /// </summary>
        /// <param name="dto">用户</param>
        /// <param name="password">支付密码</param>
        /// <returns></returns>
        public bool ChangePaymentPassword(UserDto dto, string password) {
            dto.PaymentPassword = PasswordConvertHelper.Create(password);
            return Update(dto);
        }

        public UserDto CheckEMail(string email) {
            return Mapper.Map<UserDto>(_repository.GetEntities().FirstOrDefault(u => u.EMail == email));
        }

        #region 权限相关
        public bool AddRole(Guid id, Guid roleId) {
            var user = Get(id);
            if (user == null) {
                return false;
            }
            var role = _roleService.Get(roleId);
            if (role == null) {
                return false;
            }
            var roles = user.UserRoles.Select(item => item.Role).ToList();
            roles.Add(role);            
            return _repository.UpdateRoles(id, Mapper.Map<List<Role>>(roles));
        }

        public bool RemoveRole(Guid id, Guid roleId) {
            var user = Get(id);
            if (user == null) {
                return false;
            }
            var role = _roleService.Get(roleId);
            if (role == null) {
                return false;
            }
            user.UserRoles.RemoveAll(item => item.UserId == id && item.RoleId == roleId);
            return Update(user);
        }

        public UserDto CheckWxUnionId(string wxUnionId) {
            return Mapper.Map<UserDto>(_repository.GetEntities().FirstOrDefault(u => u.WxUnionId == wxUnionId));
        }
        #endregion
    }
}
