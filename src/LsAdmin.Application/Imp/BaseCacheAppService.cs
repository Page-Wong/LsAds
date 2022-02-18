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
using System.Collections.Concurrent;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Application.Imp {
    public abstract class BaseCacheAppService<TKey, TDto> : IBaseCasheAppService<TKey, TDto> {

        protected List<TDto> Data;

        public BaseCacheAppService() {
            Data = new List<TDto>();
        }

        private void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest) {
            foreach (var mItem in typeof(T).GetProperties()) {
                var destValue = mItem.GetValue(pTargetObjDest, new object[] { });
                var scrValue = mItem.GetValue(pTargetObjSrc, new object[] { });

                if (destValue == null || scrValue == null || (!destValue.Equals(scrValue) && mItem.Name != "CreateTime" && mItem.Name != "CreateUserId")) {
                    mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
                }
            }
        }

        private TKey GetKeyValue(TDto item) {
            TKey key = default(TKey);
            Type t = item.GetType();

            foreach (var element in t.GetProperties()) {
                var t1 = element.PropertyType;
                var t2 = typeof(TKey);

                if (element.IsDefined(typeof(KeyAttribute), true) && element.PropertyType.Equals(typeof(TKey))) {
                    key = (TKey)element.GetValue(item);
                    break;
                }
            }
            return key;
            /*Array.ForEach(t.GetProperties(), element => {
                var t1 = element.PropertyType;
                var t2 = typeof(TKey);
                
                if (element.IsDefined(typeof(KeyAttribute), true) && element.PropertyType.Equals(typeof(TKey))) {
                    key = (TKey)element.GetValue(item);
                    return;
                }
               
            });
            return key;*/
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public virtual List<TDto> GetAllList() {
            return Data;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public virtual List<TDto> GetAllPageList(int startPage, int pageSize, out int rowCount) {
            var list = Data.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
            rowCount = Data.Count;
            return list;
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public virtual bool InsertOrUpdate(TDto dto) {            
            if (Get(GetKeyValue(dto)) != null)
                return Update(dto);
            return Insert(dto);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public virtual bool Insert(TDto dto) {
            try {
                Data.Add(dto);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public virtual bool Update(TDto dto) {
            try {
                var obj = Get(GetKeyValue(dto));
                EntityToEntity(dto, obj);
                return true;

                /*var index = Data.IndexOf(Get(GetKeyValue(dto)));
                if (index < 0) {
                    return false;
                }
                Data.RemoveAt(index);
                Data.Add(dto);
                return true;*/
                //return Insert(dto);

            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        public virtual void DeleteBatch(List<TKey> ids) {
            ids.ForEach(id => {
                Delete(id);
            });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">Id</param>
        public virtual void Delete(TKey id) {
            Data.Remove(Get(id));
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public virtual TDto Get(TKey id) {
            return Data.Find(d => GetKeyValue(d).Equals(id));
        }

        /// <summary>
        /// 根据Id集合获取实体集合
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        public virtual List<TDto> GetByIds(List<TKey> ids) {
            return Data.Where(d => ids.Contains(GetKeyValue(d))).ToList();
        }
    }
}
