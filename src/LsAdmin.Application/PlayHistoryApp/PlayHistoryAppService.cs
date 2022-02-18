using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.PlayHistoryApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System;
using System.Collections.Generic;
using AutoMapper;

namespace LsAdmin.Application.PlayHistoryApp {
    public class PlayHistoryAppService : BaseAppService<PlayHistory, PlayHistoryDto>, IPlayHistoryAppService {
        private readonly IPlayHistoryRepository _repository;
        private readonly IOrderTimeRepository _orderTimeRepository;
        

        public PlayHistoryAppService(IPlayHistoryRepository repository, IOrderTimeRepository orderTimeRepository,IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderTimeRepository = orderTimeRepository;
        }

        public int GetCountByOrderTimeId(Guid orderTimeId) {
           return  _repository.GetEntities().Where(item => item.OrderTimeId == orderTimeId).Count();
        }

        public int GetCountByOrderTimeIds(Guid[] orderTimeIds) {
            return _repository.GetEntities().Where(item => orderTimeIds.Contains(item.OrderTimeId)).Count();
        }

        public List<PlayHistoryDto> GetPlayHisByOrderidOrOrdertimeid(Guid orderId)
        {
            List<Guid> allid = new List<Guid>();
            allid.Add(orderId);
            allid.AddRange(_orderTimeRepository.GetEntities().Where(w => w.OrderId == orderId).Select(s => s.Id).ToList());

            return Mapper.Map<List<PlayHistoryDto>>( _repository.GetEntities().Where(item => allid.Contains(item.OrderTimeId)).ToList());

        }
    }
}
