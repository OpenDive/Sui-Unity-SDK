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

            switch (signatureScheme)
            {
                case SignatureScheme.ED25519:
                    PrivateKey = new Cryptography.Ed25519.PrivateKey(privateKey);
                    PublicKey = PrivateKey.PublicKey();
                    break;
                case SignatureScheme.Secp256k1:
                    throw new NotImplementedException();
                    //break;
                case SignatureScheme.Secp256r1:
                    throw new NotImplementedException();
                    //break;
                case SignatureScheme.MultiSig:
                    throw new NotImplementedException();
                    //break;
                case SignatureScheme.Zk:
                    throw new NotImplementedException();
                    //break;
            }
        }

        /// <summary>
        /// Generates an account from a random seed.
        /// </summary>
        /// <param name="signatureScheme"></param>
        public Account(SignatureScheme signatureScheme = SignatureScheme.ED25519)
        {
            SignatureScheme = signatureScheme;

            switch (signatureScheme)
            {
                case SignatureScheme.ED25519:
                    PrivateKey = Cryptography.Ed25519.PrivateKey.Random();
                    PublicKey = PrivateKey.PublicKey();
                    break;
                case SignatureScheme.Secp256k1:
                    throw new NotImplementedException();
                    //break;
                case SignatureScheme.Secp256r1:
                    throw new NotImplementedException();
                    //break;
                case SignatureScheme.MultiSig:
                    throw new NotImplementedException();
                    //break;
                case SignatureScheme.Zk:
                    throw new NotImplementedException();
                    //break;
            }
        }

        /// <summary>
        /// Generate an Account of given signature scheme
        /// </summary>
        /// <param name="signatureScheme"></param>
        /// <returns></returns>
        public static Account Generate(SignatureScheme signatureScheme = SignatureScheme.ED25519)
        {
            return new Account(signatureScheme);
        }

        /// <summary>
        /// Verifies a given signature.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public bool Verify(byte[] message, SignatureBase signature)
        {
            return PublicKey.Verify(message, signature);
        }

        /// <summary>
        /// Signs a message with the account's private key.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public SignatureBase Sign(byte[] message) => PrivateKey.Sign(message);

        /// <summary>
        /// Derives a Sui address from the account's public key.
        /// </summary>
        /// <returns></returns>
        public AccountAddress SuiAddress() => PublicKey.ToSuiAddress();

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