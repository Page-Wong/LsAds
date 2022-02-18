using LsAdmin.Application.PlayHistoryApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.PlayHistoryApp {
    public interface IPlayHistoryAppService : IBaseAppService<PlayHistoryDto> {
        int GetCountByOrderTimeId(Guid orderId);
        int GetCountByOrderTimeIds(Guid[] orderIds);

        List<PlayHistoryDto> GetPlayHisByOrderidOrOrdertimeid(Guid orderId);
    }
}
