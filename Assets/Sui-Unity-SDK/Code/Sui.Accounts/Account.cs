using Sui.Cryptography;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Accounts
{
    public class Account
    {
        public SignatureScheme SignatureScheme { get; set; }
        public PrivateKeyBase PrivateKey { get; set; }
        public PublicKeyBase PublicKey { get; set; }
        public AccountAddress AccountAddress { get; set; }
    }
}