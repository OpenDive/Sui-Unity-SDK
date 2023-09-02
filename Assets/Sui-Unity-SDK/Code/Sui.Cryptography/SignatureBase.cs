using System;
using System.Linq;
using Chaos.NaCl;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    public abstract class SignatureBase
    {
        public const int SignatureLength = 64;
        public abstract byte[] Data();

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