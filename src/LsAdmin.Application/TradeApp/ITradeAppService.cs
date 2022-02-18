using LsAdmin.Application.TradeApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;

namespace LsAdmin.Application.TradeApp {
    public interface ITradeAppService : IBaseAppService<TradeDto>
    {
        /// <summary>
        /// 获取当前用户所有交易分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<TradeDto> GetCurrentUserAllPageList(int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 获取当前用户积分分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<TradeDto> GetCurrentUserPointAllPageList(int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 获取当前用户可用余额
        /// </summary>
        /// <returns></returns>
        float GetCurrentUserPointBalance();

        /// <summary>
        /// 获取当前用户全部余额
        /// </summary>
        /// <returns></returns>
        float GetCurrentUserPointAllBalance();

        /// <summary>
        /// 按时间范围统计消费
        /// </summary>
        /// <param name="StartDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <returns></returns>
        float GetCurrentUserPointExpenseByDate(DateTime StartDate, DateTime EndDate);

        /// <summary>
        /// 根据单号获取交易数据
        /// </summary>
        /// <param name="tradeNo">订单号</param>
        /// <returns></returns>
        TradeDto GetByTradeNo(string tradeNo);

        bool IncreaseCurrentUserPointIncome(ref TradeDto dto);
        bool IncreasePointIncome(ref TradeDto dto);
        bool ExpenseCurrentUserPoint(ref TradeDto dto);
        bool ExpensePoint(ref TradeDto dto);
        bool ApplyRefundByAlipay(ref TradeDto dto);
        bool PayByAlipay(ref TradeDto dto);
    }
}
