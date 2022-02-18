using LsAdmin.Application.PlayerApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.OrderApp.Dtos;

namespace LsAdmin.Application.PlayerApp
{
    public interface IPlayerAppService : IBaseAppService<PlayerDto> {
        List<PlayerDto> GetByEquipmentId(Guid equipmentId);
        List<PlayerDto> GetByEquipmentIds(List<Guid> equipmentIds);

        List<PlayerDto> GetByType_OwnerUserId(Guid ownerUserId, PlayerType type, int startPage, int pageSize, out int rowCount);
        List<PlayerDto> GetByOwnerUserId(Guid ownerUserId, int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 获取所有者播放器列表
        /// </summary>
        /// <param name="ownerUserId">播放器所有者id</param>
        /// <returns>播放器列表</returns>
        List<PlayerDto> GetByOwnerUserId(Guid ownerUserId);

        List<PlayerDto> GetCanSetByOwnerUserId(Guid ownerUserId, int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 获取播放器类型为我的推广的 OrderPlayerProgram 列表
        /// </summary>
        /// <param name="playerId">播放器ID</param>
        /// <returns></returns>
        List<OrderPlayerProgramDto> GetMyProgramsOrderPlayerProgramsByPlayerId(Guid playerId);
        /// <summary>
        /// / 获取播放器所有 OrderPlayerProgram 列表
        /// </summary>
        /// <param name="playerId">播放器ID</param>
        /// <returns></returns>
        List<OrderPlayerProgramDto> GetAllOrderPlayerProgramsByPlayerId(Guid playerId);

        bool GetPlayersAllInfo(ref List<PlayerDto> players);


    }
}
