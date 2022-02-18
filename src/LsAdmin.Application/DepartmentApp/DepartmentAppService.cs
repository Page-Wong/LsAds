using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LsAdmin.Application.DepartmentApp.Dtos;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using AutoMapper;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;

namespace LsAdmin.Application.DepartmentApp
{
    public class DepartmentAppService : BaseAppService<Department, DepartmentDto>, IDepartmentAppService
    {
        private readonly IDepartmentRepository _repository;
        public DepartmentAppService(IDepartmentRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }        

        /// <summary>
        /// 根据父级Id获取子级列表
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public List<DepartmentDto> GetChildrenByParent(Guid parentId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<DepartmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.ParentId == parentId, it => it.Code));
        }        

        public DepartmentDto FindByCode(string code) {
            return Mapper.Map<DepartmentDto>(_repository.FindByCode(code));
        }

        public DepartmentDto GetDefualt() {
            return FindByCode("defualt");
        }
    }
}
