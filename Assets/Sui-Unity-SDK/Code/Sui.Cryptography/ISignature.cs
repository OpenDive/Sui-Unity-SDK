using System.Collections.Generic;

namespace Sui.Cryptography
{
    public interface ISignature
    {
        public string Serialize();
        public ISignature Deserialize(string serializedSignature);

        public static string KeyHex
        {
            get
            {
                return "";
            }

            set
            {
            }
        }
    }
}