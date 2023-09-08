using System;
using Sui.Cryptography;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Accounts
{
    public class Account
    {
        /// <summary>
        /// Signature scheme of the account.
        /// </summary>
        public SignatureScheme SignatureScheme { get; set; }

        /// <summary>
        /// Represents a PrivateKey object.
        /// </summary>
        public PrivateKeyBase PrivateKey { get; set; }

        /// <summary>
        /// Represents a PublicKey object.
        /// </summary>
        public PublicKeyBase PublicKey { get; set; }

        /// <summary>
        /// Represents an AccoutAddress object.
        /// </summary>
        public AccountAddress AccountAddress { get; set; }

        /// <summary>
        /// Private key as 32-byte array
        /// </summary>
        public byte[] PrivateKeyShort { get; }

        public Account(string privateKey, string publicKey, SignatureScheme signatureScheme)
        {
            SignatureScheme = signatureScheme;
        }

        public Account(byte[] privateKey, byte[] publicKey, SignatureScheme signatureScheme)
        {
            SignatureScheme = signatureScheme;
        }

        public Account(byte[] privateKey, SignatureScheme signatureScheme)
        {
            SignatureScheme = signatureScheme;

            if(signatureScheme == SignatureScheme.ED25519)
            {

            }
            else if (signatureScheme == SignatureScheme.Secp256k1)
            {

            }
            else if (signatureScheme == SignatureScheme.Secp256r1)
            {

            }
            else if(signatureScheme == SignatureScheme.MultiSig)
            {

            }
            else if(signatureScheme == SignatureScheme.Zk)
            {

            }
            else
            {

            }
        }

        /// <summary>
        /// Generates an account from a random seed.
        /// </summary>
        /// <param name="signatureScheme"></param>
        private Account(SignatureScheme signatureScheme = SignatureScheme.ED25519)
        {
            SignatureScheme = signatureScheme;

            if (signatureScheme == SignatureScheme.ED25519)
            {
                PrivateKey = Cryptography.Ed25519.PrivateKey.Random();
                PublicKey = PrivateKey.PublicKey();

            }
            else if (signatureScheme == SignatureScheme.Secp256k1)
            {
                throw new NotImplementedException();
            }
            else if (signatureScheme == SignatureScheme.Secp256r1)
            {
                throw new NotImplementedException();
            }
            else if (signatureScheme == SignatureScheme.MultiSig)
            {
                throw new NotImplementedException();
            }
            else if (signatureScheme == SignatureScheme.Zk)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        public static Account Generate(SignatureScheme signatureScheme = SignatureScheme.ED25519)
        {
            return new Account(signatureScheme);
        }

        public bool Verify(byte[] message, SignatureBase signature)
        {
            return PublicKey.Verify(message, signature);
        }

        public SignatureBase Sign(byte[] message)
        {
            return PrivateKey.Sign(message);
        }
    }
}