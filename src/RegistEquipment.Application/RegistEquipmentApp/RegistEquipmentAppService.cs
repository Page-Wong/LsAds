using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Linq;
using RegistEquipment.Application.DataModel;
using System.Threading.Tasks;
using LsAdmin.Application.RegistEquipmentApp.Dto;

namespace RegistEquipment.Application.RegistEquipmentApp {
    public class RegistEquipmentAppService : BaseCacheAppService<Guid, RegistEquipmentDto>, IRegistEquipmentAppService
    {

        //IEquipmentAppService _equipmentService;
        public double _aeadMinutes = 10;

        public RegistEquipmentAppService(/*IEquipmentAppService equipmentService*/) : base() {
          //  _equipmentService = equipmentService;
        }

        public void DeleteOverTimeList(DateTime dateTime)
        {
            dateTime = dateTime!=null? dateTime :  DateTime.Now;
            DeleteBatch(GetAllList().Where(w => w.AeadTime <= dateTime).Select(s => (Guid)s.Id).ToList());
        }

        public List<RegistEquipmentDto> GetByDeviceId(string deviceid)
        {
           return  GetAllList().Where(w => w.DeviceId == deviceid).ToList();
        }

        public RegistEquipmentDto GetByWebSocket(WebSocket webSocket)
        {
            return GetAllList().Where(w => w.WebSocket == webSocket).FirstOrDefault();
        }

        public bool GetRegisteredUrl(string id, out string errormessage)
        {
            errormessage = "";
            try
            {
                if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid preRegisteredDeviceId))
                {
                    errormessage = "参数信息有误";
                    return false;
                }

                RegistEquipmentDto registEquipmentDto = Get(preRegisteredDeviceId);

                if (registEquipmentDto == null)
                {
                    errormessage = "预注册记录状态信息有误，请重新注册！";
                    return false;
                };


             /*   if (_equipmentService.GetByDeviceId(registEquipmentDto.DeviceId) != null)
                {
                    errormessage = "设备已注册，不能重新注册！";
                    Delete(preRegisteredDeviceId);
                    return false;
                }*/
                //更新预注册记录状态为已扫描
                UpdateStatusById(preRegisteredDeviceId, 1);

                if (registEquipmentDto.IsDefunct)
                {
                    errormessage = "注册链接已过期，请重新注册！";
                    Delete(preRegisteredDeviceId);
                    return false;
                }
                else if (!registEquipmentDto.isCanRegisteredStatus)
                {
                    errormessage = "预注册记录状态信息有误，请重新注册！";
                    return false;
                }
             

                //更新预注册记录状态为已发送
                UpdateStatusById(preRegisteredDeviceId, 2);
                return true;
            }
            catch (Exception ex)
            {
                errormessage = "程序出错";
                return false;
            }
        }

        public bool Insert(ref RegistEquipmentDto dto, out string errormessage)
        {
            try
            {
                errormessage = "";

                var _webSocket = dto.WebSocket;
                var _deviceId = dto.DeviceId;

                if (_webSocket == null) { errormessage = "WebSocket信息为空！"; return false; }
                if (_deviceId == null) { errormessage = "设备号为空！"; return false; }
                //if (_equipmentService.GetByDeviceId(_deviceId) != null) { errormessage = "设备已注册不能重新注册！"; return false; }

                foreach (var id in  GetAllList().Where(w => w.WebSocket == _webSocket).Select(s => s.Id).ToList())
                {
                    Delete((Guid)id);
                }

                foreach (var id in GetByDeviceId(_deviceId).Select(s => s.Id).ToList())
                {
                    Delete((Guid)id);
                }

               // dto.Status =  0;
                dto.Id = dto.Id == null ? Guid.NewGuid() : dto.Id;
                dto.ApplyTime = dto.ApplyTime == null ? DateTime.Now : dto.ApplyTime;
                dto.AeadMinutes = dto.AeadMinutes == 0 ? _aeadMinutes : dto.AeadMinutes;

                Insert(dto);
                return true;
            }
            catch (Exception ex)
            {
                errormessage = ex.ToString();
                return false;
            }
        }

        public bool Insert(string deviceId, WebSocket webSocket, out string errormessage)
        {
            try
            {
                errormessage = "";
                RegistEquipmentDto dto = new RegistEquipmentDto
                {
                    DeviceId = deviceId,
                    WebSocket = webSocket,
                    ApplyTime = DateTime.Now,
                    AeadMinutes = _aeadMinutes,
                    Status = 0,
                    Id = Guid.NewGuid(),
                };

                return Insert(ref dto, out errormessage);
            }
            catch (Exception ex)
            {
                errormessage = ex.ToString();
                return false;
            }
        }

        public bool Registered(Guid id, Guid ownerUserId, string equipmentName, Guid? equipmentModeId, out string errormessage)
        {
            errormessage = "";
            try
            {
 
                if (id==null)
                {
                    errormessage = "参数信息有误！";
                    return false;
                }

                if (ownerUserId==null)
                {
                    errormessage = "设备所有者信息有误！";
                    return false;
                }

                if (string.IsNullOrEmpty(equipmentName))
                {
                    errormessage = "设备名称不为空！";
                    return false;
                }
                UpdateStatusById(id, 3);
                return true;
                /* if (_equipmentService.Registered(id, ownerUserId, equipmentName, equipmentModeId, out errormessage))
                 {
                     UpdateStatusById(preRegisteredDeviceId, 3);
                     return true;
                 }
                 else
                 {
                     return false;
                 }*/
            }
            catch (Exception ex)
            {
                errormessage = "程序出错";
                return false;
            }
        }

        public bool UpdateStatusById(Guid id, ushort status)
        {
            try
            {
                var _dto = base.Get(id);
                if (_dto == null) return false;
                _dto.Status = status;
                Update(_dto);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStatusByWebSocket(WebSocket webSocket, ushort status)
        {
            try
            {
                var _dto = GetByWebSocket(webSocket);
                if (_dto == null) return false;
                _dto.Status = status;
                Update(_dto);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<RegistResult> Register(Guid registeid, Guid ownerUserId, string equipmentName, Guid? equipmentModeId)
        {
            if (registeid==null)
                return await Task.Run(() => new RegistResult { Code = RegistResult.ResultCode.REGIST_NONE });

            if (ownerUserId == null)
                return await Task.Run(() => new RegistResult { Code = RegistResult.ResultCode.REGIST_OWNERUSERID_NONE });

            if (string.IsNullOrEmpty(equipmentName))
                return await Task.Run(() => new RegistResult { Code = RegistResult.ResultCode.REGIST_EQUIPMENTNAME_NONE });

            var dto = Get(registeid);

            if (dto == null || !dto.isCanRegisteredStatus)
                return await Task.Run(() => new RegistResult { Code = RegistResult.ResultCode.REGIST_NONE });

            if (dto.IsDefunct)
                return await Task.Run(() => new RegistResult { Code = RegistResult.ResultCode.REGIST_OVERTIME });

            UpdateStatusById(registeid, 3);

            return await Task.Run(() => new RegistResult { Code = RegistResult.ResultCode.SUCCESS });
        }
    }
}
