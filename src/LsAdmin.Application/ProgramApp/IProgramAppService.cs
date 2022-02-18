using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LsAdmin.Application.FilesApp.Dtos;

namespace LsAdmin.Application.ProgramApp {
    public interface IProgramAppService : IBaseAppService<ProgramDto> {

        /// <summary>
        /// 为节目增加素材关联
        /// </summary>
        /// <param name="id">节目Id</param>
        /// <param name="playerProgramId">素材Id</param>
        /// <returns>操作是否成功</returns>
        bool AddMaterial(Guid id, Guid materialId);

        /// <summary>
        /// 删除节目的素材关联
        /// </summary>
        /// <param name="id">节目Id</param>
        /// <param name="playerProgramId">素材Id</param>
        /// <returns>操作是否成功</returns>
        bool RemoveMaterial(Guid id, Guid materialId);

        /// <summary>
        /// 查找指定用户的节目列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="startPage">startPage</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="rowCount">rowCount</param>
        /// <returns>节目列表</returns>
        List<ProgramDto> GetByOwnerUser(Guid userId, int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 打包储存节目素材文件
        /// </summary>
        /// <param name="id">节目id</param>
        /// <returns></returns>
        bool PackageMaterialsZipById(Guid id);

        /// <summary>
        /// 对未打包储存节目素进行打包存储节目文件 
        /// </summary>
        void PackageMaterialsZipFromNoPackage();

        /// <summary>
        /// 获取节目资源压缩文件
        /// </summary>
        /// <param name="id">节目id</param>
        /// <returns></returns>
        FilesInfoDto  GetResourcesById(Guid id);

        /// <summary>
        /// 获取节目Launcher
        /// </summary>
        /// <param name="id">j节目id</param>
        /// <returns></returns>
        string GetLauncher(Guid id);
        
    }
}
