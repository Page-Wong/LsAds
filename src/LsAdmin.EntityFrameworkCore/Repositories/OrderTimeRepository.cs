using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class OrderTimeRepository : LsAdminRepositoryBase<OrderTime>,IOrderTimeRepository
    {
        public OrderTimeRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
