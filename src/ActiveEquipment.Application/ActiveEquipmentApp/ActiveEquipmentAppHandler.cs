using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.EquipmentApp;
using ActiveEquipment.Application.Common;
using ActiveEquipment.Application.DataModel;
using System.Web;
using LsAdmin.Application.InstructionLogApp;
using LsAdmin.Application.InstructionApp;
using LsAdmin.Application.InstructionApp.Dto;
using LsAdmin.Application.InstructionLogApp.Dto;
using LsAdmin.Utility.Json;
using LsAdmin.Application.EquipmentApp.Dtos;
using System.Linq;

namespace ActiveEquipment.Application.ActiveEquipmentApp {
    public class ActiveEquipmentAppHandler : IActiveEquipmentAppHandler {

        IEquipmentAppService _equipmentAppService;
        IActiveEquipmentAppService _activeEquipmentAppService;
        IInstructionLogAppService _instructionLogAppService;
        IInstructionAppService _instructionAppService;
        public ActiveEquipmentAppHandler(IEquipmentAppService equipmentAppService, IActiveEquipmentAppService activeEquipmentAppService, IInstructionLogAppService instructionLogAppService, IInstructionAppService instructionAppService) : base() {
            _activeEquipmentAppService = activeEquipmentAppService;
            _instructionLogAppService = instructionLogAppService;
            _instructionAppService = instructionAppService;
            _equipmentAppService = equipmentAppService;
        }

        public async Task OnConnected(HttpContext context, WebSocket socket) {
            if (context.Request.Query.ContainsKey("deviceId")) {

                string deviceId = HttpUtility.UrlDecode(context.Request.Query["deviceId"], Encoding.UTF8);

                var ip = context.Connection.RemoteIpAddress;
                var port = context.Connection.RemotePort;
                var token = Guid.NewGuid().ToString();

                #region 记录连接中日志
                LogHelper.Log(new ActiveEquipmentLog {
                    Token = token,
                    Type = ActiveEquipmentLog.ActiveEquipmentLogType.Connecting,
                    Ip = ip.ToString(),
                    Port = port,
                    Message = deviceId
                });
                #endregion

                var equipment = _equipmentAppService.GetByDeviceId(deviceId);
                var item = new ActiveEquipmentDto {
                    Ip = ip.ToString(),
                    Port = port,
                    Token = token,
                    WebSocket = socket,
                    DeviceId = equipment?.DeviceId,
                    Equipment = equipment
                };
                if (_activeEquipmentAppService.IsCanActive(item)) {
                    item.OnlineTime = DateTime.Now;
                    item.LastConnectTime = DateTime.Now;
                    if (_activeEquipmentAppService.InsertOrUpdate(item)) {
                        #region 记录已连接日志
                        LogHelper.Log(new ActiveEquipmentLog {
                            Token = item.Token,
                            Type = ActiveEquipmentLog.ActiveEquipmentLogType.Connected,
                            Ip = ip.ToString(),
                            Port = port,
                            Message = deviceId,
                            EquipmentId = item.EquipmentId
                        });
                        #endregion

                        //发送连接令牌到设备
                        await SendMessageAsync(item, new Message { MessageType = MessageType.ConnectionEvent, Data = item.Token });
                    }
                    return;
                }
                #region 记录连接失败日志
                LogHelper.Log(new ActiveEquipmentLog {
                    Token = item.Token,
                    Type = ActiveEquipmentLog.ActiveEquipmentLogType.ConnecteFail,
                    Ip = ip.ToString(),
                    Port = port,
                    Message = deviceId,
                    EquipmentId = item.EquipmentId
                });
                #endregion
            }
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

        }

        public async Task OnDisconnected(WebSocket socket) {

            var item = _activeEquipmentAppService.GetBySocket(socket);
            if (item != null) {
                _activeEquipmentAppService.Delete(item.EquipmentId);

                #region 记录断开连接日志
                LogHelper.Log(new ActiveEquipmentLog {
                    Token = item.Token,
                    Type = ActiveEquipmentLog.ActiveEquipmentLogType.Disconnected,
                    Ip = item.Ip,
                    Port = item.Port,
                    EquipmentId = item.EquipmentId
                });
                #endregion
            }
            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                        statusDescription: "Closed by the WebSocketManager",
                                        cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, string text) {
            await Task.Run(() => {
                var item = _activeEquipmentAppService.GetBySocket(socket);
                if (item != null) {

                    #region 记录接收心跳日志
                    /*LogHelper.Log(new ActiveEquipmentLog {
                        Token = item.Token,
                        Type = ActiveEquipmentLog.ActiveEquipmentLogType.Received,
                        Ip = item.Ip,
                        Port = item.Port,
                        EquipmentId = item.EquipmentId,
                        DeviceId = item.DeviceId,
                        Message = text
                    });*/
                    #endregion

                    item.LastConnectTime = DateTime.Now;
                    _activeEquipmentAppService.Update(item);
                }
            });
        }

