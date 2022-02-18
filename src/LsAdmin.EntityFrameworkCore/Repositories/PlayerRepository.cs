using System;
using System.Linq;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories {
    public class PlayerRepository : LsAdminRepositoryBase<Player>, IPlayerRepository {

        public PlayerRepository(LsAdminDbContext dbContext, IPlayerProgramRepository playerProgramRepository) : base(dbContext)
        {

        }

        public IQueryable<OrderPlayerProgram> GetAllOrderPlayerProgramsByPlayerId(Guid playerId)
        {
            var result = from pp in _dbContext.Set<PlayerProgram>()
                         join opp in _dbContext.Set<OrderPlayerProgram>() on pp.Id equals opp.PlayerProgramId
                         join o in _dbContext.Set<Order>() on opp.OrderId equals o.Id
                         where pp.PlayerId == playerId
                         select new OrderPlayerProgram
                         {
                             OrderId = opp.OrderId,
                             PlayerProgramId = opp.PlayerProgramId,
                             Order = o,
                             PlayerProgram =  pp ,
                         };

            return result;
        }

        public IQueryable<OrderPlayerProgram> GetMyProgramsOrderPlayerProgramsByPlayerId(Guid playerId)
        {
            var result = from pp in _dbContext.Set<PlayerProgram>()
                         join opp in _dbContext.Set<OrderPlayerProgram>() on pp.Id equals opp.PlayerProgramId
                         join o in _dbContext.Set<Order>() on opp.OrderId equals o.Id
                         where pp.PlayerId == playerId && o.Type == 12
                         select new OrderPlayerProgram {
                            OrderId=  opp.OrderId,
                            PlayerProgramId= opp.PlayerProgramId,
                            Order=  o,
                            PlayerProgram= pp
                          };
            return result;

        }
    }
}
