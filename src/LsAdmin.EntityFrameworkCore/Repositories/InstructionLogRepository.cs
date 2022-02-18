using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class InstructionLogRepository : LsAdminRepositoryBase<InstructionLog>, IInstructionLogRepository {
        public InstructionLogRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }
    }
}
