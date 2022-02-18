using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using LsAdmin.Application.MaterialApp;
using System.Collections.Generic;
using Newtonsoft.Json;
using LsAdmin.Application.PlayHistoryApp.Dtos;
using System;
using LsAdmin.Application.PlayHistoryApp;
using LsAdmin.Application.ForwardHistoryApp.Dtos;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Utility.Convert;
using LsAdmin.Application.PlaceApp;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderPlaceApp;
using LsAdmin.Application.OrderMaterialApp;
using LsAdmin.Application.PlaceApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using LsAdmin.Application.OrderTimeApp;
using LsAdmin.Application.ForwardHistoryApp;
using LsAdmin.Application.OrderEquipmentApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.FilesApp;
using LsAdmin.Application.MaterialApp.Dtos;
using LsAdmin.Utility.FTP;
using EquipmentService.WebAPI.Models;

namespace EquipmentService.WebAPI.Controllers {
    [Route("[controller]/[action]")]
    public class PlayController : Controller
    {
        private readonly IMaterialAppService _materialService;
        private readonly IPlayHistoryAppService _playHistoryService;
        private readonly IEquipmentAppService _equipmentService;
        private readonly IPlaceAppService _placeService;
        private readonly IOrderPlaceAppService _orderPlaceService;
        private readonly IOrderMaterialAppService _orderMaterialService;
        private readonly IForwardHistoryAppService _forwardHistoryService;
        private readonly IFilesAppService _filesAppService;
        

        private readonly IOrderAppService _orderService;
        private readonly IOrderTimeAppService _orderTimeService;
        private readonly IOrderEquipmentAppService _orderEquipmentService;  


        private List<PlayListResultModel> playListResults;
        private List<OrderPoolModel> orderPools;

        public PlayController(IMaterialAppService service, IPlayHistoryAppService playHistoryService,
                               IEquipmentAppService EquipmentAppService, IPlaceAppService PlaceAppService,
                                IOrderPlaceAppService OrderPlaceAppService, IForwardHistoryAppService forwardHistoryService,
                               IOrderMaterialAppService OrderMaterialAppService, IMaterialAppService IMaterialAppService,
                                IOrderAppService orderService, IOrderTimeAppService orderTimeService, IOrderEquipmentAppService orderEquipmentService,
                                IFilesAppService filesAppService
                                                       )
        {
            _materialService = service;
            _playHistoryService = playHistoryService;
            _equipmentService = EquipmentAppService;
            _placeService = PlaceAppService;
            _orderPlaceService = OrderPlaceAppService;
            _orderMaterialService = OrderMaterialAppService;
            _materialService = IMaterialAppService;
            _forwardHistoryService = forwardHistoryService;

             _orderService = orderService;
            _orderTimeService = orderTimeService;
            _orderEquipmentService = orderEquipmentService;

            _filesAppService = filesAppService;


        playListResults = (List<PlayListResultModel>)CacheHelper.GetCacheValue(CacheKeys.PlayListResult) ?? new List<PlayListResultModel>();

            if ((List<OrderPoolModel>)CacheHelper.GetCacheValue(CacheKeys.OrderPool) == null)
            {
                new BackStageManagement.CycleRunJob().UpdateOrderPool();
            }
            orderPools = (List<OrderPoolModel>)CacheHelper.GetCacheValue(CacheKeys.OrderPool) ?? new List<OrderPoolModel>();
        }
        /// <summary>
        /// 获取设备播放素材列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public JsonResult SyncPlayFile(string deviceId) {
            List<PlayFileModel> files = new List<PlayFileModel>();
            try{
                EquipmentDto equipment = _equipmentService.GetByDeviceId(deviceId);
                if (equipment == null){
                    return Json(new JsonResultModel{
                        Total = 0,
                        Success = false,
                        DataList = files
                    });
                }

                //获取设备的素材列表
                var syncPlayFiles = _orderService.GetSyncPlayFile(equipment.Id);

                if (syncPlayFiles != null){
                    foreach (var syncPlayFile in syncPlayFiles){
                        files.Add(new PlayFileModel{
                            FileId = syncPlayFile.Key,
                            FileName = syncPlayFile.Value
                        });
                    }
                }

                return Json(new JsonResultModel{
                    Total = files.Count,
                    Success = true,
                    DataList = files
                });
            }
            catch (Exception){
                return Json(new JsonResultModel{
                    Total = 0,
                    Success = false,
                    DataList = files
                });
            }
        }

