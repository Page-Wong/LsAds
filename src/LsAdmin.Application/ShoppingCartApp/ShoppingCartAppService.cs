using AutoMapper;
using LsAdmin.Application.Imp;
using LsAdmin.Application.ShoppingCartApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Application.ShoppingCartApp
{
    public class ShoppingCartAppService:BaseAppService<ShoppingCart,ShoppingCartDto>, IShoppingCartAppService
    {
        private readonly IShoppingCartRepository _repository;

        public ShoppingCartAppService(IShoppingCartRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public List<ShoppingCartDto> GetCurrentUserAll()
        {
            return Mapper.Map<List<ShoppingCartDto>>(_repository.GetEntities().Where(o=>o.CreateUserId==CurrentUser.Id).ToList());
        }

        public List<ShoppingCartDto> GetCurrentUserByType(ushort type)
        {
            return Mapper.Map<List<ShoppingCartDto>>(_repository.GetEntities().Where(o => o.CreateUserId == CurrentUser.Id && o.Type==type).ToList());
        }
    }
}
