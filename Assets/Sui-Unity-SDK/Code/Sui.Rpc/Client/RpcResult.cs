using Sui.Rpc.Client;

namespace Sui.Rpc
{
    public class RpcResult<T> : RpcObjectBase
    {
        public T Result { get; set; }
        public RpcError Error { get; set; }

        public RpcResult(T result, RpcError error = null)
        {
            this.Result = result;
            this.Error = error;
        }

        public static RpcResult<T> GetErrorResult(string message)
        {
            return new RpcResult<T>(default, new RpcError(-1, message, null));
        }
    }
}