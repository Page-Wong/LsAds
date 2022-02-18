using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.EquipmentApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;
using System;
using LsAdmin.Application.EquipmentRepairApp;
using LsAdmin.Application.EquipmentApp;
using System.Net.WebSockets;
using LsAdmin.Application.InstructionLogApp;
using LsAdmin.Application.InstructionApp.Dto;

namespace LsAdmin.Application.InstructionApp {
    public class InstructionAppService : BaseAppService<Instruction, InstructionDto>, IInstructionAppService {

        private readonly IInstructionRepository _repository;
        IInstructionLogAppService _instructionLogAppService;
        public InstructionAppService(IInstructionLogAppService instructionLogAppService, IInstructionRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _instructionLogAppService = instructionLogAppService;
        }

        public bool ChangeStatus(InstructionDto dto, InstructionStatus status) {
            dto.Status = status;
            var success = base.Update(dto);
            /*if (success) {
                var log = new InstructionLogDto {
                    InstructionId = dto.Id,
                    EquipmentId = dto.EquipmentId
                };
                switch (status) {
                    case InstructionStatus.Send:
                        //发送完成后记录指令日志
                        log.Type = InstructionLogType.Send;
                        _instructionLogAppService.Insert(ref log);
                        break;
                    case InstructionStatus.Processing:
                        log.Type = InstructionLogType.Receive;
                        _instructionLogAppService.Insert(ref log);
                        break;
                    default:
                        break;
                }
            }*/
            return success;
        }

        public List<InstructionDto> GetAllReadyList() {
            return Mapper.Map<List<InstructionDto>>(_repository.GetAllList(it => it.Status == InstructionStatus.Waiting));
        }
    }
}
