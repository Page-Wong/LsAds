using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class EquipmentIncomeRepository : LsAdminRepositoryBase<EquipmentIncome>, IEquipmentIncomeRepository
    {
        public EquipmentIncomeRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
