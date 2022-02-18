using LsAdmin.Application.Imp;
using LsAdmin.Application.PlaceTypeApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.PlaceTypeApp
{
    public class PlaceTypeAppService : BaseAppService<PlaceType, PlaceTypeDto>, IPlaceTypeAppService
    {
        private readonly IPlaceTypeRepository _repository;
        public PlaceTypeAppService(IPlaceTypeRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
        }
    }
}
