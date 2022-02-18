using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LsAdmin.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// 用户管理仓储实现
    /// </summary>
    public class UserRepository : LsAdminRepositoryBase<User>, IUserRepository
    {
        public UserRepository(LsAdminDbContext dbcontext) : base(dbcontext)
        {

        }

        /// <summary>
        /// 检查用户是存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>存在返回用户实体，否则返回NULL</returns>
        public User CheckUser(string userName, string password)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(it => it.UserName == userName && it.Password == password);
            if (user == null) {
                user = _dbContext.Set<User>().FirstOrDefault(it => it.MobileNumber == userName && it.Password == password);
            }
            return user;
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public User GetWithRoles(Guid id)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(it => it.Id == id);
            if (user != null)
            {
                user.UserRoles = _dbContext.Set<UserRole>().Include(it => it.Role).Where(it => it.UserId == id).ToList();
            }
            return user;
        }

        /// <summary>
        /// 以用户名为条件查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User FindByUsername(string userName) {
            var user = _dbContext.Set<User>().FirstOrDefault(it => it.UserName == userName);
            return user;
        }

        public bool UpdateRoles(Guid id, List<Role> roles) {
            var user = GetWithRoles(id);
            if (user == null) {
                return false;
            }
            using (var transaction = _dbContext.Database.BeginTransaction()) {
                try {
                    _dbContext.Set<UserRole>().Where(item => item.UserId == id).ToList().ForEach(it => _dbContext.Set<UserRole>().Remove(it));
                    Save();
                    foreach (var role in roles.Distinct()) {
                        _dbContext.Set<UserRole>().Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                        Save();
                    }
                    /*user.UserRoles.Clear();
                    Update(user);
                    foreach (var role in roles.Distinct()) {
                        user.UserRoles.Add(new UserRole {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                    }
                    Update(user);*/
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
