using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Messages
{
    public class JsonRpcStreamResponse <T>
    {
        public ResponseValue<T> result { get; set; }

        public int subscription { get; set; }
    }

    public class JsonRpcWrapResponse<T>
    {
        public JsonRpcStreamResponse<T> @params { get; set; }

    }
}
