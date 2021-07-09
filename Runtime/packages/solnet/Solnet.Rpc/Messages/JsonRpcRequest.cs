using System.Collections.Generic;

namespace Solnet.Rpc.Messages
{
    [System.Serializable]
    public class JsonRpcRequest : JsonRpcBase
    {
        public string method;// { get; }

        public IList<object> @params;// { get; }

        internal JsonRpcRequest(int id, string method, IList<object> parameters)
        {
            @params = parameters;
            this.method = method;
            base.id = id;
            jsonrpc = "2.0";
        }
    }
}
