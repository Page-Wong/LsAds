using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class PreRegisteredDeviceRepository : LsAdminRepositoryBase<PreRegisteredDevice>, IPreRegisteredDeviceRepository
    {
        public PreRegisteredDeviceRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
