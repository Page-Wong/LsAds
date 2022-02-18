using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class EquipmentRepository : LsAdminRepositoryBase<Equipment>, IEquipmentRepository {
        public EquipmentRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }

        public IQueryable<Program> GetDowmloadPlayInfoByEquipmentId(Guid equipmentId)
        {        
            var programs = from pl in _dbContext.Set<Player>()
                           join plpr in _dbContext.Set<PlayerProgram>()
                           on pl.Id equals plpr.PlayerId
                           join pr in _dbContext.Set<Program>().Include(i => i.ProgramMaterials)
                           on plpr.ProgramId equals pr.Id
                           where pl.EquipmentId == equipmentId
                           // 状态 为 暂停，播放中，准备的记录。
                           && (new PlayerProgramStatus[] { PlayerProgramStatus.Pause, PlayerProgramStatus.Playing, PlayerProgramStatus.Ready }).Contains(plpr.Status)
                           select pr;
            return programs;
         }

        public IQueryable<PlayerProgram> GetDowmloadPlayerProgramByEquipmentId(Guid equipmentId)
        {
            var playerPrograms = from pl in _dbContext.Set<Player>()
                                 join plpr in _dbContext.Set<PlayerProgram>().Include(i => i.Program) 
                                 on pl.Id equals plpr.PlayerId
                                 where pl.EquipmentId == equipmentId
                                 // 状态 播放中 记录。
                                  && plpr.Status== PlayerProgramStatus.Playing
                                 select plpr;
            return playerPrograms;
        }

        public bool AddLogFile(Guid id, Guid fileId) {
            _dbContext.Set<EquipmentLogFile>().Add(new EquipmentLogFile { EquipmentId = id, LogFileId = fileId });
            Save();
            return true;
        }

        public IQueryable<Files> GetLogFiles(Guid id) {
            return GetEntities().Where(it => it.Id == id).Include(it => it.EquipmentLogfiles).FirstOrDefault().EquipmentLogfiles.AsQueryable().Include(it => it.LogFile).Select(it => it.LogFile).AsQueryable();
        }

        /* public override IQueryable<Equipment> GetEntities()
         {
             return _dbContext.Set<Equipment>().Include(item => item.EquipmentModel).Include(i => i.Place);
         }*/
    }
}
