using ActiveEquipment.Application.DataModel;
using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.InstructionApp.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ActiveEquipment.Application.ActiveEquipmentApp {
    public interface IActiveEquipmentAppHandler {

        Task OnConnected(HttpContext context, WebSocket socket);
        Task OnDisconnected(WebSocket socket);
        Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string text);
        Task<Result> SendMessageAsync(ActiveEquipmentDto item, Message message);
        Task<Result> SendInstructionAsync(ActiveEquipmentDto item, InstructionDto instruction, string memo = "");
        Task<Result> SendInstructionAsync(Guid equipmentId, InstructionDto instruction);

    }

}