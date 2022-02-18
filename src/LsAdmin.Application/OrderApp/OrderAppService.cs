using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using LsAdmin.Application.OrderTimeApp.Dtos;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using System.Transactions;
using System;
using LsAdmin.Application.OrderTimeApp;
using LsAdmin.Application.OrderEquipmentApp;
using LsAdmin.Application.OrderMaterialApp;
using LsAdmin.EntityFrameworkCore;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.MenuApp.Dtos;
using LsAdmin.Application.TradeApp;
using LsAdmin.Application.PlaceMaterialApp.Dtos;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.TradeApp.Dtos;
using Microsoft.EntityFrameworkCore;
using LsAdmin.Application.TradeBusinessTypeApp;
using System.Threading;
using LsAdmin.Application.PlayerApp;
using LsAdmin.Application.PlayerProgramApp;
using LsAdmin.Application.MaterialApp.Dtos;
using LsAdmin.Application.ProgramApp.Dtos;
using LsAdmin.Application.PlayerApp.Dtos;
using LsAdmin.Application.PlayerProgramApp.Dtos;

namespace LsAdmin.Application.OrderApp {
    public class OrderAppService : BaseAppService<Order, OrderDto>, IOrderAppService {

        private readonly IOrderRepository _repository;
        private readonly IOrderEquipmentRepository _orderEquipmentRepository;
        private readonly IOrderTimeRepository _orderTimeRepository;
        private readonly IOrderMaterialRepository _orderMaterialRepository;
        private readonly IMaterialRepository _materialRepository;

        private readonly IOrderEquipmentAppService _orderEquipmentService;
        private readonly IOrderMaterialAppService _orderMaterialService;
        private readonly IOrderTimeAppService _orderTimeService;
        private readonly IPlaceAppService _placeService;
        private readonly IEquipmentAppService _equipmentService;
        private readonly ITradeAppService _tradeService;
        private readonly ITradeBusinessTypeAppService _tradeBusinessTypeService;
        private readonly IPlayerAppService _playerService;
        private readonly IPlayerProgramAppService _playerProgramService;


        private readonly LsAdminDbContext _dbcontext;
        public OrderAppService(IOrderRepository repository, IOrderEquipmentRepository orderEquipmentRepository, IOrderTimeRepository orderTimeRepository, IOrderMaterialRepository orderMaterialRepository, IMaterialRepository materialRepository,
                              IOrderEquipmentAppService orderEquipmentService, IOrderMaterialAppService orderMaterialService, IOrderTimeAppService orderTimeService,IEquipmentAppService equipmentAppService, LsAdminDbContext dbcontext,
                              IPlaceAppService placeService,ITradeAppService tradeService, ITradeBusinessTypeAppService tradeBusinessTypeService,
                              IPlayerAppService playerService, IPlayerProgramAppService playerProgramService,
                              IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {

            _repository = repository;
            _orderEquipmentRepository= orderEquipmentRepository;
            _orderTimeRepository= orderTimeRepository;
            _orderMaterialRepository= orderMaterialRepository;
            _materialRepository= materialRepository;

            _orderEquipmentService = orderEquipmentService;
            _orderMaterialService = orderMaterialService;
            _orderTimeService = orderTimeService;
            _equipmentService = equipmentAppService;
            _placeService = placeService;
            _tradeService = tradeService;
            _tradeBusinessTypeService = tradeBusinessTypeService;
            _playerService = playerService;
            _playerProgramService = playerProgramService;
            _dbcontext = dbcontext;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public List<OrderDto> GetCurrentUserAllList() {
            return Mapper.Map<List<OrderDto>>(_repository.GetEntities().Where(o => o.CreateUserId == CurrentUser.Id).ToList());
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public List<OrderDto> GetCurrentUserAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<OrderDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, o =>  o.CreateUserId == CurrentUser.Id &&o.Type==11|| o.Type == 12, _orderby,_orderbyDesc));
        }

        public OrderDto GetByOrderNo(string orderNo) {
            return Mapper.Map<OrderDto>(_repository.GetEntities().FirstOrDefault(item => item.OrderNo == orderNo));
        }

