using Sui.Accounts;
using Sui.Cryptography;

namespace Sui.Cryptography.Ed25519
{
    public class Account : Sui.Accounts.Account
    {
        public override SignatureUtils.SignatureScheme SignatureScheme { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override PrivateKeyBase PrivateKey { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override PublicKeyBase PublicKey { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public override AccountAddress AccountAddress { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override byte[] PrivateKeyShort => throw new System.NotImplementedException();
    }
}