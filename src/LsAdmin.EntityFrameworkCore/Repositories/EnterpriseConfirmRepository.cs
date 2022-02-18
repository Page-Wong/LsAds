using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    
    public class EnterpriseConfirmRepository : LsAdminRepositoryBase<EnterpriseConfirm>, IEnterpriseConfirmRepository
    {
        public EnterpriseConfirmRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
