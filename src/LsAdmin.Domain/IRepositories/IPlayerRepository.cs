using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Domain.IRepositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        /// <summary>
        /// 获取播放器类型为我的推广的 OrderPlayerProgram 列表
        /// </summary>
        /// <param name="playerId">播放器ID</param>
        /// <returns></returns>
        IQueryable<OrderPlayerProgram> GetMyProgramsOrderPlayerProgramsByPlayerId(Guid playerId);
        /// <summary>
        /// / 获取播放器所有 OrderPlayerProgram 列表
        /// </summary>
        /// <param name="playerId">播放器ID</param>
        /// <returns></returns>
        IQueryable<OrderPlayerProgram> GetAllOrderPlayerProgramsByPlayerId(Guid playerId);
    }
}
