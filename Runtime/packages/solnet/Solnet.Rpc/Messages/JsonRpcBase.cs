namespace Solnet.Rpc.Messages
{
    public class JsonRpcBase
    {
        public string jsonrpc { get; protected set; }

        public int id { get; set; }
    }
}
