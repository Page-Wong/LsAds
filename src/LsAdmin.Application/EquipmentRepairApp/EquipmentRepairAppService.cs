using AutoMapper;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentRepairApp.Dtos;
using LsAdmin.Application.Imp;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using LsAdmin.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Application.EquipmentRepairApp
{
    public class EquipmentRepairAppService : BaseAppService<EquipmentRepair, EquipmentRepairDto>, IEquipmentRepairAppService
    {
        private readonly IEquipmentRepairRepository _repository;
        private readonly IEquipmentRepository _repositoryEquipment;
        private readonly LsAdminDbContext _dbcontext;

        public EquipmentRepairAppService(IEquipmentRepairRepository repository, IEquipmentRepository repositoryEquipment, LsAdminDbContext dbcontext,IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _repositoryEquipment = repositoryEquipment;
            _dbcontext = dbcontext;
        }

        public EquipmentRepairDto GetByEquipmentId(Guid equipmentId) {
            return Mapper.Map<EquipmentRepairDto>( _repository.GetEntities().Where(it => it.EquipmentId == equipmentId).FirstOrDefault());
        }

        public List<EquipmentRepairDto> GetListByEquipmentId(Guid equipmentId) {
            return Mapper.Map<List<EquipmentRepairDto>>(_repository.GetAllList(it => it.EquipmentId == equipmentId));
        }

        public List<EquipmentRepairDto> GetOwnerEquipmentRepairPageList(int startPage, int pageSize,uint status, out int rowCount, out int unConfirmedCount, out int confirmedCount, out int completeCount)
        {   
            var equipmentRepairs = GetAllList();
            var result = equipmentRepairs.Where(w => w.Status == status).OrderByDescending(o => o.WarningDate);
            rowCount = result.Count();

            unConfirmedCount = equipmentRepairs.Where(w => w.Status == 0).Count();
            confirmedCount = equipmentRepairs.Where(w => w.Status == 1).Count();
            completeCount = equipmentRepairs.Where(w => w.Status == 2).Count();

            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }
     
       
       public override bool Update( EquipmentRepairDto dto)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    if(base.Update(dto) == false)  return false;
                    var equipment = _repositoryEquipment.Get(dto.EquipmentId);

                    if (dto.Status == 2 && equipment.Status==2){
                        equipment.Status = 0;
                    }else if(equipment.Status==0)
                        equipment.Status = 2;
                    _repositoryEquipment.Update(equipment);        
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }
        
    }
}


