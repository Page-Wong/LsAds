using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class LabelRepository : LsAdminRepositoryBase<Label>, ILabelRepository 
    {
        public LabelRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }
    }
}