        public override bool InsertOrUpdate(ref OrderDto dto) {
            var entity = Mapper.Map<Order>(dto);
            dto.OrderNo = CreateNewOrderNo();
            if (Get(entity.Id) != null)
            {
                dto.CreateTime = _repository.Get(dto.Id).CreateTime;
                return Update(dto);
            }                       
            return Insert(ref dto);
        }

        public bool SaveOrder(ref OrderDto order,Guid programID, List<OrderAreaModel> orderAreas, List<OrderTimeDto> orderTimes) {
            using (var transaction = _dbcontext.Database.BeginTransaction()) {
                try {
                    InsertOrUpdate(ref order);

                    _orderTimeService.DeleteBatch(_orderTimeService.GetByOrderId(order.Id).Select(item => item.Id).ToList());
                    for (int i = 0; i < orderTimes.Count(); i++) {
                        var item = orderTimes.ElementAt(i);
                        if (item.ExposureCount != 0)
                        {
                            item.OrderId = order.Id;
                            item.Amount = item.UnitPrice * item.ExposureCount * order.TotalSeconds;
                            _orderTimeService.Insert(ref item);
                        }                       
                    }

                    if (RemoveAllPlayerProgram(order.Id) == false)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    _orderEquipmentService.DeleteBatch(_orderEquipmentService.GetByOrderId(order.Id).Select(s => s.Id).ToList());

                    var orderTimesHasId = _orderTimeService.GetByOrderId(order.Id).ToList();
                    for (int i = 0; i < orderTimesHasId.Count(); i++)
                    {
                        var item = orderTimesHasId[i];
                        _orderEquipmentService.DeleteBatch(_orderEquipmentService.GetByOrderId(item.Id).Select(s=>s.Id).ToList());

                        for (int j = 0; j < orderAreas.Count(); j++)
                        {
                            var area = orderAreas.ElementAt(j);
                            List<Guid> EquipmentIds = _equipmentService.GetEquipmentByArea(area.Province, area.City, area.District).Select(s => s.Id).ToList();
                            foreach (var EquipmentId in EquipmentIds)
                            {
                                OrderEquipmentDto orderTimeEquipment = new OrderEquipmentDto
                                {
                                    CreateTime = DateTime.Now,
                                    CreateUserId = CurrentUser.Id,
                                    OrderId = order.Id,
                                    EquipmentId = EquipmentId,
                                };
                                _orderEquipmentService.Insert(ref orderTimeEquipment);

                                var players = _playerService.GetByEquipmentId(EquipmentId);
                                for (int k = 0; k < players.Count(); k++)
                                {
                                    AddOrderPlayer(order.Id, players[k].Id);
                                    PlayerProgramDto playerProgramDto = new PlayerProgramDto
                                    {
                                        CreateTime = DateTime.Now,
                                        CreateUserId = CurrentUser.Id,
                                        ExposureCount = 0,
                                        PlayerId = players[k].Id,
                                        ProgramId = programID,
                                        Status = 0,
                                        DateTimeSetting = order.StartDate.ToString("D") + " 至 " + order.EndDate.ToString("D"),
                                    };
                                    _playerProgramService.InsertOrUpdate(ref playerProgramDto);
                                    _repository.UpdatePlayerProgram(order.Id, playerProgramDto.Id);
                                }
                            }
                        }                        
                    }

                    transaction.Commit();
                }
                catch (Exception ex) {
                    return false;
                }
            }
            return true;
        }

        public bool Save(ref OrderDto order, List<OrderMaterialDto> orderMaterials, List<OrderEquipmentDto> orderEquipments)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    InsertOrUpdate(ref order);

                    _orderMaterialService.DeleteBatch(_orderMaterialService.GetByOrderId(order.Id).Select(item => item.Id).ToList());
                    for (int i = 0; i < orderMaterials.Count(); i++)
                    {
                        var item = orderMaterials.ElementAt(i);
                        item.OrderId = order.Id;
                        _orderMaterialService.Insert(ref item);
                    }

