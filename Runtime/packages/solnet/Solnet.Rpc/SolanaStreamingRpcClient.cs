using Newtonsoft.Json;
using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UI;

namespace Solnet.Rpc
{
    public class SolanaStreamingRpcClient : StreamingRpcClient, IStreamingRpcClient
    {
        /// <summary>
        /// Message Id generator.
        /// </summary>
        IdGenerator _idGenerator = new IdGenerator();

        Dictionary<int, SubscriptionState> unconfirmedRequests = new Dictionary<int, SubscriptionState>();

        Dictionary<int, SubscriptionState> confirmedSubscriptions = new Dictionary<int, SubscriptionState>();


        public SolanaStreamingRpcClient(string url, IWebSocket websocket = default) : base(url, websocket)
        {
        }

        protected override void HandleNewMessage(ArraySegment<byte> mem)
        {
            //#TODO: remove and add proper logging
            string str = Encoding.UTF8.GetString(mem.Array);
            JsonTextReader reader = new JsonTextReader(new StringReader(str));
            UnityEngine.Debug.Log($"New msg: {str}");

            string prop = "", method = "";
            int id = -1, intResult = -1;
            bool handled = false;
            bool? boolResult = null;

            while (!handled && reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        prop = reader.Value.ToString();
                        if (prop == "params")
                        {
                            HandleDataMessage(str, method);
                            handled = true;
                        }
                        break;
                    case JsonToken.String:
                        if (prop == "method")
                        {
                            method = reader.Value.ToString();
                        }
                        break;
                    case JsonToken.Integer:
                        if (prop == "id")
                        {
                            id = Convert.ToInt32(reader.Value);
                        }
                        else if (prop == "result")
                        {
                            intResult = Convert.ToInt32(reader.Value);
                        }
                        if (id != -1 && intResult != -1)
                        {
                            ConfirmSubscription(id, intResult);
                            handled = true;
                        }
                        break;
                    case JsonToken.Boolean:
                        if (prop == "result")
                        {
                            boolResult = (bool)reader.Value;
                        }
                        break;
                }
            }

            if (boolResult.HasValue)
            {
                RemoveSubscription(id, boolResult.Value);
            }
        }

        private void RemoveSubscription(int id, bool value)
        {
            SubscriptionState sub;

            if (!confirmedSubscriptions.Remove(id))
            {
                // houston, we might have a problem?
            }

            if (value)
            {
                //sub?.ChangeState(SubscriptionStatus.Unsubscribed);
            }
            else
            {
                //sub?.ChangeState(sub.State, "Subscription doesnt exists");
            }
        }

        #region SubscriptionMapHandling

        private void ConfirmSubscription(int internalId, int resultId)
        {
            SubscriptionState sub;
            sub = unconfirmedRequests[internalId];
            if (unconfirmedRequests.Remove(internalId))
            {
                sub.SubscriptionId = resultId;
                confirmedSubscriptions.Add(resultId, sub);
            }

            sub?.ChangeState(SubscriptionStatus.Subscribed);
        }

        private void AddSubscription(SubscriptionState subscription, int internalId)
        {
            unconfirmedRequests.Add(internalId, subscription);
        }

        private SubscriptionState RetrieveSubscription(int subscriptionId)
        {
            UnityEngine.Debug.Log(confirmedSubscriptions.ContainsKey(subscriptionId));
            var sub = confirmedSubscriptions[subscriptionId];
            return sub;
        }
        #endregion

        private void HandleDataMessage(string reader, string method)
        {
            JsonSerializerSettings opts = new JsonSerializerSettings() { MaxDepth = 64 };
            UnityEngine.Debug.Log(reader);
            switch (method)
            {
                case "accountNotification":
                    var accNotification = JsonConvert.DeserializeObject<JsonRpcWrapResponse<ResponseValue<AccountInfo>>>(reader, opts);
                    if (accNotification == null) break;

                    NotifyData(accNotification.@params.subscription, accNotification.@params.result.Value.Value);
                    break;
                case "logsNotification":
                    var logsNotification = JsonConvert.DeserializeObject<JsonRpcStreamResponse<ResponseValue<LogInfo>>>(reader);
                    if (logsNotification == null) break;
                    NotifyData(logsNotification.subscription, logsNotification.result);
                    break;
                case "programNotification":
                    var programNotification = JsonConvert.DeserializeObject<JsonRpcStreamResponse<ResponseValue<ProgramInfo>>>(reader);
                    if (programNotification == null) break;
                    NotifyData(programNotification.subscription, programNotification.result);
                    break;
                case "signatureNotification":
                    var signatureNotification = JsonConvert.DeserializeObject<JsonRpcStreamResponse<ErrorResult>>(reader);
                    if (signatureNotification == null) break;
                    NotifyData(signatureNotification.subscription, signatureNotification.result);
                    // remove subscription from map
                    break;
                case "slotNotification":
                    var slotNotification = JsonConvert.DeserializeObject<JsonRpcStreamResponse<SlotInfo>>(reader);
                    if (slotNotification == null) break;
                    NotifyData(slotNotification.subscription, slotNotification.result);
                    break;
                case "rootNotification":
                    var rootNotification = JsonConvert.DeserializeObject<JsonRpcStreamResponse<int>>(reader);
                    if (rootNotification == null) break;
                    NotifyData(rootNotification.subscription, rootNotification.result);
                    break;
            }
        }

