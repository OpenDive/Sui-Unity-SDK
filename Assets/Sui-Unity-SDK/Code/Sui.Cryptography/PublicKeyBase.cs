using System;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    public interface PublicKeyBase
    {
        /// <summary>
        /// Return the base-64 representation of the public key
        /// </summary>
        /// <returns></returns>
        public string ToBase64()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the Sui representation of the public key encoded in
        /// base-64. A Sui public key is formed by the concatenation
        /// of the scheme flag with the raw bytes of the public key
        /// </summary>
        /// <returns></returns>
        public string ToSuiPublicKey()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the signature is valid for for the provided message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        abstract public bool Verify(byte[] message, ISignature signature);

        // TODO: VerifyWithIntent

        // TODO: verifyPersonalMessage

        // TODO: verifyTransactionBlock

        /// <summary>
        /// Returns the bytes representation of the public key
        /// prefixed with the signature scheme flag
        /// </summary>
        /// <returns></returns>
        public byte[] ToSuiBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the byte array representation of the public key
        /// </summary>
        /// <returns></returns>
        abstract public byte[] ToRawBytes();

        /// <summary>
        /// // TODO: Look into whether Sui uses Ed25519 for all schemes public keys to generate the address
        /// Return the Sui address associated with this Ed25519 public key
        /// </summary>
        /// <returns></returns>
        public string ToSuiAddress()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return signature scheme flag of the public key
        /// </summary>
        /// <returns></returns>
        abstract public SignatureScheme Flag();

        //TODO: public void Serialize(Serializer serializer);
        //TODO: public void Deserialize(Deserializer deserializer);
    }
}