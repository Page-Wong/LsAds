using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class EquipmentPlayerRepository : LsAdminRepositoryBase<Player>, IEquipmentPlayerRepository {
        public EquipmentPlayerRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
   
    }
}
