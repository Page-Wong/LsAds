using LsAdmin.Application.Imp;
using LsAdmin.Application.ShoppingCartApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.ShoppingCartApp
{
    public interface IShoppingCartAppService : IBaseAppService<ShoppingCartDto>
    {
        List<ShoppingCartDto> GetCurrentUserAll();

        List<ShoppingCartDto> GetCurrentUserByType(ushort type);
    }
}
