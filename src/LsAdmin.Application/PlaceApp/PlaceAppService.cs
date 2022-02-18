using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.PlaceApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using AutoMapper;
using System.Linq;
using LsAdmin.Application.EquipmentApp;
using LsAdmin.Application.FilesApp;
using LsAdmin.Application.EquipmentApp.Dtos;

namespace LsAdmin.Application.PlaceApp {
    public class PlaceAppService : BaseAppService<Place, PlaceDto>, IPlaceAppService {
        private readonly IPlaceRepository _repository;
        private readonly IEquipmentAppService _serverEquipment;
        private readonly IFilesAppService     _serveFilest;

        public PlaceAppService(IPlaceRepository repository, IHttpContextAccessor httpContextAccessor, IEquipmentAppService serverEquipment, IFilesAppService serveFilest) : base(repository, httpContextAccessor) {
            _repository = repository;
            _serverEquipment = serverEquipment;
            _serveFilest = serveFilest;


        }

        public List<PlaceDto> GetPlaceByType(Guid typeId, int startPage, int pageSize, out int rowCount)
        {
            return Mapper.Map<List<PlaceDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.TypeId == typeId, it => it.CreateTime));
        }

     

        public List<PlaceDto> GetParentPlace(PlaceDto place)
        {
            try
            {
                return  this.GetAllList().FindAll(f => ((f.Province == place.Province && f.City == place.City && f.District == place.District && f.Street == place.Street && f.StreetNumber=="")
                                                         || (f.Province == place.Province && f.City == place.City && f.District == place.District && f.Street == "")
                                                         || (f.Province == place.Province && f.City == place.City && f.District == "")
                                                         || (f.Province == place.Province && f.City == ""))
                                                         && f.Id!= place.Id
                                                        );              
            }
            catch (Exception )
            {
                return new List<PlaceDto>();
            }
        }
        public List<string> GetProvince()
        {
            return _repository.GetAllList().Select(s => s.Province).Distinct().ToList();
        }

        public List<string> GetCity(string province)
        {
            //var city= this.GetAllList().Where(w => w.Province == province).Select(s => s.City).Distinct().ToList();
            return this.GetAllList().FindAll(f => f.Province == province).Select(s => s.City).Distinct().ToList();
        }
        public List<string> GetDistrict(string province,string city)
        {
            //var city= this.GetAllList().Where(w => w.Province == province).Select(s => s.City).Distinct().ToList();
            return this.GetAllList().FindAll(f => f.Province == province && f.City==city).Select(s => s.District).Distinct().ToList();
        }

        public List<string> GetStreet(string province, string city,string district)
        {
            //var city= this.GetAllList().Where(w => w.Province == province).Select(s => s.City).Distinct().ToList();
            return this.GetAllList().FindAll(f => f.Province == province && f.City == city && f.District==district).Select(s => s.Street).Distinct().ToList();
        }

        /// <summary>
        /// 返回指定区域播放总时长，单位为分钟
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district "></param>
        /// <param name="street"></param>
        /// <param name="adsTag"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public double GetAreaStockPlayMinutes(DateTime startDate, DateTime endDate, string province, string city, string district ="", string street="",  string adsTag="", string timeRange="")
        {
            double AreaStockPlayMinutes =0.0001;
            try {
                if (endDate.Date < DateTime.Now.Date)
                {
                    return AreaStockPlayMinutes;
                }
            
                DateTime playStartDate = startDate.Date > DateTime.Now.Date ? startDate.Date : DateTime.Now.Date;

                DateTime playEndDate = endDate.Date;

                int playDays = playEndDate.DayOfYear - playStartDate.DayOfYear + 1;

                List <string> adsTags = !string.IsNullOrEmpty(adsTag) ? adsTag.Split(',').ToList() : new List<string>();

                //没有设置播放时间段的就默认全天播new List<string> { "0:00-23:59" };
                List<string> areaTimeRange = !string.IsNullOrEmpty(timeRange) ? timeRange.Split(',').ToList() : new List<string> { "0:00-23:59" };

                var playPlaces = this.GetAllList();
                if (!string.IsNullOrEmpty(province)) {
                    playPlaces = playPlaces.FindAll(f => f.Province == province);
              
                    if (!string.IsNullOrEmpty(city)) {
                        playPlaces = playPlaces.FindAll(f => f.City == city);

                        if (!string.IsNullOrEmpty(district ))
                        {
                            playPlaces = playPlaces.FindAll(f => f.District == district );

                            if (!string.IsNullOrEmpty(street)){
                                playPlaces = playPlaces.FindAll(f => f.Street == street);
                            }
                        }
                    }
                }
                else
                {
                    return AreaStockPlayMinutes;
                }

                //排除非白名单记录&&排除黑名单记录
                for (int i = playPlaces.Count - 1; i >= 0; i--)
                {
                    var playPlace = playPlaces[i];
                    try
                    {
                        List<string> adsWhiteTags =  playPlace.AdsWhiteTag !="" ?  playPlace.AdsWhiteTag.Split(',').ToList() : new List<string>();
                        List<string> adsBlackTags =  playPlace.AdsBlackTag != "" ? playPlace.AdsBlackTag.Split(',').ToList() : new List<string>();

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
                                playPlaces.Remove(playPlace);
                        }
                        else
                        {
                            //排除黑名单记录
                            foreach (var adsBlackTag in adsBlackTags)
                            {
                                if (adsTags.Exists(e => e.EndsWith(adsBlackTag)))
                                    playPlaces.Remove(playPlace);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        playPlaces.Remove(playPlace);
                    }
                }



                //排除播放时间段不包含当前时间的记录
                for (int i = playPlaces.Count - 1; i >= 0; i--)
                {
                    var playPlace = playPlaces[i];
                    try
                    {
                        //无设置播放时间段的默认全天播放
                        if (playPlace.TimeRange != null)
                        {
                            List<string> orderPlacesTimeRanges = playPlace.TimeRange.Split(',').ToList();

                            Boolean isInorderPlaceTimeRange = false;
                            //排除播放时间段不包含当前时间的记录
                            foreach (var orderPlacesTimeRange in orderPlacesTimeRanges)
                            {
                                List<string> orderPlacesTimes = !string.IsNullOrEmpty(orderPlacesTimeRange) ? orderPlacesTimeRange.Split('-').ToList() : new List<string> { "0:00", "23:59" };

                                if (DateTime.Now.TimeOfDay >= DateTime.Parse(orderPlacesTimes[0]).TimeOfDay && DateTime.Now.TimeOfDay <= DateTime.Parse(orderPlacesTimes[1]).TimeOfDay)
                                {
                                    isInorderPlaceTimeRange = true;
                                    break;
                                }
                            }
                            if (isInorderPlaceTimeRange == false)
                                playPlaces.Remove(playPlace);
                        }
                    }
                    catch (Exception)
                    {
                        playPlaces.Remove(playPlace);
                    }
                }


                //计算可播放剩余时间
                for (int i = playPlaces.Count - 1; i >= 0; i--)
                {
                    var playPlace = playPlaces[i];

                    try
                    {
                        List<string> playPlaceTimeRanges = !string.IsNullOrEmpty(playPlace.TimeRange) ? playPlace.TimeRange.Split(',').ToList() : new List<string> { "0:00-23:59" };

                        double TodayAvailabletimes = 0;
                        double OnedayAvailabletimes = 0;
                        double TimeRangeAvailabletimes = 0;

                        foreach (var playPlaceTimeRange in playPlaceTimeRanges)
                        {
                            //无设置播放时间段的默认全天播放
                            List<string> PlacesTimes =!string.IsNullOrEmpty(playPlaceTimeRange)  ? playPlaceTimeRange.Split('-').ToList() : new List<string> { "0:00", "23:59" };

                            TimeRangeAvailabletimes = ((TimeSpan)(DateTime.Parse(PlacesTimes[1]).TimeOfDay - DateTime.Parse(PlacesTimes[0]).TimeOfDay)).TotalMinutes;

                            OnedayAvailabletimes = OnedayAvailabletimes + TimeRangeAvailabletimes;

                           
                            if (playStartDate.Day==DateTime.Now.Day && DateTime.Now.TimeOfDay <= DateTime.Parse(PlacesTimes[1]).TimeOfDay)
                            {
                                if (DateTime.Now.TimeOfDay >= DateTime.Parse(PlacesTimes[0]).TimeOfDay)
                                {
                                    TodayAvailabletimes = TodayAvailabletimes + ((TimeSpan)(DateTime.Parse(PlacesTimes[1]).TimeOfDay - DateTime.Now.TimeOfDay)).TotalMinutes;
                                }
                                else
                                {
                                    TodayAvailabletimes = TodayAvailabletimes + TimeRangeAvailabletimes;
                                }
                            }
                        }

                        if (TodayAvailabletimes > 0)
                        {
                            AreaStockPlayMinutes += playDays * (OnedayAvailabletimes-1)+ TodayAvailabletimes;
                        }
                        else
                        {
                            AreaStockPlayMinutes += playDays * OnedayAvailabletimes;
                        }

                    }
                    catch (Exception)
                    {
                        return AreaStockPlayMinutes;
                    }
                }
            }
            catch(Exception)
            {
                return AreaStockPlayMinutes;
            }

            return AreaStockPlayMinutes;
        }

        /// <summary>
        /// 删除场所信息
        /// </summary>
        /// <param name="id">场所id</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns></returns>
        public bool DeletePlace(Guid id, out string errormessage)
        {
            try
            {
                var place = Get(id);
                if (place == null)
                {
                    errormessage = "记录中不存在你要删除的记录！";
                    return false;
                }

                if (_serverEquipment.GetAllEquipmentByPlace(id).Count() > 0)
                {
                    errormessage = "场所已存在投放设备，不能删除场所记录！";
                    return false;
                }
                _serveFilest.DeletetOwnerObj(id);
                Delete(id);

            }catch(Exception e)
            {
                errormessage = "记录删除出错！";
                return false;
            }

            errormessage = "";
            return true;
        }

        public bool DeleteBatchPlaces(List<Guid> ids, out string errormessage)
        {
            try{
                foreach (var id in ids) { 
                    if (_serverEquipment.GetAllEquipmentByPlace(id).Count() > 0){
                        errormessage = "你所选择的场所存在投放设备，不能删除场所记录！";
                        return false;
                    }
                }

                foreach (var id in ids){
                    DeletePlace(id, out errormessage);
                }
            }
            catch (Exception e)
            {
                errormessage = "记录删除出错！";
                return false;
            }

            errormessage = "";
            return true;
        }

        public List<PlaceDto> GetUserPagePlaceByType(Guid typeId, int startPage, int pageSize, Guid UserId, out int rowCount)
        {             
            return Mapper.Map<List<PlaceDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.TypeId == typeId && it.OwnerUserId== UserId, it => it.CreateTime));
        }

        public List<PlaceDto> GetUserPagePlaces(int startPage, int pageSize, Guid UserId, out int rowCount)
        {
            return Mapper.Map<List<PlaceDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it=> it.OwnerUserId == UserId, _orderby, _orderbyDesc));
        }

        public List<PlaceDto> GetUserAllPlaces(Guid UserId) {
            return Mapper.Map<List<PlaceDto>>(_repository.GetAllList(it => it.OwnerUserId == CurrentUser.Id));
        }

        #region 场所设备相关
        public List<EquipmentDto> GetUserPageEquipments(int startPage, int pageSize, Guid userId, out int rowCount, List<uint> status = null) {
            var equipments = GetUserAllEquipments(userId);
            if (status != null) {
                equipments = equipments.Where(it => status.Contains(it.Status)).ToList();
            }
            rowCount = equipments.Count();
            return equipments.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<EquipmentDto> GetUserAllEquipments(Guid UserId) {

            var places = GetUserAllPlaces(UserId);
            var equipments = _serverEquipment.GetAllEquipmentByPlaces(places.Select(it => it.Id).ToList());
            return Mapper.Map<List<EquipmentDto>>(equipments);
        }
        #endregion
    }
}
