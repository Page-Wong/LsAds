using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    public class OrderEquipmentRepository : LsAdminRepositoryBase<OrderEquipment>, IOrderEquipmentRepository {
        public OrderEquipmentRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }
    }
}
