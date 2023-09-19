namespace Sui.Rpc.Client
{
    public class RpcError
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}