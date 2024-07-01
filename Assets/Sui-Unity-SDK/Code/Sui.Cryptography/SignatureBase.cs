using System;
using System.Linq;
using Chaos.NaCl;
using UnityEngine;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    /// <summary>
    /// Represents all the properties and basic functions of a signature.
    /// </summary>
    public abstract class SignatureBase
    {
        /// <summary>
        /// The length of the signature.
        /// </summary>
        public const int SignatureLength = 64;

        protected byte[] _signatureBytes;

        private string _signatureHex;

        private string _signatureBase64;

        public string SignatureHex
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public string SignatureBase64
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        /// <summary>
        /// Signature as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] Data() => _signatureBytes;

        /// <summary>
        /// Return the signature as a hex string.
        /// </summary>
        /// <returns>The signature as a hex string.</returns>
        public string ToHex() => SignatureHex;

        /// <summary>
        /// Return the signature as a base64 string.
        /// </summary>
        /// <returns>The signature as a base64 string.</returns>
        public string ToBase64() => SignatureBase64;

        /// <summary>
        /// Create a serialized signature from:
        /// signature scheme, signature, and public key.
        /// </summary>
        /// <param name="serializeSignatureInput"></param>
        /// <returns></returns>
        public static string ToSerializedSignature(SerializeSignatureInput serializeSignatureInput)
        {
            PublicKeyBase publicKey = serializeSignatureInput.PublicKey;
            byte[] signature = serializeSignatureInput.Signature;
            SignatureScheme signatureScheme = serializeSignatureInput.SignatureScheme;

            if (publicKey == null) throw new Exception("Public key is required");

            byte[] publicKeyBytes = publicKey.ToRawBytes();
            byte[] serializedSignature = new byte[1 + signature.Length + publicKey.KeyLength];

            serializedSignature[0] = SignatureSchemeToFlag.GetFlag(signatureScheme);
            Array.Copy(signature, 0, serializedSignature, 1, signature.Length);
            Array.Copy(publicKeyBytes, 0, serializedSignature, 1 + signature.Length, publicKeyBytes.Length);

            return CryptoBytes.ToBase64String(serializedSignature);
        }

        /// <summary>
        /// Decodes a serialized signature into its constituent components:
        /// the signature scheme, the actual signature, and the public key
        /// </summary>
        /// <param name="serializedSignature">Sui signature as a base64 string.</param>
        /// <returns></returns>
        public static ParsedSignatureOutput ParseSerializedSignature(string serializedSignature)
        {
            byte[] bytes = Convert.FromBase64String(serializedSignature);
            SignatureScheme signatureScheme = SignatureFlagToScheme.GetScheme(bytes[0]);

            if(signatureScheme == SignatureScheme.MultiSig)
            {
                // TODO: Implement 
            }

            if(signatureScheme == SignatureScheme.Zk)
            {
                throw new NotImplementedException("Unable to parse a zk signature. (not implemented yet)");
            }

            int size = SignatureSchemeToSize.GetSize(signatureScheme);

            byte[] signature = bytes.Skip(1).Take(bytes.Length - 1 - size).ToArray(); // TODO: Verifiy is this is correct. See TS implementation
            byte[] publicKey = bytes.Skip(1 + signature.Length).ToArray();

            return new ParsedSignatureOutput(
                serializedSignature,
                bytes,
                signatureScheme,
                signature,
                publicKey
            );
        }

        public abstract string Serialize();
        public abstract SignatureBase Deserialize(string serializedSignature);
    }

    public class SignatureWithBytes
    {
        public string Bytes { get; set; }
	    public string Signature { get; set; } // SerializedSignature in the TypeScript SDK
    }

    /// <summary>
    /// Represents a Sui signature
    /// https://docs.sui.io/learn/cryptography/sui-signatures#user-signature
    /// </summary>
    public class SerializeSignatureInput
    {
        public SignatureScheme SignatureScheme { get; set; }
        public byte[] Signature { get; set; }
        public PublicKeyBase PublicKey { get; set; }
    }

    /// <summary>
    /// Models the result of parsing a base64 serialized signature
    /// </summary>
    public class ParsedSignatureOutput
    {
        public string SerializedSignature { get; set; }
        public byte[] SerializedSignatureBytes { get; set; }
        public SignatureScheme SignatureScheme { get; set; }
        public byte[] Signature { get; set; }
        public byte[] PublicKey { get; set; }

        public ParsedSignatureOutput(
            string serializedSignature,
            byte[] serializedSignatureBytes,
            SignatureScheme signatureScheme,
            byte[] Signature,
            byte[] publicKey)
        {
            this.SerializedSignature = serializedSignature;
            this.SerializedSignatureBytes = serializedSignatureBytes;
            this.SignatureScheme = signatureScheme;
            this.Signature = Signature;
            this.PublicKey = publicKey;
        }
    }
}