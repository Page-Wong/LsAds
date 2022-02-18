using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.OrderTimeApp.Dtos;
using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using System.Collections.Generic;
using System;
using LsAdmin.Application.PlaceMaterialApp.Dtos;
using LsAdmin.Application.TradeApp.Dtos;

namespace LsAdmin.Application.OrderApp {
    public interface IOrderAppService : IBaseAppService<OrderDto>
    {
        OrderDto GetByOrderNo(string orderNo);


        /// <summary>
        /// 保存订单信息和订单扩展信息
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="orderMaterials">订单素材信息</param>
        /// <param name="orderTimes">订单时间信息</param>
        /// <returns></returns>
        bool SaveOrder(ref OrderDto order, Guid programID,List<OrderAreaModel> orderAreas, List<OrderTimeDto> orderTimes);

        /// <summary>
        /// 保存订单信息和订单扩展信息
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="orderMaterials">订单素材信息</param>
        /// <param name="orderEquipments">订单设备信息</param>
        /// <returns></returns>
        bool Save(ref OrderDto order, List<OrderMaterialDto> orderMaterials, List<OrderEquipmentDto> orderEquipments);

        /// <summary>
        /// 保存订单信息和订单扩展信息
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="programID">方案</param>
        /// <param name="orderEquipments">订单设备信息</param>
        /// <returns></returns>
        bool SaveAllEquipments(ref OrderDto order, Guid programID, List<OrderEquipmentDto> orderEquipments);

        //bool SavePlaceMaterialOrder(Guid placeId, List<PlaceMaterialDto> dtos);
        List<OrderDto> GetCurrentUserAllList();
        List<OrderDto> GetCurrentUserAllPageList(int startPage, int pageSize, out int rowCount);
        /// <summary>
        /// 获取设备的素材列表
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        Dictionary<string, string> GetSyncPlayFile(Guid equipmentId);

        List<Guid> GetCanPalyOrderIds();

        List<OrderPlayEquipmentModel> GetEquipmentOrderTimes(Guid equipmentId);

        List<OrderPlayEquipmentModel> GetEquipmentOrders(Guid equipmentId);

        bool ApplyRefund(Guid UserId, OrderDto Dto, string ApplyRefundReason);
        List<OrderDto> GetAllRefundPageList(int startPage, int pageSize, out int rowCount);

        List<Guid> GetePlaceMaterialOrde(Guid placeId);

        bool PayByPoint(Guid userId, OrderDto dto, out TradeDto tradeDto);
        bool PayByAlipay(Guid userId, OrderDto dto, out TradeDto tradeDto);

        bool AfterPay(OrderDto dto, TradeDto tradeDto);

        List<TradeDto> GetTradesByOrderId(Guid id);
        List<TradeDto> GetTradesWithBusinessTypeByOrderId(Guid id);

        /// <summary>
        /// 获取带播放器信息的订单对象
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>订单对象</returns>
        OrderDto GetWithPlayer(Guid id);
        /// <summary>
        /// 为订单增加播放器
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="playerId">播放器Id</param>
        /// <returns>操作是否成功</returns>
        bool AddPlayer(Guid id, Guid playerId);
        /// <summary>
        /// 删除订单的播放器
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="playerId">播放器Id</param>
        /// <returns>操作是否成功</returns>
        bool RemovePlayer(Guid id, Guid playerId);

        /// <summary>
        /// 获取带可播放节目信息的订单对象
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>订单对象</returns>
        OrderDto GetWithPlayerProgram(Guid id);
        /// <summary>
        /// 为订单增加可播放节目
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="playerProgramId">可播放节目Id</param>
        /// <returns>操作是否成功</returns>
        bool AddPlayerProgram(Guid id, Guid playerProgramId);

        /// <summary>
        /// 删除订单可播放节目
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <param name="playerProgramId">可播放节目Id</param>
        /// <returns>操作是否成功</returns>
        bool RemovePlayerProgram(Guid id, Guid playerProgramId);

        /// 获取订单的方案
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>方案Id</returns>
        Guid GetOrderProgram(Guid id);

        /// 获取订单的播放器
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>方案Id</returns>
        List<Guid> GetOrderPlayers(Guid id);

        /// <summary>
        /// 删除所有订单可播放节目
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>操作是否成功</returns>
        bool RemoveAllPlayerProgram(Guid id);


         /// <summary>
         /// 保存订单_by_hua
         /// </summary>
         /// <param name="order">订单</param>
         /// <param name="programID">节目id</param>
         /// <param name="playIds">播放器id列表</param>
         /// <returns>保存成功返回True否则返回false</returns>
         bool SaveAll(ref OrderDto order, Guid programID, List<Guid> playIds);
    }
}
