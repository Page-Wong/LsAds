using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    class EquipmentModelRepository : LsAdminRepositoryBase<EquipmentModel>, IEquipmentModelRepository
    {
        public EquipmentModelRepository(LsAdminDbContext dbcontext) : base(dbcontext){
        }
    }
}


