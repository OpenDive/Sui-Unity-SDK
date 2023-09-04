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
        /// The size of an ED25519 public key - 32
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

        public override bool Verify(byte[] message, SignatureBase signature)
        {
            return Chaos.NaCl.Ed25519.Verify(signature.Data(), message, ToRawBytes());
        }
    }
}