using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using LsAdmin.Utility.Service.WebSocketService.Common;
using Newtonsoft.Json;
using RegistEquipment.Application.DataModel;
using System.Collections.Concurrent;
using LsAdmin.Application.EquipmentApp;
using Microsoft.Extensions.Options;
using System.Linq;
using LsAdmin.Utility.Json;
using LsAdmin.Application.RegistEquipmentApp.Dto;

namespace RegistEquipment.Application.RegistEquipmentApp {
    public class RegistEquipmentAppHandler : IRegistEquipmentAppHandler
    {
        IRegistEquipmentAppService _registEquipmentAppService;
         IEquipmentAppService _equipmentService;
         RegistrationUrlOptions _registrationUrlOptions;

        public RegistEquipmentAppHandler(IRegistEquipmentAppService registEquipmentAppService,IEquipmentAppService equipmentService, IOptions<RegistrationUrlOptions> registrationUrlOptions):base()
        {
            _registEquipmentAppService = registEquipmentAppService;
            _equipmentService = equipmentService;
            _registrationUrlOptions = registrationUrlOptions.Value;
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            //写入连接日志
            var item = _registEquipmentAppService.GetByWebSocket(socket);
            if (item != null)
            {
                _registEquipmentAppService.Delete(item.Id);
            }
            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                        statusDescription: "Closed by the WebSocketManager",
                                        cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        public async Task RegisterApply(HttpContext context, WebSocket socket)
        {

            
           
            if (context.Request.Query.Count > 0)
            {
                //string urlString = _registrationUrlOptions.RegistrationUrl;

                var registMessage = new RegistMessage
                {
                    methodName = "RegistrationUrl",
                    arguments = new ConcurrentDictionary<string, string>(),
                };

                Microsoft.Extensions.Primitives.StringValues deviceId;
                context.Request.Query.TryGetValue("deviceId", out deviceId);
                var decrypDeviceId = deviceId;

               /* System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                byte[] key = Convert.FromBase64String(_registrationUrlOptions.AesKey);
                byte[] iv = new byte[8];      //当模式为ECB时，IV无用 
                var decrypDeviceId = Encoding.UTF8.GetString(Des3Helper.Des3DecodeECB(key, iv, Convert.FromBase64String(deviceId))).Replace("\0","");
                */

                var ip = context.Connection.RemoteIpAddress;
                var port = context.Connection.RemotePort;
                var token = Guid.NewGuid().ToString();

                var item = new RegistEquipmentDto
                {
                    DeviceId = decrypDeviceId,
                    ApplyTime = DateTime.Now,
                    WebSocket = socket,
                    Token = token,
                    Status=0,
                    Id = Guid.NewGuid(),
                };

                string errormessage = "";

                /*if (_equipmentService.GetByDeviceId(deviceId) != null)
                {
                    registMessage.arguments.TryAdd("result", "Faild");
                    registMessage.arguments.TryAdd("message", string.Format("注册出错：{0}", "设备已注册，不能重新注册！"));
                    registMessage.arguments.TryAdd("url", "");

                }else*/
                if (!_registEquipmentAppService.Insert(ref item, out errormessage)) {
                    registMessage.arguments.TryAdd("result", "Faild");
                    registMessage.arguments.TryAdd("message", string.Format("注册出错：{0}", errormessage));
                    registMessage.arguments.TryAdd("url", "");
                }
                else {
                    registMessage.arguments.TryAdd("result", "Success");
                    registMessage.arguments.TryAdd("message", "");
                    registMessage.arguments.TryAdd("url", string.Format( "{0}", item.Id));
                }

                //发送注册指令，包含设备端需显示的二维码信息
                await SendMessageAsync(item, new Message
                {
                    MessageType = MessageType.ClientMethodInvocation,
                    Data = JsonConvert.SerializeObject(registMessage, new LsJsonSerializerSettings()),
                }).ConfigureAwait(false);
            }
            else { 
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }

        public async Task SendRegisterSucceedAsync(Guid id)
        {
            var registMessage = new RegistMessage
            {
                methodName = "RegistrationResult",
                arguments = new ConcurrentDictionary<string, string>(),
            };
            //{ messageType: 1,data: { methodName: "RegistrationResult",arguments:[{ result: "Success",message: "aaa",deviceId: "abcd1234"}]} }
            var dto = _registEquipmentAppService.Get(id);

            registMessage.arguments.TryAdd("result", "Success");
            registMessage.arguments.TryAdd("message", "");
            registMessage.arguments.TryAdd("deviceId", dto.DeviceId.ToString());

            await SendMessageAsync(dto, new Message
            {
                MessageType = MessageType.ClientMethodInvocation,
                Data = JsonConvert.SerializeObject(registMessage, new LsJsonSerializerSettings()),
            }).ConfigureAwait(false);
            await dto.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public async Task SendMessageAsync(RegistEquipmentDto item, Message message)
        {
            if (item.WebSocket.State != WebSocketState.Open)
                return;
            var serializedMessage = JsonConvert.SerializeObject(message, new LsJsonSerializerSettings());
            var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);
            await item.WebSocket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                                                                    offset: 0,
                                                                    count: encodedMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        public async Task ReceiveRegisterSucceedAsync(WebSocket socket)
        {
            await OnDisconnected(socket);
        }

        public async Task DeleteOverTimeWebSocketsAsync(DateTime? time)
        {
            try { 
            time = time != null ? time : DateTime.Now;

            foreach (var RegistEquipments in _registEquipmentAppService.GetAllList().Where(w => w.AeadTime <= time).ToList())
            {
                await OnDisconnected(RegistEquipments.WebSocket);
                    _registEquipmentAppService.Delete(RegistEquipments.Id);
            }

            }catch(Exception ex)
            {
                throw new NotImplementedException();
            }
            
        }
    }
}