                     _orderEquipmentService.DeleteBatch(_orderEquipmentService.GetByOrderId(order.Id).Select(s => s.Id).ToList());
                    for(int i = 0; i < orderEquipments.Count(); i++)
                    {
                        var item = orderEquipments.ElementAt(i);
                        item.OrderId = order.Id;
                        _orderEquipmentService.Insert(ref item);
                    }
                     /*List<Guid> orderEquipmentIds = orderEquipments.Select(s => s.EquipmentId).ToList();
                     foreach (var orderEquipmentId in orderEquipmentIds)
                     {
                         OrderEquipmentDto orderEquipment = new OrderEquipmentDto
                         {
                              CreateTime = DateTime.Now,
                              CreateUserId = CurrentUser.Id,
                              OrderId = order.Id,
                              EquipmentId = orderEquipmentId,
                          };
                         _orderEquipmentService.Insert(ref orderEquipment);
                      }*/
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveAllEquipments(ref OrderDto order, Guid programID, List<OrderEquipmentDto> orderEquipments)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    InsertOrUpdate(ref order);
                    if (RemoveAllPlayerProgram(order.Id)==false)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    _orderEquipmentService.DeleteBatch(_orderEquipmentService.GetByOrderId(order.Id).Select(s => s.Id).ToList());

