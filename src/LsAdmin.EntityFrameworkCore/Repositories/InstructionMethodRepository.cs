using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System.Linq;

namespace LsAdmin.EntityFrameworkCore.Repositories {
    public class InstructionMethodRepository : LsAdminRepositoryBase<InstructionMethod>, IInstructionMethodRepository {
        public InstructionMethodRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }

        public InstructionMethod GetByName(string name) {
            return GetEntities().FirstOrDefault(it => it.Name == name);
        }
    }
}
