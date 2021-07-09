using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    public abstract class SubscriptionState
    {
        private readonly SolanaStreamingRpcClient _rpcClient;
        internal int SubscriptionId { get; set; }
        public SubscriptionChannel Channel { get; }

        public SubscriptionStatus State { get; private set; }

        private List<object> AdditionalParameters { get; }

        public event EventHandler<SubscriptionEvent> SubscriptionChanged;

        internal SubscriptionState(SolanaStreamingRpcClient rpcClient, SubscriptionChannel chan, IList<object> aditionalParameters = default)
        {
            _rpcClient = rpcClient;
            Channel = chan;
            AdditionalParameters = aditionalParameters?.ToList();
        }

        internal void ChangeState(SubscriptionStatus newState, string error = null, string code = null)
        {
            State = newState;
            SubscriptionChanged?.Invoke(this, new SubscriptionEvent(newState, error, code));
        }

        public abstract void HandleData(object data);

        public void Unsubscribe() => _rpcClient.Unsubscribe(this);
        public async Task UnsubscribeAsync() => await _rpcClient.UnsubscribeAsync(this).ConfigureAwait(false);
    }

    public class SubscriptionState<T> : SubscriptionState
    {
        public Action<SubscriptionState<T>, T> DataHandler;

        public SubscriptionState(SolanaStreamingRpcClient rpcClient, SubscriptionChannel chan, Action<SubscriptionState, T> handler, IList<object> aditionalParameters = default)
            : base(rpcClient, chan, aditionalParameters)
        {
            DataHandler = handler;
        }

        public override void HandleData(object data) {
            try
            {
                UnityEngine.Debug.Log(data.GetType());

                DataHandler.Invoke(this, (T)data);
            }
            catch (Exception e)
            {
                
                throw new Exception(e.Message);
            }
        }
    }
}
