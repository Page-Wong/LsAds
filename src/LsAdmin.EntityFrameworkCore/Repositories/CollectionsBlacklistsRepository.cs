using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories {

    public class CollectionsBlacklistsRepository : LsAdminRepositoryBase<CollectionsBlacklists>, ICollectionsBlacklistsRepository
    {
        public CollectionsBlacklistsRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }

    }

}