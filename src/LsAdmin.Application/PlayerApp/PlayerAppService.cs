using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.PlayerApp.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using LsAdmin.Application.PlayerProgramApp;
using LsAdmin.Application.ProgramApp;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.OrderApp.Dtos;

namespace LsAdmin.Application.PlayerApp {
    public class PlayerAppService : BaseAppService<Player, PlayerDto>, IPlayerAppService {

        private readonly IPlayerRepository _repository;
        private readonly IProgramAppService _programService;
        private readonly IEquipmentAppService _equipmentService;
        public PlayerAppService(IPlayerRepository repository, IHttpContextAccessor httpContextAccessor,
                                IProgramAppService programService, IEquipmentAppService equipmentService) : base(repository, httpContextAccessor) {
            _repository = repository;
            _programService = programService;
            _equipmentService = equipmentService;
        }

        public List<OrderPlayerProgramDto> GetAllOrderPlayerProgramsByPlayerId(Guid playerId)
        {
            var items= Mapper.Map<List<OrderPlayerProgramDto>>(_repository.GetAllOrderPlayerProgramsByPlayerId(playerId));
            items.ForEach(it => it.PlayerProgram.Program= _programService.Get(it.PlayerProgram.ProgramId));
            return items;
        }

        public List<PlayerDto> GetByEquipmentId(Guid equipmentId) {
            return Mapper.Map <List<PlayerDto>>(_repository.GetAllList(it => it.EquipmentId == equipmentId));
        }

        public List<PlayerDto> GetByEquipmentIds(List<Guid> equipmentIds) {
            return Mapper.Map<List<PlayerDto>>(_repository.GetAllList(it => equipmentIds.Contains(it.EquipmentId)));
        }

        public List<PlayerDto> GetByOwnerUserId(Guid ownerUserId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<PlayerDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.OwnerUserId == ownerUserId , it => it.CreateTime));
        }

        public List<PlayerDto> GetByOwnerUserId(Guid ownerUserId)
        {
            return Mapper.Map<List<PlayerDto>>(_repository.GetEntities().Where(w => w.OwnerUserId == ownerUserId).OrderBy(o => o.EquipmentId));
        }


        public List<PlayerDto> GetByType_OwnerUserId(Guid ownerUserId, PlayerType type, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<PlayerDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.OwnerUserId == ownerUserId && it.Type== type, it => it.CreateTime));
        }

        public List<PlayerDto> GetCanSetByOwnerUserId(Guid ownerUserId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<PlayerDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.OwnerUserId == ownerUserId &&  (new PlayerType[]{ PlayerType.Private, PlayerType.Public}).Contains(it.Type) , it => it.CreateTime));
        }

        public List<OrderPlayerProgramDto> GetMyProgramsOrderPlayerProgramsByPlayerId(Guid playerId)
        {
            var items = Mapper.Map<List<OrderPlayerProgramDto>>(_repository.GetMyProgramsOrderPlayerProgramsByPlayerId(playerId));
            items.ForEach(it => it.PlayerProgram.Program = _programService.Get(it.PlayerProgram.ProgramId));
            return items;
        }

        public bool GetPlayersAllInfo(ref List<PlayerDto> players)
        {
            //   var temp =  DeepCopyHelper.Copy(players); //克隆一个对象  
            try
            {
                players.ForEach(it => {
                    //  it.PlayerPrograms = _playerProgramService.GetByPlayerId(it.Id)?.ToList();
                    it.Equipment = _equipmentService.Get(it.EquipmentId);
                });
                //players.ForEach(it => it.PlayerPrograms.ForEach(p => p.Program = _programService.Get(p.ProgramId)));
                return true;
            }
            catch (Exception ex)
            {
                // players = (List<PlayerDto> )temp;
                return false;
            }
        }
    }
}
