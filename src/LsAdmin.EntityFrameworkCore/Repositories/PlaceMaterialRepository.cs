using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class PlaceMaterialRepository : LsAdminRepositoryBase<PlaceMaterial>, IPlaceMaterialRepository
    {
        public PlaceMaterialRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
