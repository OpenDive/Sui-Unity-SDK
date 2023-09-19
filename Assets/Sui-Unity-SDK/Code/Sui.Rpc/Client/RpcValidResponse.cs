namespace Sui.Rpc.Client
{
    public class RpcValidResponse<T> : RpcObjectBase
    {
        public T Result { get; set; }
    }
}