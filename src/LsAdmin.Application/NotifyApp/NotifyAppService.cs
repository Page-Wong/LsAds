using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.NotifyApp.Dtos;
using Microsoft.AspNetCore.Http;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using AutoMapper;
using LsAdmin.Application.NotifyTypeApp.Dtos;
using Microsoft.EntityFrameworkCore;
using LsAdmin.Application.NotifyTypeApp;

namespace LsAdmin.Application.NotifyApp {
    public class NotifyAppService : BaseAppService<Notify, NotifyDto>, INotifyAppService {
        private readonly INotifyRepository _repository;
        private readonly INotifyTypeAppService _notifyTypeService;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        public NotifyAppService(
            INotifyRepository repository,
            INotifyTypeAppService notifyTypeService,
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor) {
            _repository = repository;
            _notifyTypeService = notifyTypeService;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
        }

        #region 网页平台消息
        public List<NotifyDto> GetCurrentUserWebNotifyAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<NotifyDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, item => item.ReceiverId == CurrentUser.Id, _orderby, _orderbyDesc).Include(item => item.Type));
        }

        public int GetCurrentUserWebNotifyUnreadCount() {
            return _repository.GetAllList(item => item.ReceiverId == CurrentUser.Id && item.Status == NotifyDto.STATUS_SENT).Count;
        }

        public List<NotifyDto> GetCurrentUserWebNotifyUnreadPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<NotifyDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, item => item.ReceiverId == CurrentUser.Id && item.Status == NotifyDto.STATUS_SENT, _orderby, _orderbyDesc).Include(item => item.Type));
        }

        public bool SendWebNotifyTodDepartment(Guid departmentId, string typeName, string[] messageArg) {
            var department = _departmentRepository.Get(departmentId);
            if (department == null) {
                return false;
            }
            var type = _notifyTypeService.GetByName(typeName);
            if (type == null) {
                return false;
            }
            foreach (var user in department.Users) {
                var notify = new NotifyDto {
                    Agent = NotifyDto.AGENT_WEB,
                    Message = String.Format(type.WebMessageTemplate, messageArg),
                    Status = NotifyDto.STATUS_SENT,
                    TypeId = type.Id,
                    MessageType = NotifyDto.MESSAGETYPE_TEXT,
                    SenderId = Guid.Empty,
                    SendTime = DateTime.Now,
                    ReceiverId = user.Id
                };
                Insert(ref notify);
            }
            return true;
        }

        public bool SendWebNotifyToUser(Guid userId, string typeName, string[] messageArg) {
            var user = _userRepository.Get(userId);
            if (user == null) {
                return false;
            }
            var type = _notifyTypeService.GetByName(typeName);
            if (type == null) {
                return false;
            }
            var notify = new NotifyDto {
                Agent = NotifyDto.AGENT_WEB,
                Message = String.Format(type.WebMessageTemplate, messageArg),
                Status = NotifyDto.STATUS_SENT,
                TypeId = type.Id,
                Type = type,
                MessageType = NotifyDto.MESSAGETYPE_TEXT,
                SenderId = Guid.Empty,
                SendTime = DateTime.Now,
                ReceiverId = userId
            };
            return Insert(ref notify);
        }

        public bool CurrentUserReadWebNotify(NotifyDto notify) {
            if (notify == null) {
                return false;
            }
            if (notify.ReceiverId != CurrentUser.Id) {
                return false;
            }
            if (notify.Status != NotifyDto.STATUS_SENT) {
                return false;
            }

            notify.Status = NotifyDto.STATUS_READ;
            notify.ReceiveTime = DateTime.Now;
            return Update(notify);
        }
        #endregion

        #region 统一管理
        public override List<NotifyDto> GetAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<NotifyDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, null, _orderby, _orderbyDesc).Include(item => item.Type));
        }
        #endregion
    }
}
