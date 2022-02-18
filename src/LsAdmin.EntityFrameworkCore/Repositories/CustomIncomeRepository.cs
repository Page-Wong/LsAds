using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class CustomIncomeRepository : LsAdminRepositoryBase<CustomIncome>, ICustomIncomeRepository
    {
        public CustomIncomeRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
