using LsAdmin.Domain.Entities;
using System;
using System.Linq;

namespace LsAdmin.Domain.IRepositories
{
    public interface IEquipmentRepository : IRepository<Equipment>
    {
        /// <summary>
        /// 获取设备播放节目列表
        /// </summary>
        /// <param name="equipmentId">设备编码</param>
        /// <returns>素材列表</returns>
        IQueryable<Program> GetDowmloadPlayInfoByEquipmentId(Guid equipmentId);

        /// <summary>
        ///  获取节目状态为播放中的设备节目清单
        /// </summary>
        /// <param name="equipmentId">设备编码</param>
        /// <returns></returns>
        IQueryable<PlayerProgram> GetDowmloadPlayerProgramByEquipmentId(Guid equipmentId);

        bool AddLogFile(Guid id, Guid fileId);
        IQueryable<Files> GetLogFiles(Guid id);
    }
}
