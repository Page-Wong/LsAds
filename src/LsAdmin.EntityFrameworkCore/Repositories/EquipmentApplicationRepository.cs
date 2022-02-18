using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class EquipmentApplicationRepository : LsAdminRepositoryBase<EquipmentApplication>, IEquipmentApplicationRepository
    {
        public EquipmentApplicationRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
