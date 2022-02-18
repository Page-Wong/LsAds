using Microsoft.AspNetCore.Http;
using RegistEquipment.Application.RegistEquipmentApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RegistEquipment.Application
{
    public class RegistEquipmentMiddleware
    {
        private readonly RequestDelegate _next;
        public RegistEquipmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IRegistEquipmentAppHandler _registEquipmentAppHandler)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
            await _registEquipmentAppHandler.RegisterApply(context, socket).ConfigureAwait(false);

            await Receive(socket, async (result, text) =>
            {
                 if (result.MessageType == WebSocketMessageType.Close)
                {
                    try
                    {
                        await _registEquipmentAppHandler.OnDisconnected(socket);
                    }
                    catch (WebSocketException)
                    {
                        //TODO G 写入错误日志
                    }
                    return;
                }

            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[1024 * 4]);
                string text = null;
                WebSocketReceiveResult result = null;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        text = await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }

                handleMessage(result, text);
            }
        }
    }
}
