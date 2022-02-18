using AutoMapper;
using LsAdmin.Application.EquipmentApplicationApp.Dtos;
using LsAdmin.Application.Imp;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Application.EquipmentApplicationApp
{
   public class EquipmentApplicationAppService : BaseAppService<EquipmentApplication, EquipmentApplicationDto>, IEquipmentApplicationAppService
    {
        private readonly IEquipmentApplicationRepository _repository;
        public EquipmentApplicationAppService(IEquipmentApplicationRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
        }

        public List<EquipmentApplicationDto> GetEquipmentApplicationByPlace(Guid placeId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<EquipmentApplicationDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.PlaceId == placeId, it => it.CreateTime));
        }

        public List<EquipmentApplicationDto> GetEquipmentApplicationByStatus(uint status, int startPage, int pageSize, out int rowCount, out  Dictionary<ushort,int> statusRowCount)
        {
           // statusRowCount = (Dictionary <ushort,int>) _repository.GetEntities().GroupBy(g => g.Status).Select(s => new { s.Key, Counts = s.Count() });
            statusRowCount = new Dictionary<ushort, int>();
            foreach (var statusCount in _repository.GetEntities().GroupBy(g => g.Status).Select(s => new { s.Key, Counts = s.Count() })){
                statusRowCount.Add(statusCount.Key, statusCount.Counts);
            }
            return Mapper.Map<List<EquipmentApplicationDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.Status == status, it => it.CreateTime));
        }

        public List<EquipmentApplicationDto> GetEquipmentApplicationWithStatusRowCount(int startPage, int pageSize, out int rowCount, out Dictionary<ushort, int> statusRowCount)
        {
            statusRowCount = new Dictionary<ushort, int>();
            foreach (var statusCount in _repository.GetEntities().GroupBy(g => g.Status).Select(s => new { s.Key, Counts = s.Count() })){
                statusRowCount.Add(statusCount.Key, statusCount.Counts);
            }
            return Mapper.Map<List<EquipmentApplicationDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount,null, it => it.CreateTime));
        }
    }
}
