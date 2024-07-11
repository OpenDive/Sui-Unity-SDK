using System;
using Org.BouncyCastle.Crypto.Digests;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography.Ed25519
{
    /// <summary>
    /// Implements an ED25519 public key functionality 
    /// </summary>
    public class PublicKey : PublicKeyBase
    {
        /// <summary>
        /// Defines signature scheme to be ED25519
        /// </summary>
        public override SignatureScheme SignatureScheme { get => SignatureScheme.ED25519; }

        /// <summary>
        /// The length of an ED25519 public key - 32
        /// </summary>
        public override int KeyLength { get => SignatureSchemeToSize.ED25519; }

        /// <summary>
        /// Creates an ED25519 PublicKey object from a byte array that represents a public key
        /// </summary>
        /// <param name="publicKey">ED25519 public key as a byte array buffer</param>
        public PublicKey(byte[] publicKey) : base(publicKey) { }

        /// <summary>
        /// Creates an ED25519 PublicKey object from a base64 or hex string that represents a public key
        /// </summary>
        /// <param name="publicKey">A hex or base64 string that represents a ED25519 public key</param>
        public PublicKey(string publicKey) : base(publicKey) { }

        /// <summary>
        /// Return the Sui address associated with this Ed25519 public key
        /// </summary>
        /// <returns></returns>
        public override byte Flag() => SignatureSchemeToFlag.ED25519;

        /// <summary>
        /// Verifies a signature from a Signature object.
        /// </summary>
        /// <param name="message">The message that was signed by the private key.</param>
        /// <param name="signature">The serialized signature to verify.</param>
        /// <returns></returns>
        public override bool Verify(byte[] message, SignatureBase signature)
        {
            throw new System.NotImplementedException();
        }

        public bool VerifyWithIntent(byte[] bytes, SignatureBase signature, IntentScope intent)
        {
            byte[] intentMessage = CreateMessageWithIntent(intent, bytes);

            // BLAKE2b hash
            byte[] digest = new byte[32];
            Blake2bDigest blake2b = new(256);
            blake2b.BlockUpdate(intentMessage, 0, intentMessage.Length);
            blake2b.DoFinal(digest, 0);

            return VerifyRaw(digest, signature.Data());
        }

        public override bool VerifyTransactionBlock(byte[] transaction_block, SignatureBase signature)
        {
            return VerifyWithIntent(transaction_block, signature, IntentScope.TransactionData);
        }

        /// <summary>
        /// Verifies a signatures that has been serialized as hex string.
        /// </summary>
        /// <param name="message">The message that was signed by the private key.</param>
        /// <param name="serializedSignature">The serialized signature to verify.</param>
        /// <returns></returns>
        public override bool Verify(byte[] message, string serializedSignature)
        {
            ParsedSignatureOutput parsedSignature
                = SignatureBase.ParseSerializedSignature(serializedSignature);

            SignatureScheme signatureScheme = parsedSignature.SignatureScheme;
            if (signatureScheme != SignatureScheme.ED25519)
                throw new Exception("Invalid signature scheme.");

            byte[] publicKey = parsedSignature.PublicKey; // TODO: Think about whether we need to have this as a byte array or just string
            if (ToRawBytes().Equals(publicKey))
                throw new Exception("Signature does not match public key.");

            byte[] signature = parsedSignature.Signature;

            return VerifyRaw(message, signature);
        }

        /// <summary>
        /// Verifies a signature passed as a raw set of bytes.
        /// </summary>
        /// <param name="message">The message that was signed by the private key.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <returns></returns>
        public override bool VerifyRaw(byte[] message, byte[] signature)
        {
            return Chaos.NaCl.Ed25519.Verify(signature, message, ToRawBytes());
        }
    }
}