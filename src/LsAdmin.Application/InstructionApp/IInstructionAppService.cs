using LsAdmin.Application.Imp;
using LsAdmin.Application.InstructionApp.Dto;
using LsAdmin.Domain.Entities;
using System.Collections.Generic;

namespace LsAdmin.Application.InstructionApp {
    public interface IInstructionAppService : IBaseAppService<InstructionDto> {
        bool ChangeStatus(InstructionDto dto, InstructionStatus status);
        List<InstructionDto> GetAllReadyList();
    }

}