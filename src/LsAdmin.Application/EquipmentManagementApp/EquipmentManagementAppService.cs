using LsAdmin.Application.EquipmentManagementApp.Dtos;
using LsAdmin.Application.Imp;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.EquipmentManagementApp
{
    public class EquipmentManagementAppService : BaseAppService<EquipmentManagement, EquipmentManagementDto>, IEquipmentManagementAppService
    {

        private readonly IEquipmentManagementRepository _repository;
        public EquipmentManagementAppService(IEquipmentManagementRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }
    }
}