        /// <summary>
        /// 返回设备播放列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public JsonResult SyncPlayList(string deviceId )
        {
            int ordercount =10;//每次获取的列表数量
            int onceSeconds =1;//前后两次的时间间隔（秒）

            try
            {
                var IsExistplay = playListResults.FirstOrDefault(f => f.DriverId == deviceId);

                if (IsExistplay == null)
                {
                    IsExistplay = new PlayListResultModel
                    {
                        DriverId = deviceId,
                        HasChange = true,
                        Success = false,
                        Data = null,
                        LastRequesTime = DateTime.Now,
                    };
                    playListResults.Add(IsExistplay);
                }
                else
                if (((TimeSpan)(DateTime.Now - IsExistplay.LastRequesTime)).TotalSeconds < onceSeconds)
                {
                    return Json(new PlayListResultModel
                    {
                        HasChange = true,
                        Success = false,
                        Data = "距上次请求小于" + onceSeconds.ToString() + "秒",
                        DriverId = deviceId,
                        LastRequesTime = IsExistplay.LastRequesTime
                    });
                }
                IsExistplay.LastRequesTime = DateTime.Now;

                EquipmentDto equipment = _equipmentService.GetByDeviceId(deviceId);

                if (equipment == null)
                {
                    return Json(new PlayListResultModel
                    {
                        HasChange = true,
                        Success = false,
                        Data = "系统中没登记该台播放设备信息！",
                        DriverId = deviceId,
                        //LastRequesTime = DateTime.Now
                    });
                }
                List<PlayItemModel> playItems = new List<PlayItemModel>();
                List<PlayOrderModel> playOrders = new List<PlayOrderModel>();

                if (equipment.PlaceId == null)
                {
                    return Json(new PlayListResultModel
                    {
                        HasChange = true,
                        Success = false,
                        Data = "该设备没有绑定播放地点！",
                        DriverId = deviceId,
                        //LastRequesTime = DateTime.Now
                    });
                }

                PlaceDto place = _placeService.Get((Guid)equipment.PlaceId);

                if (place == null)
                {
                    return Json(new PlayListResultModel
                    {
                        HasChange = true,
                        Success = false,
                        Data = "该设备没有绑定播放地点！",
                        DriverId = deviceId,
                        //LastRequesTime = DateTime.Now
                    });
                }

                //无设置播放时间段的默认全天播放
                if (!string.IsNullOrEmpty(place.TimeRange))
                {
                    //timeRanges 格式 06:30-07:00
                    List<string> timeRanges = place.TimeRange.Split(',').ToList();
                    // isIntimeRange 设备的播放时间是否在场地播放时间段内  
                    Boolean isIntimeRange = false;

                    //校验设备的播放时间是否在场地播放时间段内，不在播放时间段内不生成播放列表
                    foreach (var timeRange in timeRanges)
                    {
                        List<string> times = new List<string>(timeRange.Split('-'));

                        if (DateTime.Now.TimeOfDay >= DateTime.Parse(times[0]).TimeOfDay && DateTime.Now.TimeOfDay <= DateTime.Parse(times[1]).TimeOfDay)
                        {
                            isIntimeRange = true;
                            break;
                        }
                    }

                    if (isIntimeRange == false)
                    {
                        return Json(new PlayListResultModel
                        {
                            HasChange = true,
                            Success = false,
                            Data = "不在场地播放时段！",
                            DriverId = deviceId,
                            //LastRequesTime = DateTime.Now
                        });
                    }
                }

                //设备播放任务    OrderEquipment
                List<OrderPlayEquipmentModel> orderPlayEquipments = new List<OrderPlayEquipmentModel>();

                // 装载ordertime结构的任务到播放任务
                 orderPlayEquipments.AddRange(_orderService.GetEquipmentOrderTimes(equipment.Id));
                // 装载order结构的任务到播放任务
                orderPlayEquipments.AddRange(_orderService.GetEquipmentOrders(equipment.Id));
                // 装载order结构的任务到播放任务
                
                //排除播放时间段不包含当前时间的记录
                for (int i = orderPlayEquipments.Count - 1; i >= 0; i--)
                {
                    var orderPlayPlace = orderPlayEquipments[i];
                    try
                    {
                        //无设置播放时间段的默认全天播放
                        if (!string.IsNullOrEmpty(orderPlayPlace.TimeRange))
                        {
                            List<string> orderPlacesTimeRanges = orderPlayPlace.TimeRange.Split(',').ToList();

                            Boolean isInorderPlaceTimeRange = false;
                            //排除播放时间段不包含当前时间的记录
                            foreach (var orderPlacesTimeRange in orderPlacesTimeRanges)
                            {
                                List<string> orderPlacesTimes = orderPlacesTimeRange != null ? orderPlacesTimeRange.Split('-').ToList() : new List<string> { "0:00", "23:59" };

                                if (DateTime.Now.TimeOfDay >= DateTime.Parse(orderPlacesTimes[0]).TimeOfDay && DateTime.Now.TimeOfDay <= DateTime.Parse(orderPlacesTimes[1]).TimeOfDay)
                                {
                                    isInorderPlaceTimeRange = true;
                                    break;
                                }
                            }
                            if (isInorderPlaceTimeRange == false)
                                orderPlayEquipments.Remove(orderPlayPlace);
                        }
                    }
                    catch (Exception)
                    {
                        orderPlayEquipments.Remove(orderPlayPlace);
                    }
                }
                List<string> adsWhiteTags = place.AdsWhiteTag != null ? place.AdsWhiteTag.Split(',').ToList() : new List<string>();
                List<string> adsBlackTags = place.AdsBlackTag != null ? place.AdsBlackTag.Split(',').ToList() : new List<string>();

                //排除非白名单记录&&排除黑名单记录
                for (int i = orderPlayEquipments.Count - 1; i >= 0; i--)
                {
                    var orderPlay = orderPlayEquipments[i];
                    try
                    {
                        List<string> adsTags = orderPlay.AdsTag?.Split(',').ToList() ?? new List<string>();

                        if (adsWhiteTags.Count > 0)
                        {
                            Boolean isInsWhiteTag = false;
                            //排除非白名单记录
                            foreach (var adsWhiteTag in adsWhiteTags)
                            {
                                if (adsTags.Exists(e => e.EndsWith(adsWhiteTag)))
                                {
                                    isInsWhiteTag = true;
                                    break;
                                }
                            }

                            if (isInsWhiteTag == false)
                                orderPlayEquipments.Remove(orderPlay);
                        }
                        else
                        {
                            //排除黑名单记录
                            foreach (var adsBlackTag in adsBlackTags)
                            {
                                if (adsTags.Exists(e => e.EndsWith(adsBlackTag)))
                                    orderPlayEquipments.Remove(orderPlay);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        orderPlayEquipments.Remove(orderPlay);
                    }
                }

                //计算可播放剩余时间
                for (int i = orderPlayEquipments.Count - 1; i >= 0; i--)
                {
                    var orderEquipment = orderPlayEquipments[i];

                    try
                    {
                        List<string> orderPlacesTimeRanges = orderEquipment.TimeRange != null ? orderEquipment.TimeRange.Split(',').ToList() : new List<string> { "0:00-23:59" };

                        double TodayAvailabletimes = 0;
                        double OnedayAvailabletimes = 0;
                        double TimeRangeAvailabletimes = 0;

                        foreach (var orderPlacesTimeRange in orderPlacesTimeRanges)
                        {
                            //无设置播放时间段的默认全天播放
                            List<string> orderPlacesTimes = orderPlacesTimeRange != null ? orderPlacesTimeRange.Split('-').ToList() : new List<string> { "0:00", "23:59" };
                            TimeRangeAvailabletimes = ((TimeSpan)(DateTime.Parse(orderPlacesTimes[1]).TimeOfDay - DateTime.Parse(orderPlacesTimes[0]).TimeOfDay)).TotalMinutes;
                            OnedayAvailabletimes = OnedayAvailabletimes + TimeRangeAvailabletimes;

                            if (DateTime.Now.TimeOfDay <= DateTime.Parse(orderPlacesTimes[1]).TimeOfDay)
                            {
                                if (DateTime.Now.TimeOfDay >= DateTime.Parse(orderPlacesTimes[0]).TimeOfDay)
                                {
                                    TodayAvailabletimes = TodayAvailabletimes + ((TimeSpan)(DateTime.Parse(orderPlacesTimes[1]).TimeOfDay - DateTime.Now.TimeOfDay)).TotalMinutes;
                                }
                                else
                                {
                                    TodayAvailabletimes = TodayAvailabletimes + TimeRangeAvailabletimes;
                                }
                            }
                        }
                        orderEquipment.Availabletimes = TodayAvailabletimes + OnedayAvailabletimes * (orderEquipment.EndDate - DateTime.Now).TotalDays;
                        orderEquipment.Totaltimes = OnedayAvailabletimes * (orderEquipment.EndDate - orderEquipment.StartDate).TotalDays;
                    }
                    catch (Exception)
                    {
                        orderEquipment.Availabletimes = 10;
                        orderEquipment.Totaltimes = 1;
                    }
                }

                var playlistorders = (from o in orderPlayEquipments
                                      join op in orderPools
                                      on  o.OrderTimeId.ToString() equals op.OrderTimeId.ToString()
                                      select new
                                      {
                                          Id = o.OrderId, 
                                          OrderTimeId = o.OrderTimeId,
                                          MateralType = o.MateralType,
                                          TotalSeconds = o.TotalSeconds,
                                          StartTime   = o.StartDate,
                                         // orderPlaces = o.place,
                                          listOrder = o.Availabletimes == 0 ? 0 : (o.Totaltimes == 0 ? 0 : op.AlreadyExposureCount / (o.ExposureCount * o.Availabletimes / o.Totaltimes)),
                                          //当前播放完成率=当前已播放次数/应该播放次数=当前已播放次数/ （订单播放总数* 已用时间/订单播放总时间）
                                      }).OrderBy(o => o.listOrder).ToList();

                for (int SortNO = 1; SortNO <= playlistorders.Count && SortNO <= ordercount; SortNO++)
                {
                    var order = playlistorders[SortNO - 1];
               
                    playOrders.Add(new PlayOrderModel
                    {
                        OrderTimeId = order.OrderTimeId.ToString(),
                        Sort = SortNO,
                        Duration = order.TotalSeconds,
                        StartTime = order.StartTime.ToString(),
                        PlayType = order.MateralType  // 播放类型，1 图片，2 视频；

                    });

                    List<OrderMaterialDto> orderMaterials = _orderMaterialService.GetByOrderId(order.Id);
                    foreach (var orderMaterial in orderMaterials)
                    {
                        MaterialDto material = _materialService.Get(orderMaterial.MaterialId);

                        playItems.Add(new PlayItemModel
                        {
                            FileId = material.Id.ToString(),
                            FileName = material.Name,
                            Md5 = material.MD5,
                            Sort = orderMaterial.Orderby,
                            PlayType = order.MateralType,
                            OrderTimeId = order.OrderTimeId.ToString(),
                            Duration = orderMaterial.Seconds
                        });
                    }
                }

                PlayListResultModel playListResult = new PlayListResultModel
                {
                    DriverId = deviceId,
                    HasChange = false,
                    Success = playItems?.Count()>0 ? true:false,
                    Data = new { PlayItems = playItems, PlayOrders = playOrders },
                    LastRequesTime = DateTime.Now
                };

                if (JsonConvert.SerializeObject(IsExistplay.Data, Formatting.None) != JsonConvert.SerializeObject(playListResult.Data, Formatting.None))
                {
                    playListResult.HasChange = true;
                    IsExistplay.Data = playListResult.Data;
                    IsExistplay.LastRequesTime = DateTime.Now;
                }

                CacheHelper.SetChacheValue(CacheKeys.PlayListResult, playListResults);

                return Json(playListResult);
            }
            catch (Exception ex)
            {
                return Json(new PlayListResultModel
                {
                    HasChange = true,
                    Success = false,
                    Data = ex.ToString(),
                });
            }
        }
        
        public FileStreamResult DownloadFile(string fileId, string deviceId)
        {
            try
            {
                EquipmentDto equipment = _equipmentService.GetByDeviceId(deviceId);
                if (equipment == null)
                    return null;

                MaterialInfoDto material = _materialService.GetInfo(new Guid(fileId));
                if (material == null)
                    return null;

                string materialName = material.Name.Replace(material.Name.Remove(material.Name.IndexOf(".")), fileId);

                byte[] files = FtpHelper.DownloadFtpFile(materialName);//  _filesAppService.GetInfo(material.Id).File;
                return File(new MemoryStream(files), "application/octet-stream", materialName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public JsonResult UploadPlayHis(string deviceId, decimal latitude, decimal longitude, string playHistorys) {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            var models = JsonConvert.DeserializeObject<List<PlayHistoryModel>>(playHistorys, settings);
            foreach (var item in models) {
                var dto = new PlayHistoryDto {
                    DeviceId = deviceId,
                    MaterialId = Guid.Parse(item.playId),
                    OrderTimeId = Guid.Parse(item.orderTimeId),
                    StartTime = DateTime.Parse(item.startTime),
                    EndTime = DateTime.Parse(item.endTime),
                    FrontalfaceCount = item.frontalfaceCount,
                    ProfilefaceCount = item.profilefaceCount,
                    ClickCount = item.clickCount,
                    Latitude = latitude,
                    Longitude = longitude
                };

                var orderPool = orderPools.FirstOrDefault(w => w.OrderTimeId.ToString() == item.orderTimeId.ToString());
                if (orderPool != null)
                {
                    orderPool.AlreadyExposureCount++;
                }
                _playHistoryService.Insert(ref dto);
            }
            return Json(new JsonResultModel {
                Success = true
            });

        }
        public void ExtraInfo(string deviceId, string orderTimeId) {

            var order = _orderService.Get(Guid.Parse(orderTimeId)) ?? _orderService.Get(_orderTimeService.Get(Guid.Parse(orderTimeId)).OrderId);
            ForwardHistoryDto dto = new ForwardHistoryDto{
                DeviceId = deviceId,
                OrderTimeId = Guid.Parse(orderTimeId),
                ForwardTime = DateTime.Now,
                ForwardUrl= order?.Url
            };
            _forwardHistoryService.InsertOrUpdate(ref dto);
            Response.Redirect(order?.Url??"http://www.lsinfo.com.cn", true);

        }

    }
}
