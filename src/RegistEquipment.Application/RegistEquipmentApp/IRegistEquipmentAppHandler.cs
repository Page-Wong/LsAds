using LsAdmin.Application.RegistEquipmentApp.Dto;
using LsAdmin.Utility.Service.WebSocketService.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace RegistEquipment.Application.RegistEquipmentApp {
    public interface IRegistEquipmentAppHandler
    {
        Task OnDisconnected(WebSocket socket);
        Task SendMessageAsync(RegistEquipmentDto item, Message message);
        Task SendRegisterSucceedAsync(Guid id);

        /// <summary>
        /// 注册申请
        /// </summary>
        /// <param name="context"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        Task RegisterApply(HttpContext context, WebSocket socket);
        Task ReceiveRegisterSucceedAsync(WebSocket socket);

        Task DeleteOverTimeWebSocketsAsync(DateTime? time);
    }
}
