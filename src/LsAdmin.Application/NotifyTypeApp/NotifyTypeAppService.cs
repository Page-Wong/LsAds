using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.NotifyTypeApp.Dtos;
using Microsoft.AspNetCore.Http;
using LsAdmin.Domain.IRepositories;
using System;
using System.Collections.Generic;
using AutoMapper;

namespace LsAdmin.Application.NotifyTypeApp {
    public class NotifyTypeAppService : BaseAppService<NotifyType, NotifyTypeDto>, INotifyTypeAppService {
        private readonly INotifyTypeRepository _repository;
        public NotifyTypeAppService(
            INotifyTypeRepository repository, 
            IHttpContextAccessor httpContextAccessor
            ) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public NotifyTypeDto GetByName(string name) {
            return Mapper.Map<NotifyTypeDto>(_repository.FirstOrDefault(item => item.Name == name));
        }
    }
}
