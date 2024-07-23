namespace Sui.Rpc.Client
{
    public class RpcValidResponse<T>
    {
        public string Jsonrpc { get => "2.0"; }

        public int Id { get; set; }

        public T Result { get; set; }

        public RpcError Error { get; set; }
    }
}