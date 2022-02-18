using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using AutoMapper;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;

namespace LsAdmin.Application.MenuApp
{
    public class MenuAppService : BaseAppService<Menu, MenuDto>, IMenuAppService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMenuRepository _repository;
        public MenuAppService(IMenuRepository repository, IUserRepository userRepository, IRoleRepository roleRepository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _orderby = m => m.SerialNumber;
        }

        public List<MenuDto> GetMenusByParent(Guid parentId, int startPage, int pageSize, out int rowCount)
        {
            var menus = _repository.LoadPageList(startPage, pageSize, out rowCount, it => it.ParentId == parentId, _orderby);
            return Mapper.Map<List<MenuDto>>(menus);
        }
        
        /// <summary>
        /// 根据用户获取功能菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<MenuDto> GetMenusByUser(Guid userId)
        {
            /*List<MenuDto> result = new List<MenuDto>();
            var allMenus = _repository.GetAllList(it=>it.Type == 0).OrderBy(it => it.SerialNumber);
            if (userId == Guid.Empty) //超级管理员
                return Mapper.Map<List<MenuDto>>(allMenus);
            var user = _userRepository.GetWithRoles(userId);
            if (user == null)
                return result;
            var userRoles = user.UserRoles;
            List<Guid> menuIds = new List<Guid>();
            foreach (var role in userRoles)
            {
                menuIds = menuIds.Union(_roleRepository.GetAllMenuListByRole(role.RoleId)).ToList();
            }
            allMenus = allMenus.Where(it => menuIds.Contains(it.Id)).OrderBy(it => it.SerialNumber);
            return Mapper.Map<List<MenuDto>>(allMenus);*/
            return GetAllMenusByUser(userId).Where(m => m.Type == 0).ToList();
        }



        public List<MenuDto> GetAllMenusByUser(Guid userId) {
            List<MenuDto> result = new List<MenuDto>();
            if (userId == Guid.Empty) //超级管理员
                return Mapper.Map<List<MenuDto>>(_repository.GetAllList().OrderBy(it => it.SerialNumber));
            var user = _userRepository.GetWithRoles(userId);
            if (user == null)
                return result;
            var userRoles = user.UserRoles;
            List<Guid> menuIds = new List<Guid>();
            foreach (var role in userRoles) {
                menuIds = menuIds.Union(_roleRepository.GetAllMenuListByRole(role.RoleId)).ToList();
            }
            var allMenus = _repository.GetAllList(it => menuIds.Contains(it.Id)).OrderBy(it => it.SerialNumber);
            return Mapper.Map<List<MenuDto>>(allMenus);
        }
    }
}
