using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class MaterialRepository : LsAdminRepositoryBase<Material>, IMaterialRepository {
        public MaterialRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
