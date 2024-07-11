using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sui.Rpc.Client
{
    public abstract class RpcObjectBase
    {
        public string Jsonrpc { get => "2.0"; }

        public int Id { get; set; }
    }
}