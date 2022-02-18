using System;
using System.Collections.Generic;
using System.Linq;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using AutoMapper;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Application.MaterialApp.Dtos;

namespace LsAdmin.Application.MaterialApp {
    public class MaterialAppService : BaseAppService<Material, MaterialDto>, IMaterialAppService {

        private readonly IMaterialRepository _repository;
        public MaterialAppService(IMaterialRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public bool Insert(MaterialDto dto) {
            return base.Insert(ref dto);
        }


        public override void  Delete(Guid id){
            Utility.FTP.FtpHelper.DeleteFtpFile(id.ToString() + "."+ Get(id).FilenameExtension);
            base.Delete(id);
        }
        public override void DeleteBatch(List<Guid> ids)
        {
            foreach(var id in ids)
            {
                Utility.FTP.FtpHelper.DeleteFtpFile(id.ToString() + "." + Get(id).FilenameExtension);
            }
            base.DeleteBatch(ids);
        }


        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <returns></returns>
        public List<MaterialInfoDto> GetAllInfoList() {         
            return _repository.GetEntities().Where(m => m.OwnerUserId.Equals(CurrentUser.Id)).Select(m => new MaterialInfoDto { Id = m.Id, Name = m.Name, Duration = m.Duration, Remarks = m.Remarks, OwnerUserId = m.OwnerUserId, FilenameExtension = m.FilenameExtension,MD5=m.MD5,Thumbnail=m.Thumbnail }).ToList();      
        }

        /// <summary>
        /// 获取信息分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public List<MaterialInfoDto> GetAllInfoPageList(int startPage, int pageSize, out int rowCount) {
            return _repository.LoadPageList(startPage, pageSize, out rowCount, m => m.OwnerUserId.Equals(CurrentUser.Id), _orderby).Select(m => new MaterialInfoDto { Id = m.Id,Name=m.Name, Duration = m.Duration, Remarks=m.Remarks, OwnerUserId=m.OwnerUserId, FilenameExtension=m.FilenameExtension, Thumbnail = m.Thumbnail }).ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="startPage">页码</param>
        /// <param name="pageSize">单页数据数</param>
        /// <param name="rowCount">行数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public List<MaterialDto> GetPageListByType(int startPage, int pageSize, out int rowCount, ushort materialType)
        {
            
            var result = from p in  GetAllList().Where(w=>w.MaterialType==materialType & w.OwnerUserId == CurrentUser.Id)
                         select p;
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据Id获取信息实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public MaterialInfoDto GetInfo(Guid id) {
            return _repository.GetEntities().Select(m => new MaterialInfoDto { Id = m.Id, Name = m.Name, Duration = m.Duration, Remarks = m.Remarks, OwnerUserId = m.OwnerUserId,FilenameExtension=m.FilenameExtension, MD5 = m.MD5, Thumbnail = m.Thumbnail }).FirstOrDefault(m => m.Id == id);
        }

        public List<MaterialDto> GetOwnerPageList(int startPage, int pageSize, out int rowCount)
        {
            var result = from p in GetAllList().Where(w => w.OwnerUserId == CurrentUser.Id)
                         select p;
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
