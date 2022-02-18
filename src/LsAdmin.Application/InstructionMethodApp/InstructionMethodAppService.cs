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

public class InstructionMethodAppService : BaseAppService<InstructionMethod, InstructionMethodDto>, IInstructionMethodAppService {

    private readonly IInstructionMethodRepository _repository;
    public InstructionMethodAppService(IInstructionMethodRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
        _repository = repository;
    }

    public InstructionMethodDto GetByName(string name) {
        return Mapper.Map<InstructionMethodDto>(_repository.GetByName(name));
    }
}

