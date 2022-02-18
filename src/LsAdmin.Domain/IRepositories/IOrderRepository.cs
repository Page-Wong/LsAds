using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.IRepositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IQueryable<Trade> GetTradesByOrderId(Guid id);
        IQueryable<Trade> GetTradesWithBusinessTypeByOrderId(Guid id);

        /// <summary>
        /// 带有播放器信息的订单
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>订单</returns>
        Order GetWithPlayers(Guid id);
        /// <summary>
        /// 带有可播放节目信息的订单
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>订单</returns>
        Order GetWithPlayerPrograms(Guid id);
        /// <summary>
        /// 更新订单的播放器
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="resources">播放器列表</param>
        /// <returns>操作是否成功</returns>
        bool UpdatePlayers(Guid id, List<Player> resources);

        /// <summary>
        /// 更新订单的播放器
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="playerId">播放器列表</param>
        /// <returns>操作是否成功</returns>
        bool UpdateOrderPlayer(Guid id, Guid playerId);
        /// <summary>
        /// 更新订单的可播放节目
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="resources">可播放节目列表</param>
        /// <returns>操作是否成功</returns>
        bool UpdatePlayerPrograms(Guid id, List<PlayerProgram> resources);

        /// <summary>
        /// 更新订单的可播放节目
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="playerProgramId">可播放节目列表</param>
        /// <returns>操作是否成功</returns>
        bool UpdatePlayerProgram(Guid id, Guid playerProgramId);

        /// <summary>
        /// 移除订单所有的可播放节目
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>操作是否成功</returns>
        bool RemovePlayerPrograms(Guid id);

        /// <summary>
        /// 移除订单所有的播放器
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>操作是否成功</returns>
        bool RemovePlayers(Guid id);


    }
}
