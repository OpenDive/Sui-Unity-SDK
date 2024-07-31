using OpenDive.BCS;

namespace Sui.Utilities
{
    public class SuiError : ErrorBase, ISerializable
    {
        public SuiError(int code, string message, object data) : base(code, message, data) { }

        public SuiError() : base() { }

        public void Serialize(Serialization serializer) => throw new System.NotImplementedException();
    }
}