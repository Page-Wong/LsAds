using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class OrderRepository : LsAdminRepositoryBase<Order>, IOrderRepository {
        public OrderRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }

        public IQueryable<Trade> GetTradesByOrderId(Guid id) {
            return _dbContext.Set<Trade>().Where(t => _dbContext.Set<OrderTrade>().Where(item => item.OrderId == id).Select(item => item.TradeId).Contains(t.Id));
        }

        public IQueryable<Trade> GetTradesWithBusinessTypeByOrderId(Guid id) => GetTradesByOrderId(id).Include(item => item.BusinessType);

        public Order GetWithPlayerPrograms(Guid id) {
            var item = Get(id);
            if (item != null) {
                item.OrderPlayerPrograms = _dbContext.Set<OrderPlayerProgram>().Include(it => it.PlayerProgram).Where(it => it.OrderId == id).ToList();
            }
            return item;
        }

        public Order GetWithPlayers(Guid id) {
            var item = Get(id);
            if (item != null) {
                item.OrderPlayers = _dbContext.Set<OrderPlayer>().Include(it => it.Player).Where(it => it.OrderId == id).ToList();
            }
            return item;
        }

        public bool UpdatePlayerPrograms(Guid id, List<PlayerProgram> resources) {
            var item = GetWithPlayerPrograms(id);
            if (item == null) {
                return false;
            }
            using (var transaction = _dbContext.Database.BeginTransaction()) {
                try {
                    _dbContext.Set<OrderPlayerProgram>().Where(it => it.OrderId == id).ToList().ForEach(it => _dbContext.Set<OrderPlayerProgram>().Remove(it));
                    Save();
                    foreach (var resource in resources.Distinct()) {
                        _dbContext.Set<OrderPlayerProgram>().Add(new OrderPlayerProgram { OrderId = item.Id, PlayerProgramId = resource.Id });
                        Save();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception) {
                    transaction.Rollback();
                }
            }
            return false;
        }

        public bool RemovePlayerPrograms(Guid id)
        {
            var item = GetWithPlayerPrograms(id);
            if (item == null)
            {
                return false;
            }
            try
            {
                foreach(var orderprogram in item.OrderPlayerPrograms.Select(s=>s.PlayerProgram))
                {
                    _dbContext.Set<PlayerProgram>().Where(it => it.Id == orderprogram.Id).ToList().ForEach(it => _dbContext.Set<PlayerProgram>().Remove(it));
                }
                _dbContext.Set<OrderPlayerProgram>().Where(it => it.OrderId == id).ToList().ForEach(it => _dbContext.Set<OrderPlayerProgram>().Remove(it));                
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemovePlayers(Guid id)
        {
            var item = GetWithPlayers(id);
            if (item == null)
            {
                return false;
            }
            try
            {
                _dbContext.Set<OrderPlayer>().Where(it => it.OrderId == id).ToList().ForEach(it => _dbContext.Set<OrderPlayer>().Remove(it));
                Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePlayerProgram(Guid id, Guid playerProgramId)
        {
            try
            {
                _dbContext.Set<OrderPlayerProgram>().Add(new OrderPlayerProgram { OrderId = id, PlayerProgramId = playerProgramId });
                Save();
                return true;
            }
            catch (Exception)
            {
            return false;
            }           
        }

        public bool UpdatePlayers(Guid id, List<Player> resources) {
            var item = GetWithPlayers(id);
            if (item == null) {
                return false;
            }
            using (var transaction = _dbContext.Database.BeginTransaction()) {
                try {
                    _dbContext.Set<OrderPlayer>().Where(it => it.OrderId == id).ToList().ForEach(it => _dbContext.Set<OrderPlayer>().Remove(it));
                    Save();
                    foreach (var resource in resources.Distinct()) {
                        _dbContext.Set<OrderPlayer>().Add(new OrderPlayer { OrderId = item.Id, PlayerId = resource.Id });
                        Save();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception) {
                    transaction.Rollback();
                }
            }
            return false;
        }

        public bool UpdateOrderPlayer(Guid id,Guid playerId)
        {
            var item= GetWithPlayers(id);
            if (item == null)
            {
                return false;
            }
            try
            {
                /*_dbContext.Set<OrderPlayer>().Where(it => it.OrderId == id).ToList().ForEach(it => _dbContext.Set<OrderPlayer>().Remove(it));
                Save();*/
                _dbContext.Set<OrderPlayer>().Add(new OrderPlayer { OrderId = id, PlayerId = playerId });
                Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
