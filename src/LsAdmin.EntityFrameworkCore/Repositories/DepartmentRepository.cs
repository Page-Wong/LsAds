using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class DepartmentRepository : LsAdminRepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }

        public Department FindByCode(string code) {
            return _dbContext.Set<Department>().FirstOrDefault(d => d.Code.Equals(code));
        }
    }
}
