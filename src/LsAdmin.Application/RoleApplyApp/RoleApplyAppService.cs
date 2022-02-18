using AutoMapper;
using LsAdmin.Application.Imp;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.RoleApplyApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.Application.RoleApplyApp {
    public class RoleApplyAppService : BaseAppService<RoleApply, RoleApplyDto>, IRoleApplyAppService {
        private readonly IRoleApplyRepository _repository;
        public RoleApplyAppService(IRoleApplyRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public override List<RoleApplyDto> GetAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<RoleApplyDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, null, _orderby, _orderbyDesc).Include(item => item.ApplyUser).Include(item => item.Role));
        }

        public List<RoleApplyDto> GetListByApplyUserId(Guid userId) {
            return Mapper.Map<List<RoleApplyDto>>(_repository.GetAllList(item => item.ApplyUserId == userId));
        }

        public bool PassApply(Guid id) {
            var dto = Get(id);
            if (dto.Status == RoleApplyDto.STATUS_AUDITTING) {
                dto.Status = RoleApplyDto.STATUS_PASS;
                return Update(dto);
            }
            return false;
        }

        public bool UpPassApply(Guid id) {
            var dto = Get(id);
            if (dto.Status == RoleApplyDto.STATUS_AUDITTING) {
                dto.Status = RoleApplyDto.STATUS_UNPASS;
                return Update(dto);
            }
            return false;
        }
    }
}
