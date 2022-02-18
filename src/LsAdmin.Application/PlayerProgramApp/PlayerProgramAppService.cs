using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.PlayerProgramApp.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using AutoMapper;
using LsAdmin.Application.ProgramApp;
using LsAdmin.Application.OrderApp.Dtos;
using System.Linq;
using LsAdmin.Application.OrderApp;

namespace LsAdmin.Application.PlayerProgramApp
{
    public class PlayerProgramAppService : BaseAppService<PlayerProgram, PlayerProgramDto>, IPlayerProgramAppService {

        private readonly IPlayerProgramRepository _repository;
        private readonly IProgramAppService _programService;
        private readonly IOrderRepository _orderRepository;

        public PlayerProgramAppService(IPlayerProgramRepository repository, IHttpContextAccessor httpContextAccessor, IProgramAppService programService, IOrderRepository orderRepository) : base(repository, httpContextAccessor) {
            _repository = repository;
            _programService = programService;
            _orderRepository = orderRepository;
        }

        public List<PlayerProgramDto> GetByPlayerId(Guid playerId)
        {
            var playerPrograms= Mapper.Map<List<PlayerProgramDto>>(_repository.GetAllList(it => it.PlayerId == playerId));
            if(playerPrograms!= null)  playerPrograms.ForEach(it => it.Program = _programService.Get(it.ProgramId));
            return playerPrograms;
            //return  Mapper.Map<List<PlayerProgramDto>>(_repository.GetAllList(it => it.PlayerId == playerIds));
        }

        public List<PlayerProgramDto> GetByPlayerIds(List<Guid> playerIds)
        {
            return Mapper.Map<List<PlayerProgramDto>>(_repository.GetAllList(it => playerIds.Contains(it.PlayerId)));
        }

        public List<PlayerProgramDto> GetByProgramIds(List<Guid> programIds) {
            return Mapper.Map<List<PlayerProgramDto>>(_repository.GetAllList(it => programIds.Contains(it.ProgramId)));
        }

        public List<PlayerProgramDto> GetMyProgramByPlayerId(Guid playerId)
        {
            var items = _repository.GetWithOrderPlayerProgramsByPlayerId(playerId);

            List<PlayerProgramDto> result = new List<PlayerProgramDto>();

            foreach(var item in items)
            {
                if(item.OrderPlayerPrograms.Count>0 /*&&  item.OrderPlayerPrograms.FirstOrDefault(w => w.Order.Type == 2) != null*/)
                {
                    result.Add(Mapper.Map<PlayerProgramDto>(item));
                }
            }
           // result.ForEach(each => each.Program = _programService.Get(each.ProgramId));
            return result;
        }

        public bool UpdateStatus(Guid playerProgramId, PlayerProgramStatus toStatus, out string errorMessage)
        {
            errorMessage = "";
           var item = Get(playerProgramId);
            if (item == null)
            {
                errorMessage = "系统不存在所指定需要更播放状态的播放器播放节目记录！";
                return false;
            }

            item.Status = toStatus;
            if (Update(item) == false)            {
                errorMessage = "数据更新失败！";
                return false;
            }

            return true;
        }
    }
}
