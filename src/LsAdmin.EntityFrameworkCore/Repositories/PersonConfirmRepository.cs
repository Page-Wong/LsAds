using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class PersonConfirmRepository:LsAdminRepositoryBase<PersonConfirm>, IPersonConfirmRepository
    {
        public PersonConfirmRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
