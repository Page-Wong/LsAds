using ActiveEquipment.Application.ActiveEquipmentApp;
using LsAdmin.Utility.Services.WebSocketService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveEquipment.Application {
    public class ActiveEquipmentMiddleware {
        private readonly RequestDelegate _next;
        //private IActiveEquipmentAppHandler _ativeEquipmentAppHandler { get; set; }

        public ActiveEquipmentMiddleware(RequestDelegate next/*, IActiveEquipmentAppHandler ativeEquipmentAppHandler*/) {
            _next = next;
            //_ativeEquipmentAppHandler = ativeEquipmentAppHandler;
        }

        public async Task Invoke(HttpContext context, IActiveEquipmentAppHandler _ativeEquipmentAppHandler) {
            if (!context.WebSockets.IsWebSocketRequest) {
                await _next.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
            await _ativeEquipmentAppHandler.OnConnected(context, socket).ConfigureAwait(false);

            await Receive(socket, async (result, text) => {
                if (result.MessageType == WebSocketMessageType.Text) {
                    await _ativeEquipmentAppHandler.ReceiveAsync(socket, result, text).ConfigureAwait(false);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Close) {
                    try {
                        await _ativeEquipmentAppHandler.OnDisconnected(socket);
                    }
                    catch (WebSocketException) {
                        //TODO G 写入错误日志
                    }

                    return;
                }

            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage) {
            while (socket.State == WebSocketState.Open) {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                string text = null;
                WebSocketReceiveResult result = null;
                using (var ms = new MemoryStream()) {
                    do {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(ms, Encoding.UTF8)) {
                        text = await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }

                handleMessage(result, text);
            }
        }
    }

}