using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography.Ed25519
{
    /// <summary>
    /// Implements an ED25519 public key functionality 
    /// </summary>
    public class PublicKey : PublicKeyBase
    {
        public override SignatureScheme SignatureScheme { get => SignatureScheme.ED25519; }
        public override int KeyLength { get => 32; }

        public PublicKey(byte[] publicKey) : base(publicKey) { }

        /// <summary>
        /// Return the Sui address associated with this Ed25519 public key
        /// </summary>
        /// <returns></returns>
        public override byte Flag() => SignatureSchemeToFlag.ED25519;

        public override bool Verify(byte[] message, SignatureBase signature)
        {
            throw new System.NotImplementedException();
        }
    }
}