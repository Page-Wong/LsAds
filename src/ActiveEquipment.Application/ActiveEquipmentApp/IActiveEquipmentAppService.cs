using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Net.WebSockets;

namespace ActiveEquipment.Application.ActiveEquipmentApp {
    public interface IActiveEquipmentAppService : IBaseCasheAppService<Guid, ActiveEquipmentDto> {

        bool IsCanActive(ActiveEquipmentDto item);
        ActiveEquipmentDto GetBySocket(WebSocket socket);
        ActiveEquipmentDto GetByToken(string token);
    }

}