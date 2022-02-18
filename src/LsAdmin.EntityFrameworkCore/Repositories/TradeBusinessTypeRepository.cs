using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories {
    public class TradeBusinessTypeRepository : LsAdminRepositoryBase<TradeBusinessType>, ITradeBusinessTypeRepository {
        public TradeBusinessTypeRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {
        }
    }
}
