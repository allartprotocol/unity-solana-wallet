using dotnetstandard_bip32;
using Merkator.BitCoin;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    public abstract class StreamingRpcClient
    {
        protected readonly IWebSocket ClientSocket;

        private readonly string _socketUri;

        protected StreamingRpcClient(string nodeUri, IWebSocket socket = default)
        {
            ClientSocket = socket ?? new WebSocketWrapper(new ClientWebSocket());
            _socketUri = nodeUri;
        }

        public async Task Init()
        {
            await ClientSocket.ConnectAsync(new Uri(_socketUri), CancellationToken.None).ConfigureAwait(false);
            _ = Task.Run(StartListening);
        }

        private async Task StartListening()
        {
            while (ClientSocket.State == WebSocketState.Open)
            {
                try
                {
                    await ReadNextMessage().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception caught: {e.Message}");
                }
            }
        }

        private async Task ReadNextMessage(CancellationToken cancellationToken = default)
        {
            var buffer = new byte[32768];
            ArraySegment<byte> mem = new ArraySegment<byte>(buffer);
            var messageParts = new StringBuilder();
            int count = 0;

            WebSocketReceiveResult result = await ClientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
            count = result.Count;
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ClientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            }
            else
            {
                if (!result.EndOfMessage)
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Write(mem.ToArray(), 0, count);


                    while (!result.EndOfMessage)
                    {
                        result = await ClientSocket.ReceiveAsync(mem, cancellationToken).ConfigureAwait(false);
                        UnityEngine.Debug.Log(Encoding.UTF8.GetString(mem.ToArray()));
                        ms.Write(mem.ToArray(), 0, (count));
                        count += result.Count;
                    }

                    mem = new ArraySegment<byte>(ms.ToArray());
                }
                else
                {
                    mem = new ArraySegment<byte>(mem.Array, 0 ,count);
                }

                HandleNewMessage(mem);
            }
        }

        protected abstract void HandleNewMessage(ArraySegment<byte> mem);

        
    }
}
