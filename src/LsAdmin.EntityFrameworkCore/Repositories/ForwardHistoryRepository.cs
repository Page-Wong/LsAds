using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories {
    public class ForwardHistoryRepository : LsAdminRepositoryBase<ForwardHistory>, IForwardHistoryRepository {
        public ForwardHistoryRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
