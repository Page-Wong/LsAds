using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class EquipmentRepairRepository : LsAdminRepositoryBase<EquipmentRepair>, IEquipmentRepairRepository{
        public EquipmentRepairRepository(LsAdminDbContext dbcontext) : base(dbcontext){

        }
        
        public override IQueryable<EquipmentRepair> GetEntities(){
            return _dbContext.Set<EquipmentRepair>().Include(item => item.Equipment).Include(i => i.Place);
        }


    }
}

