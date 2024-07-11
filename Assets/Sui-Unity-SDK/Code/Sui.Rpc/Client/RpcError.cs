namespace Sui.Rpc.Client
{
    public class RpcError
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public RpcError(int code, string message, object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }
}