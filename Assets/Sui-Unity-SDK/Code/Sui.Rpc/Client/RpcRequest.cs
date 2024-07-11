using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sui.Rpc
{
    public class RpcRequest : IRequest
    {
        public string Method { get; }

        public IEnumerable<object> Params { get; }

        public RpcRequest(string method, IEnumerable<object> @params, int id = 1)
        {
            Method = method;
            Params = @params;
            Id = id;
        }
    }
}