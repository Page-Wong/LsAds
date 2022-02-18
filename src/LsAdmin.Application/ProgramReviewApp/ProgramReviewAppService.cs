using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.ProgramReviewApp.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using AutoMapper;
using System.Collections.Generic;
using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.PlaceApp;
using System.Linq;
using LsAdmin.Application.PlayerApp;
using LsAdmin.Application.PlayerProgramApp;
using Microsoft.EntityFrameworkCore;

namespace LsAdmin.Application.ProgramReviewApp {
    public class ProgramReviewAppService : BaseAppService<ProgramReview, ProgramReviewDto>, IProgramReviewAppService {

        private readonly IProgramReviewRepository _repository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IPlaceAppService _placeService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerProgramRepository _playerProgramRepository;
        
        public ProgramReviewAppService(IProgramReviewRepository repository, IEquipmentRepository equipmentRepository, IPlaceAppService placeService, IPlayerRepository playerRepository, IPlayerProgramRepository playerProgramRepository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _equipmentRepository = equipmentRepository;
            _placeService = placeService;
            _playerRepository = playerRepository;
            _playerProgramRepository = playerProgramRepository;
        }

        public ProgramReviewDto Get(Guid reviewerId, Guid programId) {
            return Mapper.Map<ProgramReviewDto>(_repository.FirstOrDefault(it => it.ProgramId == programId && it.ReviewerId == reviewerId));
        }

        public List<ProgramDto> LoadProgramPageListByUserId(Guid userId, int startPage, int pageSize, out int rowCount) {
            var places = _placeService.GetUserAllPlaces(userId);
            var equipments = _equipmentRepository.GetAllList(it => it.PlaceId != null && places.Select(p => p.Id).Contains(it.PlaceId.Value));
            var players = _playerRepository.GetAllList(it => equipments.Select(e => e.Id).Contains(it.EquipmentId));
            var allowPlayerProgramStatus = new List<PlayerProgramStatus> { PlayerProgramStatus.Unpublished, PlayerProgramStatus.Ready, PlayerProgramStatus.Playing };
            var programs = _playerProgramRepository.GetEntities().Where(it => allowPlayerProgramStatus.Contains(it.Status) && players.Select(p => p.Id).Contains(it.PlayerId)).Include(it => it.Program).Select(it => it.Program).Distinct().OrderBy(it => it.CreateTime).ToList();            
            //programs = programs.Select(it => new { program = it, programReview = _repository.FirstOrDefault(pr => pr.ProgramId == it.Id) }).OrderBy(it => it.programReview?.Id).Select(it => it.program).ToList();
            rowCount = programs.Count();
            return Mapper.Map<List<ProgramDto>>(programs.Skip((startPage-1)* pageSize).Take(pageSize));
        }
    }
}
