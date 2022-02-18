using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Domain.Entities
{
    public class User : BaseEntity {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 支付密码
        /// </summary>
        public string PaymentPassword { get; set; }
        
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginTimes { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public int IsDeleted { get; set; }

        /// <summary>
        /// 是否已实名认证
        /// </summary>
        public int AuthStatus { get; set; }


        /// <summary>
        /// 用户头像
        /// </summary>
        public byte[] Avatar { get; set; }

        /// <summary>
        /// 微信唯一ID
        /// </summary>
        public string WxUnionId { get; set; }

        /// <summary>
        /// 所属部门实体
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// 角色集合
        /// </summary>

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
