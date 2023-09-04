using Chaos.NaCl;

namespace Sui.Cryptography.Ed25519
{
    public class Signature : SignatureBase
    {
        public Signature(byte[] signature)
        {
            _signatureBytes = signature;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            string signatureHex = CryptoBytes.ToHexStringLower(_signatureBytes);
            SignatureHex = "0x" + signatureHex;

            return SignatureHex;
        }

        public override bool Equals(object obj)
        {
            if (obj is Signature signature)
            {
                return signature.ToString() == this.ToString();
            }

            return false;
        }


        public override string Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override SignatureBase Deserialize(string serializedSignature)
        {
            throw new System.NotImplementedException();
        }
    }
}