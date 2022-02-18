using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class FilesRepository : LsAdminRepositoryBase<Files>, IFilesRepository
    {
        public FilesRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
