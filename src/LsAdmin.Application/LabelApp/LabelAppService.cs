using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.LabelApp.Dtos;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace LsAdmin.Application.LabelApp.Dtos
{
    public class LabelAppService : BaseAppService<Label, LabelDto>, ILabelAppService
    {
        private readonly ILabelRepository _repository;
       
        public LabelAppService(ILabelRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        public List<LabelDto> GetAdsTag()
        {
            var adstag=_repository.GetAllList(it => it.Type == "广告标签").OrderBy(it=>it.Name);
            return Mapper.Map<List<LabelDto>>(adstag);
        }
        public List<LabelDto> GetPlaceTag()
        {
            var placetag = _repository.GetAllList(it => it.Type == "场所标签").OrderBy(it => it.Name);
            return Mapper.Map<List<LabelDto>>(placetag);
        }
    }
}
