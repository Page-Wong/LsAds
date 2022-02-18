using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class EquipmentReplaceRepository : LsAdminRepositoryBase<EquipmentReplace>, IEquipmentReplaceRepository
    {
        public EquipmentReplaceRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
