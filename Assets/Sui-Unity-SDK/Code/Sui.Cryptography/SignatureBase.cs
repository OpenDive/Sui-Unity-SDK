using System.Collections.Generic;

namespace Sui.Cryptography
{
    public abstract class SignatureBase
    {
        public const int SignatureLength = 64;
        public abstract byte[] Data();
        public abstract string Serialize();
        public abstract SignatureBase Deserialize(string serializedSignature);
    }
}