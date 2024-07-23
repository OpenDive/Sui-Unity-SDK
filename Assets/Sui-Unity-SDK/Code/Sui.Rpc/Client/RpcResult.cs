using Sui.Rpc.Client;
using Sui.Utilities;

namespace Sui.Rpc
{
    public class RpcResult<T> : ResultBase<T>
    {
        public string Jsonrpc { get => "2.0"; }

        public int Id { get; set; }

        public RpcResult(T result, RpcError error = null) : base(result, error) { }

        public static RpcResult<T> GetErrorResult(string message)
            => new RpcResult<T>(default, new RpcError(-1, message, null));
    }
}