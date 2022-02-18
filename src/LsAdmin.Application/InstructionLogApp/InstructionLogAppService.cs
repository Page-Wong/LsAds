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
using LsAdmin.Application.InstructionLogApp.Dto;

namespace LsAdmin.Application.InstructionLogApp {
    public class InstructionLogAppService : BaseAppService<InstructionLog, InstructionLogDto>, IInstructionLogAppService {

        private readonly IInstructionLogRepository _repository;
        public InstructionLogAppService(IInstructionLogRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }


    }

}