        public async Task<Result> SendMessageAsync(ActiveEquipmentDto item, Message message) {
            //TODO G 写入指令状态处理日志
            if (item?.WebSocket == null)
                return await Task.Run(() => new Result { Code = Result.ResultCode.ACTIVE_EQUIPMENT_NONE });
            if (item.WebSocket.State != WebSocketState.Open)
                return await Task.Run(() => new Result { Code = Result.ResultCode.ACTIVE_EQUIPMENT_WEBSOCKET_CLOSE });

            var serializedMessage = JsonConvert.SerializeObject(message, new LsJsonSerializerSettings());
            var encodedMessage = Encoding.UTF8.GetBytes(serializedMessage);

            #region 记录websocket信息发送日志
            LogHelper.Log(new ActiveEquipmentLog {
                Token = item.Token,
                Type = ActiveEquipmentLog.ActiveEquipmentLogType.Sended,
                Ip = item.Ip,
                Port = item.Port,
                EquipmentId = item.EquipmentId,
                Message = serializedMessage
            });
            #endregion
            await item.WebSocket.SendAsync(buffer: new ArraySegment<byte>(array: encodedMessage,
                                                                    offset: 0,
                                                                    count: encodedMessage.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None).ConfigureAwait(false);
            return await Task.Run(() => new Result { Code = Result.ResultCode.SUCCESS });
        }

        public async Task<Result> SendInstructionAsync(ActiveEquipmentDto activeEquipment, InstructionDto instruction, string memo = "") {
            try {
                memo = memo != "" ? String.Format("[{0}]", memo) : "";
                var result = new Result();
                #region 发送指令到设备端
                var instructionMessage = new InstructionMessage {
                    InstructionId = instruction.Id.ToString(),
                    NotifyUrl = instruction.NotifyUrl,
                    Key = instruction.MethodId.ToString(),
                    Timestamp = instruction.Timestamp,
                    Params = instruction.ParamsMap,
                    Content = instruction.ContentMap
                };
                instructionMessage.Sign = SignHelper.Sign(instructionMessage, activeEquipment?.Token, activeEquipment.DeviceId);
                var serializedMessage = JsonConvert.SerializeObject(instructionMessage, new LsJsonSerializerSettings());

                var message = new Message {
                    MessageType = MessageType.ClientMethodInvocation,
                    Data = serializedMessage
                };
                result = await SendMessageAsync(activeEquipment, message);
                #endregion
                if (result.IsSuccess()) {

                    #region 发送完成后更新指令状态
                    instruction.Sign = instructionMessage.Sign;
                    instruction.Status = InstructionStatus.Send;
                    _instructionAppService.Update(instruction);
                    #endregion

                    #region 发送完成后记录指令日志
                    var log = new InstructionLogDto {
                        InstructionId = instruction.Id,
                        EquipmentId = instruction.EquipmentId,
                        Type = InstructionLogType.Send,
                        InstructionStatus = instruction.Status,
                        Remarks = String.Format("{0}发送成功", memo)
                    };
                    _instructionLogAppService.Insert(ref log);
                    #endregion
                }
                return await Task.Run(() => result);
            }
            catch (Exception e) {
                #region 记录指令发送错误日志
                var errorLog = new InstructionLogDto {
                    InstructionId = instruction.Id,
                    EquipmentId = instruction.EquipmentId,
                    Type = InstructionLogType.Send,
                    InstructionStatus = instruction.Status,
                    Remarks = String.Format("{0}发送失败，失败原因：{1}", memo, e.Message)
                };
                _instructionLogAppService.Insert(ref errorLog);
                #endregion
                return await Task.Run(() => new Result { Code = Result.ResultCode.SEND_INSTRUCTION_ERROR, Exception = e });
            }
        }

        public async Task<Result> SendInstructionAsync(Guid equipmentId, InstructionDto instruction) {
            var item = _activeEquipmentAppService.Get(equipmentId);
            return await SendInstructionAsync(item, instruction);
        }

    }

}