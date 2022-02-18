using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Domain.IRepositories
{
    public interface IPlayerProgramRepository : IRepository<PlayerProgram>
    {
        PlayerProgram GetWithOrderPlayerPrograms(Guid id);

        IQueryable<PlayerProgram> GetWithOrderPlayerProgramsByPlayerId(Guid playerId);
    }
}
