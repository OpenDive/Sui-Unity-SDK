
using System;
using Sui.Cryptography;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Accounts
{
    public class AccountFactory
    {
        public static Account CreateAccount(PrivateKeyBase privateKey)
        {
            throw new NotImplementedException();
        }

        public static Account CreateAccount(SignatureScheme signatureScheme = SignatureScheme.ED25519)
        {
            throw new NotImplementedException();
        }

        public static Account CreateAccount(byte[] privateKey, SignatureScheme signatureScheme = SignatureScheme.ED25519)
        {
            throw new NotImplementedException();
        }
    }
}