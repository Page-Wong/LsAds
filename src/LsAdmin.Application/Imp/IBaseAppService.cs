using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LsAdmin.Application.Imp
{
    public interface IBaseAppService<TDto> {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<TDto> GetAllList();

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<TDto> GetAllPageList(int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertOrUpdate(ref TDto dto);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool Insert(ref TDto dto);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool Update(TDto dto);

        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        void DeleteBatch(List<Guid> ids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">Id</param>
        void Delete(Guid id);

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        TDto Get(Guid id);

        /// <summary>
        /// 根据Id集合获取实体集合
        /// </summary>
        /// <param name="ids">Id集合</param>
        /// <returns></returns>
        List<TDto> GetByIds(List<Guid> ids);
    }
}
