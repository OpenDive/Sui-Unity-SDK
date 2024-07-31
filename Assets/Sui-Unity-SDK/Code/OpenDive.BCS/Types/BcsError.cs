using Sui.Utilities;

namespace OpenDive.BCS
{
    /// <summary>
    /// Represents an error derived from the BCS utility module.
    /// </summary>
    public class BcsError : ErrorBase, ISerializable
    {
        public BcsError(int code, string message, object data) : base(code, message, data) { }

        public BcsError() : base() { }

        public void Serialize(Serialization serializer) => throw new System.NotImplementedException();
    }
}
