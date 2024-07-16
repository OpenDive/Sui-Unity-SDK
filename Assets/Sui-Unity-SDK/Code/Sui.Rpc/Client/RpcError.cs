using OpenDive.BCS;

namespace Sui.Rpc.Client
{
    public abstract class ErrorBase
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public ErrorBase(int code, string message, object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }

    public class RpcError : ErrorBase
    {
        public RpcError(int code, string message, object data) : base(code, message, data) { }
    }

    public class SuiError : ErrorBase, ISerializable
    {
        public SuiError(int code, string message, object data) : base(code, message, data) { }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}