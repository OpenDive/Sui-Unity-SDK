using System;
using Chaos.NaCl;
using Org.BouncyCastle.Crypto.Digests;
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

        public AccountAddress SuiAddress()
        {
            return PublicKey.ToSuiAddress();
        }

        /// <summary>
        /// Sign messages with a specific intent. By combining
        /// the message bytes with the intent before hashing and signing,
        /// it ensures that a signed message is tied to a specific purpose
        /// and domain separator is provided
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="intent"></param>
        /// <returns></returns>
        public SignatureWithBytes SignWithIntent(byte[] bytes, IntentScope intent)
        {
            byte[] intentMessage = CreateMessageWithIntent(intent, bytes);
            // BLAKE2b hash
            byte[] digest = new byte[32];
            Blake2bDigest blake2b = new(256);
            blake2b.BlockUpdate(intentMessage, 0, intentMessage.Length);
            blake2b.DoFinal(digest, 0);

            byte[] privateKeySig = Sign(digest).Data();

            SerializeSignatureInput serializedSigInput = new SerializeSignatureInput();
            serializedSigInput.Signature = privateKeySig;
            serializedSigInput.SignatureScheme = SignatureScheme;
            serializedSigInput.PublicKey = PublicKey;

            string signature = SignatureBase.ToSerializedSignature(serializedSigInput);

            SignatureWithBytes sigWithBytes = new SignatureWithBytes();
            sigWithBytes.Signature = signature;
            sigWithBytes.Bytes = CryptoBytes.ToBase64String(bytes);
            return sigWithBytes;
        }

        /// <summary>
        /// Signs provided transaction block by calling `signWithIntent()`
        /// with a `TransactionData` provided as intent scope
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public SignatureWithBytes SignTransactionBlock(byte[] bytes)
        {
            return SignWithIntent(bytes, IntentScope.TransactionData);
        }

        /// <summary>
        /// TODO: Implement SignPersonalMessage
        /// https://github.com/MystenLabs/sui/blob/a7c64653f084983c369baf12517992fb5c192aec/sdk/typescript/src/cryptography/keypair.ts#L59
        /// </summary>
        /// <returns></returns>
        public SignatureWithBytes SignPersonalMessage(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}