using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.TradeApp.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using System.Linq.Expressions;
using LsAdmin.Application.TradeBusinessTypeApp;

namespace LsAdmin.Application.TradeApp {
    public class TradeAppService : BaseAppService<Trade, TradeDto>, ITradeAppService {

        private readonly ITradeRepository _repository;
        private readonly ITradeBusinessTypeAppService _tradeBusinessTypeService;
        public TradeAppService(ITradeRepository repository, ITradeBusinessTypeAppService tradeBusinessTypeService, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _tradeBusinessTypeService = tradeBusinessTypeService;
            _orderbyDesc = item => item.TradeTime;
        }

        /// <summary>
        /// 获取当前用户所有交易分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public List<TradeDto> GetCurrentUserAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<TradeDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, o => o.UserId == CurrentUser.Id, _orderby, _orderbyDesc));
        }

        public TradeDto GetByTradeNo(string tradeNo) {
            return Mapper.Map<TradeDto>(_repository.GetEntities().FirstOrDefault(item=>item.TradeNo == tradeNo));
        }



        #region 积分
        /// <summary>
        /// 获取当前用户积分分页列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public List<TradeDto> GetCurrentUserPointAllPageList(int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<TradeDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, o => o.UserId == CurrentUser.Id && o.TradeMethod == TradeDto.TRADEMETHOD_POINT, _orderby, _orderbyDesc));
        }

        /// <summary>
        /// 获取当前用户所有积分，排除锁定与解锁
        /// </summary>
        /// <returns></returns>
        public float GetCurrentUserPointAllBalance() {
            var list = _repository.GetEntities().Where(item => 
                item.UserId == CurrentUser.Id && 
                item.TradeStatus == TradeDto.TRADESTATUS_SUCCESS && 
                item.TradeMethod == TradeDto.TRADEMETHOD_POINT
                ).ToList();
            return list.Sum(item => item.Amount);
        }
        /// <summary>
        /// 获取当前用户所有可用积分，包括锁定与解锁
        /// </summary>
        /// <returns></returns>
        public float GetCurrentUserPointBalance() {
            var list = _repository.GetEntities().Where(item => 
                item.UserId == CurrentUser.Id && 
                item.TradeStatus == TradeDto.TRADESTATUS_SUCCESS &&
                item.TradeMethod == TradeDto.TRADEMETHOD_POINT
                ).ToList();
            return list.Sum(item => item.Amount);
        }
        /// <summary>
        /// 获取当前用户时间范围内的积分消费
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public float GetCurrentUserPointExpenseByDate(DateTime startDate, DateTime endDate) {
            ParameterExpression parameter1 = Expression.Parameter(typeof(Trade), "item");
            Expression<Func<Trade, bool>> where = 
                item => item.UserId == CurrentUser.Id &&
                item.TradeStatus == TradeDto.TRADESTATUS_SUCCESS &&
                item.Type == TradeDto.TYPE_SPEND &&
                item.TradeMethod == TradeDto.TRADEMETHOD_POINT;

            var entities = _repository.GetEntities().Where(where);
            //如果开始日期不为空，则添加开始日期条件
            if (startDate != null) {
                entities = entities.Where(item => item.TradeTime >= startDate);
            }

            //如果结束日期不为空，则添加结束日期条件
            if (endDate != null) {
                entities = entities.Where(item => item.TradeTime <= endDate);
            }
            var list = entities.ToList();
            return list.Sum(item => item.Amount) * -1;
        }


        /// <summary>
        /// 增加当前用户积分
        /// </summary>
        /// <param name="TradeDto">交易对象，，需预先设置BusinessTypeId、Amount、Remarks、TradeStatus，其中Amount为正数</param>
        /// <returns></returns>
        public bool IncreaseCurrentUserPointIncome(ref TradeDto dto) {
            dto.UserId = CurrentUser.Id;
            return IncreasePointIncome(ref dto);
        }
        /// <summary>
        /// 增加积分
        /// </summary>
        /// <param name="TradeDto">交易对象，，需预先设置BusinessTypeId、UserId、Amount、Remarks、TradeStatus，其中Amount为正数</param>
        /// <returns></returns>
        public bool IncreasePointIncome(ref TradeDto dto) {
            dto.TradeMethod = TradeDto.TRADEMETHOD_POINT;
            dto.TradeNo = CreateNewTradeNo();
            dto.TradeTime = DateTime.Now;
            dto.Type = TradeDto.TYPE_INCOME;
            dto.Amount = Math.Abs(dto.Amount);

            return Insert(ref dto);
        }
        /// <summary>
        /// 消费当前用户积分
        /// </summary>
        /// <param name="TradeDto">交易对象，，需预先设置BusinessTypeId、Amount、Remarks、TradeStatus，其中Amount为正数</param>
        /// <returns></returns>
        public bool ExpenseCurrentUserPoint(ref TradeDto dto) {
            dto.UserId = CurrentUser.Id;
            return ExpensePoint(ref dto);
        }
        /// <summary>
        /// 消费积分
        /// </summary>
        /// <param name="TradeDto">交易对象，，需预先设置BusinessTypeId、UserId、Amount、Remarks、TradeStatus，其中Amount为正数</param>
        /// <returns></returns>
        public bool ExpensePoint(ref TradeDto dto) {
            dto.TradeMethod = TradeDto.TRADEMETHOD_POINT;
            dto.TradeNo = CreateNewTradeNo();
            dto.TradeTime = DateTime.Now;
            dto.Type = TradeDto.TYPE_SPEND;
            dto.Amount = Math.Abs(dto.Amount) * -1;

            return Insert(ref dto);
        }
        #endregion

        #region 支付宝 
        /// <summary>
        /// 支付宝支付
        /// </summary>
        /// <param name="TradeDto">交易对象，，需预先设置BusinessTypeId、UserId、Amount、Remarks、TradeStatus，其中Amount为正数</param>
        /// <returns></returns>
        public bool PayByAlipay(ref TradeDto dto) {
            dto.TradeMethod = TradeDto.TRADEMETHOD_ALIPAY;
            dto.TradeNo = CreateNewTradeNo();
            dto.TradeStatus = TradeDto.TRADESTATUS_TRADING;
            dto.TradeTime = DateTime.Now;
            dto.Type = TradeDto.TYPE_SPEND;
            dto.Amount = Math.Abs(dto.Amount) * -1;
            
            return Insert(ref dto);
        }
        /// <summary>
        /// 申请支付宝退款
        /// </summary>
        /// <param name="TradeDto">交易对象，，需预先设置BusinessTypeId、UserId、Amount、Remarks、TradeStatus，其中Amount为正数</param>
        /// <returns></returns>
        public bool ApplyRefundByAlipay(ref TradeDto dto) {
            dto.TradeMethod = TradeDto.TRADEMETHOD_ALIPAY;
            dto.TradeNo = CreateNewTradeNo();
            dto.TradeStatus = TradeDto.TRADESTATUS_TRADING;
            dto.TradeTime = DateTime.Now;
            dto.Type = TradeDto.TYPE_INCOME;
            dto.Amount = Math.Abs(dto.Amount);

            return Insert(ref dto);
        }
        #endregion        

        private string CreateNewTradeNo() {
            return DateTime.Now.ToFileTimeUtc() + new Random().Next(100, 999).ToString();
        }

    }
}