        private void NotifyData(int subscription, object data)
        {
            var sub = RetrieveSubscription(subscription);
            sub.HandleData(data);
        }

        #region AccountInfo
        public async Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)

        {
            var sub = new SubscriptionState<ResponseValue<AccountInfo>>(this, SubscriptionChannel.Account, callback, new List<object> { pubkey });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "accountSubscribe", new List<object> { pubkey, new Dictionary<string, string> { { "encoding", "base64" }, { "commitment", "finalized" } } });

            return await Subscribe(sub, msg).ConfigureAwait(false);
        }

        public SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback)
        {
            return SubscribeAccountInfoAsync(pubkey, callback).Result;
        }
        #endregion

        #region Logs
        public async Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, new List<object> { pubkey });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", new List<object> { new Dictionary<string, object> { { "mentions", new List<string> { pubkey } } } });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
            => SubscribeLogInfoAsync(pubkey, callback).Result;

        public async Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<LogInfo>>(this, SubscriptionChannel.Logs, callback, new List<object> { subscriptionType });


            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "logsSubscribe", new List<object> { subscriptionType });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback)
            => SubscribeLogInfoAsync(subscriptionType, callback).Result;
        #endregion

        #region Signature
        public async Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<ErrorResult>>(this, SubscriptionChannel.Signature, callback, new List<object> { transactionSignature });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "signatureSubscribe", new List<object> { transactionSignature });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeSignature(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback)
            => SubscribeSignatureAsync(transactionSignature, callback).Result;
        #endregion

        #region Program
        public async Task<SubscriptionState> SubscribeProgramAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ProgramInfo>> callback)
        {
            var sub = new SubscriptionState<ResponseValue<ProgramInfo>>(this, SubscriptionChannel.Program, callback,
                new List<object> { transactionSignature/*, new Dictionary<string, string> { { "encoding", "base64" } }*/ });

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "programSubscribe", new List<object> { transactionSignature });
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeProgram(string transactionSignature, Action<SubscriptionState, ResponseValue<ProgramInfo>> callback)
            => SubscribeProgramAsync(transactionSignature, callback).Result;
        #endregion

        #region SlotInfo

        public async Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback)
        {
            var sub = new SubscriptionState<SlotInfo>(this, SubscriptionChannel.Slot, callback);

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "slotSubscribe", null);
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback)
            => SubscribeSlotInfoAsync(callback).Result;
        #endregion


        #region Root
        public async Task<SubscriptionState> SubscribeRootAsync(Action<SubscriptionState, int> callback)
        {
            var sub = new SubscriptionState<int>(this, SubscriptionChannel.Root, callback);

            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), "rootSubscribe", null);
            return await Subscribe(sub, msg).ConfigureAwait(false);
        }
        public SubscriptionState SubscribeRoot(Action<SubscriptionState, int> callback)
            => SubscribeRootAsync(callback).Result;
        #endregion

        private async Task<SubscriptionState> Subscribe(SubscriptionState sub, JsonRpcRequest msg)
        {
            string json = JsonConvert.SerializeObject(msg);
            var bytes = Encoding.UTF8.GetBytes(json);

            List<byte> mem = new List<byte>(bytes);
            await ClientSocket.SendAsync(mem, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);

            AddSubscription(sub, msg.id);
            return sub;
        }



        private string GetUnsubscribeMethodName(SubscriptionChannel channel) {
            switch (channel)
            {
                case SubscriptionChannel.Account:
                    return "accountUnsubscribe";
                case SubscriptionChannel.Logs:
                    return "logsUnsubscribe";
                case SubscriptionChannel.Program:
                    return "programUnsubscribe";
                case SubscriptionChannel.Root:
                    return "rootUnsubscribe";
                case SubscriptionChannel.Signature:
                    return "signatureUnsubscribe";
                case SubscriptionChannel.Slot:
                    return "slotUnsubscribe";
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel), channel, "invalid message type");
            };
        }

        public async Task UnsubscribeAsync(SubscriptionState subscription)
        {
            var msg = new JsonRpcRequest(_idGenerator.GetNextId(), GetUnsubscribeMethodName(subscription.Channel), new List<object> { subscription.SubscriptionId });

            await Subscribe(subscription, msg).ConfigureAwait(false);
        }

        public void Unsubscribe(SubscriptionState subscription) => UnsubscribeAsync(subscription).Wait();
    }
}