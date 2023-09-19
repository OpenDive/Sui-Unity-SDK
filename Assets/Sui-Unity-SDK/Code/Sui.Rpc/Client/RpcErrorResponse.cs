namespace Sui.Rpc.Client
{
    public class RpcErrorResponse : RpcObjectBase
    {
        public RpcError Error { get; set; }
    }
}