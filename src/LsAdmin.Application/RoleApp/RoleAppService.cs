using AutoMapper;
using LsAdmin.Application.Imp;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.RoleApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.RoleApp
{
    public class RoleAppService : BaseAppService<Role, RoleDto>, IRoleAppService
    {
        private readonly IRoleRepository _repository;
        public RoleAppService(IRoleRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }        

        /// <summary>
        /// 根据角色获取权限
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetAllMenuListByRole(Guid roleId)
        {
            return _repository.GetAllMenuListByRole(roleId);
        }

        /// <summary>
        /// 更新角色权限关联关系
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="roleMenus">角色权限集合</param>
        /// <returns></returns>
        public bool UpdateRoleMenu(Guid roleId, List<RoleMenuDto> roleMenus)
        {
            return _repository.UpdateRoleMenu(roleId, Mapper.Map<List<RoleMenu>>(roleMenus));
        }

        /// <summary>
        /// 根据Code获取实体
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual RoleDto GetByCode(string code) {
            return Mapper.Map<RoleDto>(_repository.FirstOrDefault(r => r.Code == code));
        }
    }
}
