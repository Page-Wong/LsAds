using LsAdmin.Application.TradeBusinessTypeApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.TradeBusinessTypeApp {
    public interface ITradeBusinessTypeAppService : IBaseAppService<TradeBusinessTypeDto>
    {
        Guid GetIdByName(string name);
    }
}
