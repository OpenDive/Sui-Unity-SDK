using Chaos.NaCl;

namespace Sui.Cryptography.Ed25519
{
    public class Signature : SignatureBase
    {
        private readonly byte[] _signatureBytes;

        private string _signature;

        //public byte[] SignatureBytes
        //{
        //    get
        //    {
        //        return "";
        //    }

        //    set
        //    {
        //    }
        //}

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
            _signature = "0x" + signatureHex;

            return _signature;
        }

        public override bool Equals(object obj)
        {
            if (obj is Signature signature)
            {
                return signature.ToString() == this.ToString();
            }

            return false;
        }

        public override byte[] Data()
        {
            throw new System.NotImplementedException();
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