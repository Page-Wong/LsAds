using LsAdmin.Application.Imp;
using LsAdmin.Application.RegistEquipmentApp.Dto;
using RegistEquipment.Application.DataModel;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RegistEquipment.Application.RegistEquipmentApp {
    public interface IRegistEquipmentAppService : IBaseCasheAppService<Guid, RegistEquipmentDto>
    {

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool Insert(ref RegistEquipmentDto dto, out string errormessage);


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="deviceId">设备号</param>
        /// <param name="webSocket">webSocket</param>
        /// <param name="errormessage">错误信息</param>
        /// <returns></returns>
        bool Insert(string deviceId, WebSocket webSocket, out string errormessage);

        /// <summary>
        /// 根据设备Id获取一个实体
        /// </summary>
        /// <param name="deviceid">设备号</param>
        /// <returns></returns>
        List<RegistEquipmentDto> GetByDeviceId(string deviceid);


        /// <summary>
        /// 根据webSocket获取一个实体
        /// </summary>
        /// <param name="webSocket">webSocket</param>
        /// <returns></returns>
        RegistEquipmentDto GetByWebSocket(WebSocket webSocket);

        bool UpdateStatusById(Guid id, ushort status);

        bool UpdateStatusByWebSocket(WebSocket webSocket, ushort status);

        /// <summary>
        /// 删除超时记录
        /// </summary>
        void DeleteOverTimeList(DateTime dateTime);

        /// <summary>
        /// 获取注册界面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="errormessage"></param>
        /// <returns></returns>
        bool GetRegisteredUrl(string id, out string errormessage);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registeid"></param>
        /// <param name="ownerUserId"></param>
        /// <param name="equipmentName"></param>
        /// <param name="equipmentModeId"></param>
        /// <param name="errormessage"></param>
        /// <returns></returns>
        bool Registered(Guid registeid, Guid ownerUserId, string equipmentName, Guid? equipmentModeId, out string errormessage);

        Task<RegistResult> Register(Guid registeid, Guid ownerUserId, string equipmentName, Guid? equipmentModeId);

    }

}
