using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class MenuRepository : LsAdminRepositoryBase<Menu>, IMenuRepository {
        public MenuRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
