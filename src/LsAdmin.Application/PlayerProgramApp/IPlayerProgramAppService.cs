using LsAdmin.Application.PlayerProgramApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using LsAdmin.Domain.Entities;

namespace LsAdmin.Application.PlayerProgramApp {
    public interface IPlayerProgramAppService : IBaseAppService<PlayerProgramDto> {
        List<PlayerProgramDto> GetByPlayerIds(List<Guid> playerIds);
        List<PlayerProgramDto> GetByPlayerId(Guid playerId);
        List<PlayerProgramDto> GetByProgramIds(List<Guid> programIds);

        List<PlayerProgramDto> GetMyProgramByPlayerId(Guid playerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerProgramId">playerProgramId</param>
        /// <param name="toStatus"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdateStatus(Guid playerProgramId, PlayerProgramStatus toStatus, out string errorMessage);

    }
}
