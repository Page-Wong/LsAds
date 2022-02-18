using System;
using System.Collections.Generic;
using LsAdmin.Domain.IRepositories;
using AutoMapper;
using LsAdmin.Domain;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.UserApp.Dtos;
using Microsoft.Extensions.Caching.Memory;
using LsAdmin.Utility.Convert;
using LsAdmin.Utility.Service;

namespace LsAdmin.Application.Imp {
    public abstract class BaseAppService<TEntity, TDto> : IBaseAppService<TDto> where TEntity : BaseEntity {

        protected Expression<Func<TEntity, object>> _orderby = x => null;
        protected Expression<Func<TEntity, object>> _orderbyDesc = x => x.CreateTime;

        private IRepository<TEntity> r;
        private IHttpContextAccessor _httpContextAccessor;

        public BaseAppService(IRepository<TEntity> repository, IHttpContextAccessor httpContextAccessor) {
            r = repository;
            _httpContextAccessor = httpContextAccessor;

        }

        protected UserDto CurrentUser {
            get {
                return _httpContextAccessor?.HttpContext?.Session.Get("CurrentUser") == null ? null : ByteConvertHelper.Bytes2Object<UserDto>(_httpContextAccessor.HttpContext.Session.Get("CurrentUser"));
            }
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public virtual List<TDto> GetAllList() {
            return Mapper.Map<List<TDto>>(r.GetAllList());
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public virtual List<TDto> GetAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<TDto>>(r.LoadPageList(startPage, pageSize, out rowCount, null, _orderby, _orderbyDesc));
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public virtual bool InsertOrUpdate(ref TDto dto) {
            var entity = Mapper.Map<TEntity>(dto);
            if (Get(entity.Id) != null)
                return Update(dto);
            return Insert(ref dto);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public virtual bool Insert(ref TDto dto) {
            TEntity entity = Mapper.Map<TEntity>(dto);
            entity.CreateTime = DateTime.Now;
            if (CurrentUser != null) {
                entity.CreateUserId = CurrentUser.Id;
            }           
            entity = r.Insert(entity);
            dto = Mapper.Map<TDto>(entity);
            return entity == null ? false : true;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public virtual bool Update(TDto dto) {
            TEntity entity = r.Update(Mapper.Map<TEntity>(dto));
            return entity == null ? false : true;
        }

        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        public virtual void DeleteBatch(List<Guid> ids) {
            r.Delete(it => ids.Contains(it.Id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">Id</param>
        public virtual void Delete(Guid id) {
            r.Delete(id);
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public virtual TDto Get(Guid id) {
            return Mapper.Map<TDto>(r.Get(id));
        }

        /// <summary>
        /// 根据Id集合获取实体集合
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        public virtual List<TDto> GetByIds(List<Guid> ids) {
            return Mapper.Map<List<TDto>>(r.GetAllList(item => ids.Contains(item.Id)));
        }
    }
}
