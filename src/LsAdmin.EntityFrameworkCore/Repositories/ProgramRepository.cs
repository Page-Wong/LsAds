using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class ProgramRepository : LsAdminRepositoryBase<Program>, IProgramRepository {
        public ProgramRepository(LsAdminDbContext dbContext) : base(dbContext)
        {

        }

        public override Program Get(Guid id) {
            return GetEntities().Include(it => it.ProgramMaterials).FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public Program GetWithMaterials(Guid id) {
            var item = Get(id);
            if (item != null) {
                item.ProgramMaterials = _dbContext.Set<ProgramMaterial>().Include(it => it.Material).Where(it => it.ProgramId == id).ToList();
            }
            return item;
        }

        public bool UpdateMaterials(Guid id, List<Material> resources) {
            var item = GetWithMaterials(id);
            if (item == null) {
                return false;
            }
            using (var transaction = _dbContext.Database.BeginTransaction()) {
                try {
                    _dbContext.Set<ProgramMaterial>().Where(it => it.ProgramId == id).ToList().ForEach(it => _dbContext.Set<ProgramMaterial>().Remove(it));
                    Save();
                    foreach (var resource in resources.Distinct()) {
                        _dbContext.Set<ProgramMaterial>().Add(new ProgramMaterial { ProgramId = item.Id, MaterialId = resource.Id });
                        Save();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception) {
                    transaction.Rollback();
                }
            }
            return false;
        }
    }
}
