using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LsAdmin.EntityFrameworkCore.Repositories {
    public class NotifyRepository : LsAdminRepositoryBase<Notify>, INotifyRepository {
        public NotifyRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {
        }
    }
}
