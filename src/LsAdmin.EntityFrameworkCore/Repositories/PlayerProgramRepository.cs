using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class PlayerProgramRepository : LsAdminRepositoryBase<PlayerProgram>, IPlayerProgramRepository {
        public PlayerProgramRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }

        public IQueryable<PlayerProgram> GetWithOrderPlayerProgramsByPlayerId(Guid playerId)
        {
            var items = GetEntities().Include(i => i.Program).Where(w => w.PlayerId == playerId);
           /* items.ForEachAsync(item =>
                item.OrderPlayerPrograms = _dbContext.Set<OrderPlayerProgram>().Where(it => it.PlayerProgramId == item.Id).ToList()
                );*/
            foreach(var item in items)
            {
                item.OrderPlayerPrograms = _dbContext.Set<OrderPlayerProgram>().Include(i => i.Order).Where(it => it.PlayerProgramId == item.Id).ToList();
            }

           return items;
        }

        public PlayerProgram GetWithOrderPlayerPrograms(Guid id)
        {
            var item = Get(id);
            if(item != null) {
                item.OrderPlayerPrograms = _dbContext.Set<OrderPlayerProgram>().Where(it => it.PlayerProgramId == id).ToList();
            }
            return item;
        }
    }
}
