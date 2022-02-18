using LsAdmin.Application.Imp;
using System;
using LsAdmin.Application.PlayPriceApp.Dtos;
using System.Collections.Generic;

namespace LsAdmin.Application.PlayPriceApp
{
    public interface IPlayPriceAppService : IBaseAppService<PlayPriceDto>
    {

        List<PlayPriceDto> GetAreaPlayPrices(string province="", string city="", string distinct = "", string street = "");
        //List<PlayPriceDto> GetPlacePlayPrices(Guid placeid);
        List<PlayPriceDto> GetEquipmentPlayPrices(Guid equipmentid);
        List<PlayPriceDto> GetComboPlayPrices(string combo);

        PlayPriceDto GetEquipmentPlayPrice(Guid equipmentid,string combo);
    }
}

