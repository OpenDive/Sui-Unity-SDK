namespace Sui.Rpc.Client
{
    public class RpcErrorResponse
    {
        public string Jsonrpc { get => "2.0"; }

        public int Id { get; set; }

        public RpcError Error { get; set; }
    }
}