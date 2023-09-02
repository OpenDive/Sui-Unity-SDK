using System.Collections.Generic;

namespace Sui.Cryptography
{
    public interface ISignature
    {
        public const int SignatureLength = 64;
        public byte[] Data();
        public string Serialize();
        public ISignature Deserialize(string serializedSignature);
    }
}