using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System;
using LsAdmin.Application.PlayPriceApp.Dtos;
using System.Collections.Generic;
using AutoMapper;

namespace LsAdmin.Application.PlayPriceApp
{
    public  class PlayPriceAppService : BaseAppService<PlayPrice, PlayPriceDto>, IPlayPriceAppService
    {
        private readonly IPlayPriceRepository _repository;
        public PlayPriceAppService(IPlayPriceRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        /// <summary>
        /// 获取区域的播放单价
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="street"></param>
        /// <returns></returns>
        public List<PlayPriceDto> GetAreaPlayPrices(string province = "", string city = "", string district = "", string street = "")
        {
            try { 
                var PlayPrices = _repository.GetAllList(w => w.PlayType=="area" && w.StartDate<=DateTime.Now.Date && w.EndDate>=DateTime.Now.Date);
                if (province != ""){
                   PlayPrices = PlayPrices.Where(w => w.Province==province).ToList();
                    if (city != ""){
                        PlayPrices= PlayPrices.Where(w => w.City == city).ToList();
                        if (district != ""){
                            PlayPrices = PlayPrices.Where(w => w.District == district).ToList();
                            if(street!="")
                                PlayPrices = PlayPrices.Where(w => w.Street == street).ToList();
                        }
                    }
                }
                return Mapper.Map<List<PlayPriceDto>>(PlayPrices) ?? new List<PlayPriceDto>();
            }
            catch(Exception)
            {
                return new List<PlayPriceDto>();
            }
          
        }

        /// <summary>
        /// 获取套餐的单价
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public List<PlayPriceDto> GetComboPlayPrices(string combo)
        {
            try
            {
                var PlayPrices = _repository.GetAllList(w => w.PlayType == "combo" && w.StartDate <= DateTime.Now.Date && w.EndDate >= DateTime.Now.Date && w.combo == combo);
                return Mapper.Map<List<PlayPriceDto>>(PlayPrices) ?? new List<PlayPriceDto>();
            }
            catch (Exception)
            {
                return new List<PlayPriceDto>();
            }
        }

        /// <summary>
        /// 获取设备某套餐的播放单价
        /// </summary>
        /// <param name="equipmentid"></param>
        /// <returns></returns>
        public PlayPriceDto GetEquipmentPlayPrice(Guid equipmentid,string combo)
        {
            try
            {
                var PlayPrice = _repository.GetAllList(w => w.PlayType == "equipment" && w.StartDate <= DateTime.Now.Date && w.EndDate >= DateTime.Now.Date && w.EquipmentID == equipmentid && w.combo == combo).FirstOrDefault();
                return Mapper.Map<PlayPriceDto>(PlayPrice) ?? new PlayPriceDto();
            }
            catch (Exception ex)
            {
                return new PlayPriceDto();
            }

        }

        /// <summary>
        /// 获取设备的播放单价
        /// </summary>
        /// <param name="equipmentid"></param>
        /// <returns></returns>
        public List<PlayPriceDto> GetEquipmentPlayPrices(Guid equipmentid)
        {
            try
            {
                var PlayPrices = _repository.GetAllList(w => w.PlayType == "equipment" && w.StartDate <= DateTime.Now.Date && w.EndDate >= DateTime.Now.Date && w.EquipmentID == equipmentid);
                return Mapper.Map<List<PlayPriceDto>>(PlayPrices) ?? new List<PlayPriceDto>();
            }
            catch (Exception)
            {
                return new List<PlayPriceDto>();
            }

        }

        /// <summary>
        ///  获取地点的播放单价
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /*public List<PlayPriceDto> GetPlacePlayPrices(Guid placeid)
        {
            try
            {
                var PlayPrices = _repository.GetAllList(w => w.PlayType == "place" && w.StartDate <= DateTime.Now.Date && w.EndDate >= DateTime.Now.Date && w.PlaceID == placeid);
                return Mapper.Map<List<PlayPriceDto>>(PlayPrices) ?? new List<PlayPriceDto>();
            }
            catch (Exception)
            {
                return new List<PlayPriceDto>();
            }

        }*/
    }
}



