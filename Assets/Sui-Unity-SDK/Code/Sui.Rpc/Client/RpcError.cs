using System;
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

        protected ErrorBase() { }
    }

    public class RpcError : ErrorBase
    {
        public RpcError(int code, string message, object data) : base(code, message, data) { }

        public RpcError() : base() { }
    }

    public class SuiError : ErrorBase, ISerializable
    {
        public SuiError(int code, string message, object data) : base(code, message, data) { }

        public SuiError(): base() { }

        public void Serialize(Serialization serializer) => throw new System.NotImplementedException();
    }

    public abstract class ReturnBase
    {
        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; protected internal set; }

        protected internal T SetError<T, U>(T item, string message, object data = null) where U : ErrorBase, new()
        {
            this.Error = new U();
            this.Error.Code = 0;
            this.Error.Message = message;
            this.Error.Data = data;

            return item;
        }

        protected internal void SetError<T>(string message, object data = null) where T : ErrorBase, new()
        {
            this.Error = new T();
            this.Error.Code = 0;
            this.Error.Message = message;
            this.Error.Data = data;
        }
    }
}