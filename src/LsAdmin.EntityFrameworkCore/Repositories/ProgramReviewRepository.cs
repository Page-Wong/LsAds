using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class ProgramReviewRepository : LsAdminRepositoryBase<ProgramReview>, IProgramReviewRepository {
        public ProgramReviewRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }
    }
}
