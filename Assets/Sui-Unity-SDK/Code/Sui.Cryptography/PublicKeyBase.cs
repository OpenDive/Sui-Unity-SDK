using System;
using Chaos.NaCl;
using Org.BouncyCastle.Crypto.Digests;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    public abstract class PublicKeyBase
    {
        public abstract SignatureScheme SignatureScheme { get; }
        public abstract int KeyLength { get; }
        public string _key;
        private byte[] _keyBytes;

        public string Key
        {
            get
            {
                if (_key == null && _keyBytes != null)
                {
                    string addressHex = CryptoBytes.ToHexStringLower(_keyBytes);
                    _key = "0x" + addressHex;
                }
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        public byte[] KeyBytes
        {
            get
            {
                if (_keyBytes == null && _key != null)
                {
                    string key = _key;
                    if (_key[0..2].Equals("0x")) { key = _key[2..]; }
                    _keyBytes = key.HexStringToByteArray();
                }
                return _keyBytes;
            }

            set
            {
                _keyBytes = value;
            }
        }

        public PublicKeyBase(byte[] publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));
            if (publicKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(publicKey));
            KeyBytes = new byte[KeyLength];
            Array.Copy(publicKey, KeyBytes, KeyLength);
        }

        public PublicKeyBase(string publicKey)
        {
            if (!Utils.IsValidHexAddress(publicKey))
                throw new ArgumentException("Invalid key", nameof(publicKey));

            Key = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        }

        /// <summary>
        /// Initialize the PublicKey object from the given string.
        /// </summary>
        /// <param name="publicKey">The public key as a byte array.</param>
        public PublicKeyBase(ReadOnlySpan<byte> publicKey)
        {
            if (publicKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(publicKey));
            KeyBytes = new byte[KeyLength];
            publicKey.CopyTo(KeyBytes.AsSpan());
        }


        /// <summary>
        /// Return the base-64 representation of the raw public key
        /// </summary>
        /// <returns></returns>
        public string ToBase64()
        {
            //string base64Key = CryptoBytes.ToBase64String(ToRawBytes()); // TODO: Review
            string base64Key = CryptoBytes.ToBase64String(KeyBytes);
            return base64Key;
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
            byte[] bytes = ToSuiBytes();
            return CryptoBytes.ToBase64String(bytes);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the signature is valid for for the provided message
        ///
        /// When a user submits a signed transaction, a serialized signature
        /// and a serialized transaction data is submitted.
        /// The serialized transaction data is the BCS serialized bytes of
        /// the struct TransactionData and the serialized signature is defined
        /// as a concatenation of bytes of flag || sig || pk.
        ///
        /// https://docs.sui.io/learn/cryptography/sui-signatures#user-signature
        /// </summary>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        abstract public bool Verify(byte[] message, SignatureBase signature);

        // TODO: VerifyWithIntent

        // TODO: verifyPersonalMessage

        // TODO: verifyTransactionBlock

        /// <summary>
        /// Byte representation of the public key
        /// prefixed with the signature scheme flag.
        /// SIGNATURE_SCHEME_FLAG + public key bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ToSuiBytes()
        {
            byte[] rawBytes = ToRawBytes();
            byte[] suiBytes = new byte[1 + rawBytes.Length];
            suiBytes[0] = Flag();
            Array.Copy(rawBytes, 0, suiBytes, 1, suiBytes.Length);

            return suiBytes;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the byte array representation of the public key
        /// TODO: Review implementation, we are changing it from the TpeScript SDK. In this impl we can easily convert between hex, base64 and raw bytes
        /// TODO: Review if we need this function otherwise, we can just define "KeyBytes" to be what returns the raw bytes of the public key
        /// </summary>
        /// <returns></returns>
        //abstract public byte[] ToRawBytes();
        public byte[] ToRawBytes()
        {
            return KeyBytes;
        }

        /// <summary>
        /// // TODO: Look into whether Sui uses Ed25519 for all schemes public keys to generate the address
        /// Return the Sui address associated with this Ed25519 public key
        ///
        /// For deriving a 32-byte Sui address,
        /// Sui hashes the signature scheme flag 1-byte concatenated with public key bytes
        /// using the BLAKE2b (256 bits output) hashing function.
        ///
        /// Sui address currently supports pure Ed25519, Secp256k1, Secp256r1,
        /// and MultiSig with corresponding flag bytes of 0x00, 0x01, 0x02, and 0x03, respectively.
        /// https://docs.sui.io/learn/cryptography/sui-wallet-specs#address-format
        /// </summary>
        /// <returns></returns>
        public string ToSuiAddress()
        {
            // Signature scheme (1 byte) + public key bytes
            //byte[] addressBytes = new byte[KeyLength + 1];
            //addressBytes[0] = SignatureSchemeToFlag.ED25519;
            //Array.Copy(KeyBytes, 0, addressBytes, 1, KeyLength);
            byte[] addressBytes = ToSuiBytes();

            // BLAKE2b hash
            byte[] result = new byte[64];
            Blake2bDigest blake2b = new(256);
            blake2b.BlockUpdate(addressBytes, 0, addressBytes.Length);
            blake2b.DoFinal(result, 0);

            // Convert to hex string
            string addressHex = BitConverter.ToString(result); // Turn into hexadecimal string
            addressHex = addressHex.Replace("-", "").ToLowerInvariant(); // Remove '-' characters from hex string hash
            return "0x" + addressHex;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Return signature scheme flag of the public key
        /// </summary>
        /// <returns></returns>
        abstract public byte Flag();

        //TODO: public void Serialize(Serializer serializer);
        //TODO: public void Deserialize(Deserializer deserializer);

        public override bool Equals(object obj)
        {
            if (obj is PublicKeyBase publicKey)
            {
                return publicKey.Key.Equals(Key);
            }

            return false;
        }

        /// <summary>
        /// Required when overriding Equals method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        /// <summary>
        /// TODO: Implement ToString(), should use Base64 according to TypeScript SDK
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return Key;
            throw new NotImplementedException();
        }

        public static bool operator ==(PublicKeyBase lhs, PublicKeyBase rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PublicKeyBase lhs, PublicKeyBase rhs)
        {
            return lhs == rhs;
        }
    }
}