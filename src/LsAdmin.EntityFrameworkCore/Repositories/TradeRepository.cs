using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class TradeRepository : LsAdminRepositoryBase<Trade>, ITradeRepository {
        public TradeRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {
        }
    }
}
