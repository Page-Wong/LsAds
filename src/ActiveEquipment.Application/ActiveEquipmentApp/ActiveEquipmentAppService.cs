using LsAdmin.Application.Imp;
using LsAdmin.Application.EquipmentApp.Dtos;
using System;
using System.Net.WebSockets;
using ActiveEquipment.Application.DataModel;

namespace ActiveEquipment.Application.ActiveEquipmentApp {
    public class ActiveEquipmentAppService : BaseCacheAppService<Guid, ActiveEquipmentDto>, IActiveEquipmentAppService {

        public ActiveEquipmentAppService() : base() {
        }

        public ActiveEquipmentDto GetBySocket(WebSocket socket) {
            return Data.Find(it => it.WebSocket == socket);
        }

        public ActiveEquipmentDto GetByToken(string token) {
            return Data.Find(it => it.Token == token);
        }

        public bool IsCanActive(ActiveEquipmentDto item) {
            var result = new Result();
            if (item.Token == null) {
                result.Code = Result.ResultCode.EQUIPMENT_TOKEN_NONE;
                //TODO G 将验证结果写入操作日志
                return false;
            }
            if (item.Equipment == null) {
                result.Code = Result.ResultCode.EQUIPMENT_NONE;
                //TODO G 将验证结果写入操作日志
                return false;
            }
            if (Data.Exists(d => d != null && d.EquipmentId == item.EquipmentId && d.Ip.ToString() != item.Ip.ToString())) {
                result.Code = Result.ResultCode.EQUIPMENT_ALREADY_EXISTS;
                //TODO G 将验证结果写入操作日志
                return false;
            }

            return true;

        }
    }

}