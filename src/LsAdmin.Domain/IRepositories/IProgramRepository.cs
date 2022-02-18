using LsAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Domain.IRepositories
{
    public interface IProgramRepository : IRepository<Program>
    {

        /// <summary>
        /// 带有素材信息的节目
        /// </summary>
        /// <param name="id">节目Id</param>
        /// <returns>节目</returns>
        Program GetWithMaterials(Guid id);
        /// <summary>
        /// 更新节目的素材
        /// </summary>
        /// <param name="id">节目Id</param>
        /// <param name="resources">素材列表</param>
        /// <returns>操作是否成功</returns>
        bool UpdateMaterials(Guid id, List<Material> resources);


    }
}
