using LsAdmin.Application.Imp;
using LsAdmin.Application.LabelApp.Dtos;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LsAdmin.Application.LabelApp
{
    public interface ILabelAppService:IBaseAppService<LabelDto>
    {
        List<LabelDto> GetAdsTag();
        List<LabelDto> GetPlaceTag();
    }
}
