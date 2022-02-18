using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories {
    public class NotifyTypeRepository : LsAdminRepositoryBase<NotifyType>, INotifyTypeRepository {
        public NotifyTypeRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }        
    }
}