                    for (int i = 0; i < orderEquipments.Count(); i++)
                    {
                        var item = orderEquipments.ElementAt(i);
                        item.OrderId = order.Id;
                        _orderEquipmentService.Insert(ref item);

                        var players = _playerService.GetByEquipmentId(orderEquipments[i].EquipmentId);
                        for (int j = 0; j < players.Count(); j++)
                        {
                            AddOrderPlayer(order.Id, players[j].Id);
                            PlayerProgramDto playerProgramDto = new PlayerProgramDto
                            {
                                CreateTime = DateTime.Now,
                                CreateUserId = CurrentUser.Id,
                                ExposureCount = 0,
                                PlayerId = players[j].Id,
                                ProgramId = programID,
                                Status = 0,
                                DateTimeSetting=order.StartDate.ToString("D")+" 至 "+order.EndDate.ToString("D"),
                            };
                            _playerProgramService.InsertOrUpdate(ref playerProgramDto);
                            _repository.UpdatePlayerProgram(order.Id, playerProgramDto.Id);
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }
        private string CreateNewOrderNo() {
            return DateTime.Now.ToFileTimeUtc() + new Random().Next(100, 999).ToString();
        }

        public List<Guid> GetCanPalyOrderIds()
        {
            try
            {
                return _repository.GetEntities().Where(w => (new int[] { 3, 4 }).Contains(w.Status)).Select(s => s.Id).ToList() ?? new List<Guid>();
            }
            catch(Exception ex)
            {
                return new List<Guid>();
            }
        }

        /// <summary>
        /// 获取设备的素材列表
        /// </summary>
        /// <param name="equipmentId">设备号</param>
        /// <returns></returns>
        public Dictionary<string, string> GetSyncPlayFile(Guid equipmentId){
            Dictionary<string, string> SyncPlayFiles = new Dictionary<string, string>();
            try{

                List<Guid> availabilityOrderIds = new List<Guid>();

                List<Guid> allOrderIds= _orderEquipmentRepository.GetEntities().Where(w => w.EquipmentId == equipmentId).GroupBy(G => G.OrderId).Select(s => s.Key).ToList();
         
                if (allOrderIds == null)
                    return SyncPlayFiles;
                
                var orderIds = _repository.GetEntities().Where(w => (new int[] { 3, 4 }).Contains(w.Status) && w.EndDate.Date >= DateTime.Now.Date && allOrderIds.Contains(w.Id))
                    .Select(s =>s.Id).ToList();

                availabilityOrderIds.AddRange(orderIds);
                            
                var ordertimeIds =  from ot in _orderTimeRepository.GetEntities().Where(w => w.EndDate.Date >= DateTime.Now.Date && allOrderIds.Contains(w.Id)) 
                                                join o in _repository.GetAllList(w => (new int[] { 3, 4 }).Contains(w.Status) && w.EndDate.Date >= DateTime.Now.Date) 
                                                on ot.OrderId equals o.Id
                                                select new { o.Id };

                if(ordertimeIds!=null && ordertimeIds.Count()>0) { 
                    availabilityOrderIds.AddRange((List<Guid>)ordertimeIds);
                }

                var orderdevicefiles = (from  om in _orderMaterialRepository.GetEntities().Where(w => availabilityOrderIds.Contains(w.OrderId))
                                        join m in _materialRepository.GetEntities().Select(m => new MaterialDto { Id = m.Id, Name = m.Name, }) on om.MaterialId equals m.Id
                                        select new
                                        {
                                            om.MaterialId,
                                            m.Name,
                                        }).ToList();

                foreach (var orderdevicefile in orderdevicefiles.Distinct())
                {
                    SyncPlayFiles.Add(orderdevicefile.MaterialId.ToString(), orderdevicefile.Name);
                }


                // 不包含ordertime 的播放素材列表 
                /*
                var orderdevicefiles = (from oe in orderEquipments
                                      join o in _repository.GetAllList(w => w.EndDate.Date <= DateTime.Now.Date) on oe.OrderId equals o.Id
                                      join om in _orderMaterialRepository.GetEntities() on o.Id equals om.OrderId
                                      join m in _materialRepository.GetEntities().Select(m => new MaterialDto { Id = m.Id, Name = m.Name, }) on om.MaterialId equals m.Id
                                      select new
                                      {
                                          om.MaterialId,
                                          m.Name,
                                      }).ToList();


                foreach (var orderdevicefile in orderdevicefiles.Distinct()){
                    SyncPlayFiles.Add(orderdevicefile.MaterialId.ToString(), orderdevicefile.Name);
                }

                // 包含ordertime 的播放素材列表
                var orderTimedevicefiles = (from oe in orderEquipments
                                            join ot in _orderTimeRepository.GetEntities().Where(w => w.EndDate.Date <= DateTime.Now.Date) on oe.OrderId equals ot.Id
                                            join o in _repository.GetAllList(w => (new int[] { 3, 4 }).Contains(w.Status)) on ot.OrderId equals o.Id
                                            join om in _orderMaterialRepository.GetEntities() on o.Id equals om.OrderId
                                            join m in _materialRepository.GetEntities().Select(m => new MaterialDto { Id = m.Id, Name = m.Name, }) on  om.MaterialId equals m.Id
                                            select new
                                            {
                                                om.MaterialId,
                                                m.Name,
                                            }).ToList();


            foreach (var devicefile in orderTimedevicefiles.Distinct()){
                SyncPlayFiles.Add(devicefile.MaterialId.ToString(), devicefile.Name);
            }*/
                return SyncPlayFiles;
            }
            catch (Exception ex){
                return SyncPlayFiles;// new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 获取设备可播放OrderTimes列表
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        public List<OrderPlayEquipmentModel> GetEquipmentOrderTimes(Guid equipmentId)
        {
            try
            {
                List<OrderPlayEquipmentModel> OrderPlayEquipments = new List<OrderPlayEquipmentModel>();

                List<Guid> EquipmentsorderTimeIds = _orderEquipmentRepository.GetEntities().Where(w => w.EquipmentId == equipmentId).GroupBy(g => g.OrderId).Select(s => s.Key).ToList();
                if(EquipmentsorderTimeIds!=null && EquipmentsorderTimeIds.Count > 0) {

                    var ordertimeEquipments = (from t in _orderTimeRepository.GetEntities().Where(w => w.StartDate <= DateTime.Now.Date && w.EndDate >= DateTime.Now.Date && EquipmentsorderTimeIds.Contains(w.Id)).Select(s => new { Id = s.Id, OrderId = s.OrderId, StartDate = s.StartDate, EndDate = s.EndDate, TimeRange = s.TimeRange, ExposureRate = s.ExposureRate, ExposureCount = s.ExposureCount })
                                               join o in _repository.GetEntities().Where(w => (new int[] { 3, 4 }).Contains(w.Status) && w.Type != 12).Select(s => new { Id = s.Id, MateralType = s.MateralType, TotalSeconds = s.TotalSeconds, AdsTag = s.AdsTag })
                                                on t.OrderId equals o.Id
                                               select new OrderPlayEquipmentModel
                                               {
                                                   Id = t.Id,
                                                   OrderTimeId = t.Id,
                                                   OrderId = t.OrderId,
                                                   StartDate = t.StartDate,
                                                   EndDate = t.EndDate,
                                                   TimeRange = t.TimeRange,
                                                   ExposureRate = t.ExposureRate,
                                                   ExposureCount = t.ExposureCount,
                                                   AvailableDays = t.EndDate.Subtract(DateTime.Now).Days,    //(uint)(t.EndDate.DayOfYear - DateTime.Now.DayOfYear),
                                          MateralType = o.MateralType,
                                          TotalSeconds = o.TotalSeconds,
                                          AdsTag = o.AdsTag,
                                      }).ToList();

                    if (ordertimeEquipments != null) { OrderPlayEquipments.AddRange(ordertimeEquipments); }
                }
                return OrderPlayEquipments;
            }
            catch (Exception ex)
            {
                return new List<OrderPlayEquipmentModel>();
            }
        }
        

        public List<Guid> GetePlaceMaterialOrde(Guid placeId)
        {
            var Orders = (from ot in _orderTimeRepository.GetEntities().Where(w => w.Area == placeId.ToString())
                         join o in _repository.GetEntities().Where(w => w.Type == 2 && w.Status != 6)
                         on ot.OrderId equals o.Id
                         select new OrderDto
                         {
                          Id=o.Id,
                         }).ToList();

            return Orders.Select(s =>s.Id).ToList();
        }


        /*public bool SavePlaceMaterialOrder(Guid placeId, List<PlaceMaterialDto> PlaceMaterials)
        {
            int playDays = 1000;//可播放时间长度;
            uint exposureCount = 10000;//可播放次数


            //  using (var transaction = _dbcontext.Database.BeginTransaction())  //由于在 PlaceMaterialAppService的 SaveToOrder函数里用了事务所以在这里不能用事务
            {
                try
                {
                    var place = _placeService.Get(placeId);
                    if (place == null)
                        return false;

                    OrderDto order = new OrderDto {
                        AdsTag = place.AdsBlackTag,
                        Amount = 0,
                        Industry = place.TypeName,
                        MateralType = PlaceMaterials[0].MaterialType,
                        Name = "场所自主广告_"+place.Name + "_"+ place.Id,
                        Status = 2,
                        Remarks = "本场所广告",
                        TotalSeconds = exposureCount*(uint)PlaceMaterials.Sum(s => s.Seconds),
                        Type = 2,
                    };
                  
                    InsertOrUpdate(ref order);
                    
                    for (int i = 0; i < PlaceMaterials.Count(); i++)
                    {
                        var PlaceMaterial = PlaceMaterials.ElementAt(i);
                        OrderMaterialDto orderMaterial = new OrderMaterialDto {
                            MaterialId= PlaceMaterial.MaterialId,
                            Orderby= PlaceMaterial.Orderby,
                            Remarks= PlaceMaterial.Remarks,
                            OrderId=order.Id,
                            Seconds= PlaceMaterial.Seconds,
                        };
                        _orderMaterialService.Insert(ref orderMaterial);
                    }

                    OrderTimeDto orderTime = new OrderTimeDto
                    {
                        OrderId = order.Id,
                        Amount = 0,
                        StartDate = DateTime.Now.Date,
                        EndDate = DateTime.Now.AddDays(playDays).Date,
                        TimeRange = place.TimeRange ?? "0:00-23:59",
                        UnitPrice=0,
                        Area= placeId.ToString(),    
                        ExposureCount= exposureCount,
                    };

                    _orderTimeService.Insert(ref orderTime);

                    List<Guid> orderTimeEquipmentIds = _equipmentService.GetAllEquipmentByPlace(place.Id).Select(s => s.Id).ToList();

                    foreach (var orderTimeEquipmentId in orderTimeEquipmentIds)
                    {
                        OrderEquipmentDto orderTimeEquipment = new OrderEquipmentDto
                        {
                            CreateTime = DateTime.Now,
                            CreateUserId = CurrentUser.Id,
                            //OrderTimeId = orderTime.Id,
                            EquipmentId = orderTimeEquipmentId,
                        };
                        _orderEquipmentService.Insert(ref orderTimeEquipment);
                    }

                   // transaction.Commit();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }*/
        public bool ApplyRefund(Guid userId, OrderDto dto, string applyRefundReason) {
            switch (dto.Status) {
                case OrderDto.ORDER_STATUS_AUDITING:
                    dto.Status = OrderDto.ORDER_STATUS_REFUNDBEFOREAUDIT;
                    break;
                case OrderDto.ORDER_STATUS_PREPARING:
                    dto.Status = OrderDto.ORDER_STATUS_REFUNDAFTERAUDIT;
                    break;
                default:
                    return false;
            }
            using (var transaction = _dbcontext.Database.BeginTransaction()) {
                try {
                    var trades = GetTradesByOrderId(dto.Id);
                    trades.ForEach(delegate (TradeDto item)
                    {
                        
                        string typeName = "OrderApplyRefundBy" + Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(item.TradeMethodName);
                        TradeDto refundTrade = new TradeDto {
                            BusinessTypeId = _tradeBusinessTypeService.GetIdByName(typeName),
                            UserId = userId,
                            Amount = item.Amount,
                            Remarks = applyRefundReason,
                            TradeStatus = TradeDto.TRADESTATUS_TRADING
                        };
                        switch (item.TradeMethod) {
                            case TradeDto.TRADEMETHOD_ALIPAY:
                                _tradeService.ApplyRefundByAlipay(ref refundTrade);
                                break;
                            case TradeDto.TRADEMETHOD_POINT:
                                _tradeService.IncreasePointIncome(ref refundTrade);
                                break;
                            default:
                                break;
                        }
                        dto.OrderTrades.Add(new OrderTradeDto {
                            TradeId = refundTrade.Id,
                            OrderId = dto.Id
                        });
                    });
                    if (Update(dto)) {
                        transaction.Commit();
                        return true;                        
                    }
                    transaction.Rollback();
                }
                catch (Exception) {
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool PayByPoint(Guid userId, OrderDto dto, out TradeDto tradeDto) {
            tradeDto = null;
            if (dto.Status != OrderDto.ORDER_STATUS_PUBLISHED) {
                return false;
            }
            using (var transaction = _dbcontext.Database.BeginTransaction()) {
                try {

                    dto.Status = OrderDto.ORDER_STATUS_AUDITING;
                    tradeDto = new TradeDto {
                        BusinessTypeId = _tradeBusinessTypeService.GetIdByName("OrderPayByPoint"),
                        UserId = userId,
                        Amount = dto.Amount,
                        Remarks = "消费积分支付投放方案",
                        TradeStatus = TradeDto.TRADESTATUS_SUCCESS
                };
                    if (_tradeService.ExpensePoint(ref tradeDto)) {
                        dto.OrderTrades.Add(new OrderTradeDto { TradeId = tradeDto.Id, OrderId = dto.Id, Trade = tradeDto, Order = dto });
                        if (Update(dto)) {
                            transaction.Commit();
                            return true;
                        }
                    }
                    transaction.Rollback();
                }
                catch (Exception) {
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool PayByAlipay(Guid userId, OrderDto dto, out TradeDto tradeDto) {
            tradeDto = null;
            if (dto.Status != OrderDto.ORDER_STATUS_PUBLISHED) {
                return false;
            }
            using (var transaction = _dbcontext.Database.BeginTransaction()) {
                try {
                    List<TradeDto> trades = GetTradesByOrderId(dto.Id).Where(item => item.TradeMethod == TradeDto.TRADEMETHOD_ALIPAY && item.TradeStatus == TradeDto.TRADESTATUS_TRADING).ToList();
                    trades.ForEach(delegate (TradeDto item) {
                            item.TradeStatus = TradeDto.TRADESTATUS_CANCEL;
                            _tradeService.Update(item);
                    });
                    tradeDto = new TradeDto {
                        BusinessTypeId = _tradeBusinessTypeService.GetIdByName("OrderPayByAlipay"),
                        UserId = userId,
                        Amount = dto.Amount,
                        Remarks ="支付宝付款投放方案"
                    };
                    if (_tradeService.PayByAlipay(ref tradeDto)) {
                        dto.OrderTrades.Add(new OrderTradeDto { TradeId = tradeDto.Id, OrderId = dto.Id, Trade = tradeDto, Order = dto });
                        if (Update(dto)) {
                            transaction.Commit();
                            return true;
                        }                        
                    }
                    transaction.Rollback();
                }
                catch (Exception) {
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool AfterPay(OrderDto dto, TradeDto tradeDto) {
            using (var transaction = _dbcontext.Database.BeginTransaction()) {
                try {
                    dto.Status = OrderDto.ORDER_STATUS_AUDITING;
                    tradeDto.TradeStatus = TradeDto.TRADESTATUS_SUCCESS;
                    if (Update(dto) && _tradeService.Update(tradeDto)) {
                        transaction.Commit();
                        return true;
                    }
                    transaction.Rollback();
                }
                catch (Exception) {
                    transaction.Rollback();
                    return false;
                }
            }
            return false;
        }

        public List<TradeDto> GetTradesByOrderId(Guid id) {
            return Mapper.Map<List<TradeDto>>(_repository.GetTradesByOrderId(id));
        }

        public List<TradeDto> GetTradesWithBusinessTypeByOrderId(Guid id)
        {
            return Mapper.Map<List<TradeDto>>(_repository.GetTradesWithBusinessTypeByOrderId(id));
        }

        public List<OrderDto> GetAllRefundPageList(int startPage, int pageSize, out int rowCount) {
            var refundStatus = new ushort[] { OrderDto.ORDER_STATUS_REFUNDAFTERAUDIT, OrderDto.ORDER_STATUS_REFUNDBEFOREAUDIT };
            return Mapper.Map<List<OrderDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, item => refundStatus.Contains(item.Status), _orderby, _orderbyDesc));
        }

        /// <summary>
        /// 获取具体某台设备的order播放任务列表
        /// </summary>
        /// <param name="equipmentId">设备号</param>
        /// <returns></returns>
        public List<OrderPlayEquipmentModel> GetEquipmentOrders(Guid equipmentId)
        {
            try
            {
                List<OrderPlayEquipmentModel> OrderPlayEquipments = new List<OrderPlayEquipmentModel>();

                List<Guid> EquipmentorderIds = _orderEquipmentRepository.GetEntities().Where(w => w.EquipmentId == equipmentId).GroupBy(g => g.OrderId).Select(s => s.Key).ToList();

                if (EquipmentorderIds != null && EquipmentorderIds.Count > 0)
                {
                    var orderEquipments = (from o in _repository.GetEntities().Where(w => (new int[] { 3, 4 }).Contains(w.Status) && EquipmentorderIds.Contains(w.Id) && w.Type == 12)
                                           select new OrderPlayEquipmentModel
                                           {
                                               Id = o.Id,
                                               OrderTimeId = o.Id,
                                               OrderId = o.Id,
                                               StartDate = o.StartDate,
                                               EndDate = o.EndDate,
                                               //TimeRange = "",
                                               ExposureRate = "1",
                                               ExposureCount = (uint)Math.Floor((double)(o.EndDate.Subtract(o.StartDate).Days*24*3600/o.TotalSeconds)),                            
                                               AvailableDays = o.EndDate.Subtract(o.StartDate).Days,            

                                     MateralType = o.MateralType,
                                     TotalSeconds = o.TotalSeconds,
                                     AdsTag = o.AdsTag,
                                 }).ToList();

                    if (orderEquipments != null) { OrderPlayEquipments.AddRange(orderEquipments); }              
                }
                return OrderPlayEquipments;
            }
            catch (Exception ex)
            {
                return new List<OrderPlayEquipmentModel>();
            }
        }

        public bool AddPlayer(Guid id, Guid playerId) {
            var item = Get(id);
            if (item == null) {
                return false;
            }
            var resource = _playerService.Get(playerId);
            if (resource == null) {
                return false;
            }
            var resources = item.OrderPlayers.Select(it => it.Player).ToList();
            resources.Add(resource);
            return _repository.UpdatePlayers(id, Mapper.Map<List<Player>>(resources));
        }

        public bool AddOrderPlayer(Guid id,Guid playerId)
        {
            var item = Get(id);
            if (item == null)
            {
                return false;
            }
            return _repository.UpdateOrderPlayer(id, playerId);
        }

        public bool RemovePlayer(Guid id, Guid playerId) {
            var item = Get(id);
            if (item == null) {
                return false;
            }
            var resource = _playerService.Get(playerId);
            if (resource == null) {
                return false;
            } 
            item.OrderPlayers.RemoveAll(it => it.OrderId == item.Id && it.PlayerId == resource.Id);
            return Update(item);
        }

        public bool AddPlayerProgram(Guid id, Guid playerProgramId) {
            var item = Get(id);
            if (item == null) {
                return false;
            }
            var resource = _playerProgramService.Get(playerProgramId);
            if (resource == null) {
                return false;
            }
            var resources = item.OrderPlayerPrograms.Select(it => it.PlayerProgram).ToList();
            resources.Add(resource);
            return _repository.UpdatePlayerPrograms(id, Mapper.Map<List<PlayerProgram>>(resources));
        }

        public bool RemovePlayerProgram(Guid id, Guid playerProgramId) {
            var item = Get(id);
            if (item == null) {
                return false;
            }
            var resource = _playerProgramService.Get(playerProgramId);
            if (resource == null) {
                return false;
            }
            item.OrderPlayerPrograms.RemoveAll(it => it.OrderId == item.Id && it.PlayerProgramId == resource.Id);
            return Update(item);
        }

        public bool RemoveAllPlayerProgram(Guid id)
        {
            var players =GetWithPlayer(id);
            if (players == null)
            {
                return false;
            }
            if (players.OrderPlayers.RemoveAll(it => it.OrderId == players.Id) == 1)
            {
                return false;
            }
            var playerPrograms = GetWithPlayerProgram(id).OrderPlayerPrograms.Select(s => s.PlayerProgramId).ToList();
            _playerProgramService.DeleteBatch(playerPrograms);
            return _repository.RemovePlayerPrograms(id);
        }

        public OrderDto GetWithPlayer(Guid id) {
            return Mapper.Map<OrderDto>(_repository.GetWithPlayers(id));
        }

        public OrderDto GetWithPlayerProgram(Guid id) {
            return Mapper.Map<OrderDto>(_repository.GetWithPlayerPrograms(id));
        }

        public Guid GetOrderProgram(Guid id)
        {
            var item =Get(id);
            var playerPrograms = item.OrderPlayerPrograms.Select(s =>s.PlayerProgram).ToList();
            return playerPrograms[0].ProgramId;
        }

        public List<Guid> GetOrderPlayers(Guid id)
        {
            var item =GetWithPlayer(id);
            var players = item.OrderPlayers.Select(s => s.PlayerId).ToList();
            return players;
        }

        public bool SaveAll(ref OrderDto order, Guid programID, List<Guid> playIds)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    InsertOrUpdate(ref order);
                    if (_repository.RemovePlayerPrograms(order.Id) == false)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    if (_repository.RemovePlayers(order.Id) == false)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    foreach(var playId in playIds)
                    {
                        PlayerProgramDto playerProgramDto = new PlayerProgramDto
                        {
                            CreateTime = DateTime.Now,
                            CreateUserId = CurrentUser.Id,
                            ExposureCount = 0,
                            PlayerId = playId,
                            ProgramId = programID,
                            Status = 0,
                            DateTimeSetting = order.StartDate.ToString("D") + " 至 " + order.EndDate.ToString("D"),
                        };

                        if( _playerProgramService.InsertOrUpdate(ref playerProgramDto) == false)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        //添加OrderPlayerProgram 记录
                        if (_repository.UpdatePlayerProgram(order.Id, playerProgramDto.Id) == false)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        //添加OrderPlayer 记录
                        if (_repository.UpdateOrderPlayer(order.Id, playId)==false)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }
    }
}
