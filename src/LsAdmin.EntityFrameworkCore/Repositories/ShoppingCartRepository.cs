using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LsAdmin.EntityFrameworkCore.Repositories { 
	
	public class ShoppingCartRepository : LsAdminRepositoryBase<ShoppingCart>,IShoppingCartRepository{
		public ShoppingCartRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }

	}

}