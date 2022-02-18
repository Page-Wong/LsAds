using System;
using System.Collections.Generic;
using System.Linq;
using LsAdmin.WebAPI.Models;
using LsAdmin.Application.PlayHistoryApp;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderTimeApp;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Utility.Convert;
using LsAdmin.Application.OrderApp.Dtos;

namespace LsAdmin.WebAPI.BackStageManagement
{
    public class CycleRunJob
    {
        private readonly IPlayHistoryRepository _playHistoryrRpository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderTimeRepository _orderTimeRepository;
        public CycleRunJob(IPlayHistoryRepository playHistoryrRpository, IOrderRepository orderRepository, IOrderTimeRepository orderTimeRepository)
        {
            _playHistoryrRpository = playHistoryrRpository;
            _orderRepository = orderRepository;
            _orderTimeRepository = orderTimeRepository;
        }

        public CycleRunJob() : this(null,null,null)
        {
            //    _playHistoryService = (IPlayHistoryAppService)HttpHelper.ServiceProvider.GetService(typeof(IPlayHistoryAppService));
            //     _OrderService = (IOrderAppService)HttpHelper.ServiceProvider.GetService(typeof(IOrderAppService));
            //     _OrderPlaceService = (IOrderPlaceAppService)HttpHelper.ServiceProvider.GetService(typeof(IOrderPlaceAppService));
        }

        public List<OrderPoolModel> CreateNewOrderPool()
        {
            List<OrderPoolModel> OrderPools = new List<OrderPoolModel>();
            try
            {
                List<OrderPlayEquipmentModel> orderPlays = new List<OrderPlayEquipmentModel>();

                var orders = _orderRepository.GetEntities().Where(w => (new int[] { 3, 4 }).Contains(w.Status)).Select(s => new { s.Type, s.Id,s.StartDate,s.EndDate,s.TotalSeconds }).ToList();

                List<OrderPlayEquipmentModel> orderPlayEquipments = (from o in orders.Where(w => w.Type == 12).ToList()
                                                                            select new OrderPlayEquipmentModel
                                                                            {
                                                                                Id = o.Id,
                                                                                OrderId = o.Id,
                                                                                OrderTimeId = o.Id,
                                                                                StartDate = o.StartDate,
                                                                                EndDate = o.EndDate,
                                                                                TimeRange = "",
                                                                                ExposureRate ="1",
                                                                                ExposureCount =(uint)Math.Floor((o.EndDate-o.StartDate).TotalDays*24*3600/o.TotalSeconds),
                                                                            }).ToList();

                orderPlays.AddRange(orderPlayEquipments);

                //ordertime 结构播放任务
                if (orders.Count()> orderPlayEquipments.Count()) {
                    List<Guid> orderTimeTypeOrderIds = orders.Where(w => w.Type != 12).Select(s => s.Id).ToList();
                    List<OrderPlayEquipmentModel> orderTimePlayEquipments = (from ot in _orderTimeRepository.GetEntities().Where(w => orderTimeTypeOrderIds.Contains(w.OrderId))
                                                                             select new OrderPlayEquipmentModel
                                                                             {
                                                                                 Id = ot.Id,
                                                                                 OrderId = ot.OrderId,
                                                                                 OrderTimeId = ot.Id,
                                                                                 StartDate = ot.StartDate,
                                                                                 EndDate = ot.EndDate,
                                                                                 TimeRange = ot.TimeRange,
                                                                                 ExposureRate = ot.ExposureRate,
                                                                                 ExposureCount = ot.ExposureCount,
                                                                             }).ToList();
                    //装载ordertime结构任务
                    orderPlays.AddRange(orderTimePlayEquipments);
                }

                OrderPools =  (from op in orderPlays
                                join h in _playHistoryrRpository.GetEntities().Select(s=> new { OrderTimeId=s.OrderTimeId,} )
                                on op.Id equals h.OrderTimeId into temp
                                from t in temp.DefaultIfEmpty()
                                group op.Id by new { op } into oph
                                select new OrderPoolModel
                                {
                                    CreatetTime = DateTime.Now,
                                    OrderTimeId = oph.Key.op.OrderTimeId.ToString(),
                                    ExposureCount = oph.Key.op.ExposureCount,
                                    AlreadyExposureCount = oph == null ? 0 : oph.Count(),
                                }).ToList(); 
                
                return OrderPools;
            }
            catch (Exception)
            {
                return new List<OrderPoolModel>();
            }
        }

        public void UpdateOrderPool()
        {
            CacheHelper.SetChacheValue(CacheKeys.OrderPool, CreateNewOrderPool());
            GC.Collect();//强制回收
        }

    }
}
