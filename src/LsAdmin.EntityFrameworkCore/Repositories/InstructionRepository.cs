using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class InstructionRepository : LsAdminRepositoryBase<Instruction>, IInstructionRepository {
        public InstructionRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }
    }
}
