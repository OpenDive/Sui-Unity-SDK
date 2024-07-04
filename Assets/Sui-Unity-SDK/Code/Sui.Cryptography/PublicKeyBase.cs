using System;
using System.Data.SqlTypes;
using Chaos.NaCl;
using Org.BouncyCastle.Crypto.Digests;
using Sui.Accounts;
using Sui.Rpc.Models;
using Sui.Utilities;
using UnityEngine;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    public abstract class PublicKeyBase
    {
        /// <summary>
        /// Signature scheme for the public key.
        /// </summary>
        public abstract SignatureScheme SignatureScheme { get; }

        /// <summary>
        /// The lenght of the public key.
        /// </summary>
        public abstract int KeyLength { get; }

        /// <summary>
        /// Public key represented as a byte array.
        /// </summary>
        private byte[] _keyBytes;

        /// <summary>
        /// Public key represented as a hex string.
        /// </summary>
        private string _keyHex;

        /// <summary>
        /// Public key represented as a base64 string.
        /// </summary>
        private string _keyBase64;

        /// <summary>
        /// Public key represented as a hex string.
        /// </summary>
        public string KeyHex
        {
            get
            {
                if (_keyHex == null)
                {
                    if(_keyBytes != null)
                    {
                        string addressHex = CryptoBytes.ToHexStringLower(_keyBytes);
                        _keyHex = "0x" + addressHex;
                    }
                    else // _keyBase64 != null -- public key was set using base64 string
                    {
                        byte[] bytes = Convert.FromBase64String(_keyBase64);
                        string hex = BitConverter.ToString(bytes);
                        hex = hex.Replace("-", "").ToLowerInvariant();
                        _keyHex = "0x" + hex;
                    }
                }

                return _keyHex;
            }

            set => _keyHex = value;
        }

        /// <summary>
        /// Public key represented as a base64 string.
        /// </summary>
        public string KeyBase64
        {
            get
            {
                if (_keyBase64 == null)
                {
                    if(_keyBytes != null)
                        _keyBase64 = CryptoBytes.ToBase64String(KeyBytes);
                    else // _keyHex != null -- public key was set using hex string
                    {
                        string key = _keyHex;
                        if (_keyHex[0..2].Equals("0x")) { key = _keyHex[2..]; }
                        byte[] keyBytes = key.HexStringToByteArray();
                        _keyBase64 = CryptoBytes.ToBase64String(keyBytes);
                    }
                    
                }
                return _keyBase64;
            }
            set => _keyBase64 = value;
        }

        /// <summary>
        /// Public key represented as a byte array.
        /// </summary>
        public byte[] KeyBytes
        {
            get
            {
                if (_keyBytes == null)
                {
                    if(_keyHex != null)
                    {
                        string key = _keyHex;
                        if (_keyHex[0..2].Equals("0x")) { key = _keyHex[2..]; }
                        _keyBytes = key.HexStringToByteArray();
                    }
                    else // _keyBase64 is not null
                        _keyBytes = CryptoBytes.FromBase64String(_keyBase64);
                }
                return _keyBytes;
            }

            set => _keyBytes = value;
        }

        /// <summary>
        /// Creates a PublicKey object from a byte array
        /// </summary>
        /// <param name="publicKey"></param>
        public PublicKeyBase(byte[] publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));
            if (publicKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(publicKey));
            KeyBytes = new byte[KeyLength];
            Array.Copy(publicKey, KeyBytes, KeyLength);
        }

        /// <summary>
        /// Creates a PublicKey object from a hex or base64 string
        /// </summary>
        /// <param name="publicKey"></param>
        public PublicKeyBase(string publicKey)
        {
            if (Utils.IsValidEd25519HexKey(publicKey))
            {
                KeyHex = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
                _keyBytes = CryptoBytes.FromHexString(publicKey);
            }
            else if(Utils.IsBase64String(publicKey))
            {
                KeyBase64 = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
                _keyBytes = CryptoBytes.FromBase64String(publicKey);
            }
            else
            {
                throw new ArgumentException("Invalid key: ", nameof(publicKey));
            }
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
        //public string ToBase64()
        //{
        //    string base64Key = CryptoBytes.ToBase64String(KeyBytes);
        //    return base64Key;
        //}
        public string ToBase64() => KeyBase64;

        /// <summary>
        /// Return the private key as a hex string.
        /// </summary>
        /// <returns>The private key as a hex string.</returns>
        public string ToHex() => KeyHex;

        /// <summary>
        /// Return the Sui representation of the public key encoded in
        /// base-64. A Sui public key is formed by the concatenation
        /// of the scheme flag with the raw bytes of the public key.
        /// </summary>
        /// <returns></returns>
        public string ToSuiPublicKey()
        {
            byte[] bytes = ToSuiBytes();
            return CryptoBytes.ToBase64String(bytes);
        }

        /// <summary>
        /// Creates a PublicKey object from a base-64
        /// string representation of a Sui-formatted public key.
        /// A Sui public key is formed by the concatenation
        /// of the scheme flag with the raw bytes of the public key.
        /// </summary>
        /// <param name="suiPubKey"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static PublicKeyBase FromSuiPublicKey(string suiPublicKey)
        {
            byte[] suiPubKeybytes = CryptoBytes.FromBase64String(suiPublicKey);
            byte flag = suiPubKeybytes[0];
            SignatureScheme signatureScheme = SignatureFlagToScheme.GetScheme(flag);

            int len = suiPubKeybytes.Length - 1;
            byte[] pubKeyBytes = new byte[len];

            switch (signatureScheme)
            {
                case SignatureScheme.ED25519:
                    Array.Copy(suiPubKeybytes, 1, pubKeyBytes, 0, len);
                    return new Ed25519.PublicKey(pubKeyBytes); 
                case SignatureScheme.Secp256k1:
                    //Array.Copy(suiPubKeybytes, 1, pubKeyBytes, 0, len);
                    //return new Sui.Cryptography.Secp256k1.PublicKey(pubKeyBytes);
                    break;
                case SignatureScheme.Secp256r1:
                    //Array.Copy(suiPubKeybytes, 1, pubKeyBytes, 0, len);
                    //return new Sui.Cryptography.Secp256r1.PublicKey(pubKeyBytes);
                    break;
                case SignatureScheme.MultiSig:
                    throw new NotSupportedException("Multisign public key not supported.");
                case SignatureScheme.Zk:
                    throw new NotSupportedException("Zk public key not supported.");
                default:
                    Array.Copy(suiPubKeybytes, 1, pubKeyBytes, 0, len);
                    return new Ed25519.PublicKey(pubKeyBytes);
            }

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

        abstract public bool VerifyTransactionBlock(byte[] transaction_block, SignatureBase signature);

        /// <summary>
        /// Signature is committed to the intent message of the transaction data,
        /// as base-64 encoded string.
        /// A serialized signature has the following format:
        /// (`flag || signature || pubkey` bytes, as base-64 encoded string).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="serializedSignatured"></param>
        /// <returns></returns>
        abstract public bool Verify(byte[] message, string serializedSignatured);

        /// <summary>
        /// Vrification of a plain raw ED25519 signature -- without the Sui format.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        abstract public bool VerifyRaw(byte[] message, byte[] signature);


        // TODO: VerifyWithIntent

        // TODO: verifyPersonalMessage

        // TODO: verifyTransactionBlock

        /// <summary>
        /// Byte representation of the public key
        /// prefixed with the signature scheme flag.
        /// SIGNATURE_SCHEME_FLAG (byte) + public key bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ToSuiBytes()
        {
            return ToSuiBytes(SignatureScheme, ToRawBytes());
        }

        public static byte[] ToSuiBytes(SignatureScheme signatureScheme, byte[] publicKey)
        {
            byte flag = SignatureSchemeToFlag.GetFlag(signatureScheme);
            byte[] suiBytes = new byte[1 + publicKey.Length];
            suiBytes[0] = flag;
            Array.Copy(publicKey, 0, suiBytes, 1, publicKey.Length);

            return suiBytes;
        }

        /// <summary>
        /// Return the byte array representation of the public key
        /// TODO: Review implementation, we are changing it from the TpeScript SDK. In this impl we can easily convert between hex, base64 and raw bytes
        /// TODO: Review if we need this function otherwise, we can just define "KeyBytes" to be what returns the raw bytes of the public key
        /// </summary>
        /// <returns></returns>
        //abstract public byte[] ToRawBytes();
        public byte[] ToRawBytes() => KeyBytes;

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
        //public string ToSuiAddress()
        //{
        //    byte[] addressBytes = ToSuiBytes();

        //    // BLAKE2b hash
        //    byte[] result = new byte[64];
        //    Blake2bDigest blake2b = new(256);
        //    blake2b.BlockUpdate(addressBytes, 0, addressBytes.Length);
        //    blake2b.DoFinal(result, 0);

        //    // Convert to hex string
        //    string addressHex = BitConverter.ToString(result); // Turn into hexadecimal string
        //    addressHex = addressHex.Replace("-", "").ToLowerInvariant(); // Remove '-' characters from hex string hash
        //    return "0x" + addressHex.Substring(0, 64);
        //}

        public string ToSuiAddress()
        {
            byte[] hashed_address = new byte[32];
            byte[] prehash_address = new byte[this.KeyLength + 1];
            prehash_address[0] = SignatureSchemeToFlag.GetFlag(SignatureScheme.ED25519);
            Array.Copy(_keyBytes, 0, prehash_address, 1, _keyBytes.Length);
            Blake2bDigest blake2b = new Blake2bDigest(256);
            blake2b.BlockUpdate(prehash_address, 0, prehash_address.Length);
            blake2b.DoFinal(hashed_address, 0);
            string addressHex = CryptoBytes.ToHexStringLower(hashed_address);
            return NormalizedTypeConverter.NormalizeSuiAddress(addressHex);
        }

        //public static string GetDigestFromBytes(byte[] bytes)
        //{
        //    string type_tag = "TransactionData";
        //    byte[] type_tag_bytes = Encoding.UTF8.GetBytes((type_tag + "::"));

        //    List<byte> data_with_tag = new List<byte>();

        //    data_with_tag.AddRange(type_tag_bytes);
        //    data_with_tag.AddRange(bytes);

        //    byte[] hashed_data = new byte[32];
        //    Blake2bDigest blake2b = new Blake2bDigest(256);
        //    blake2b.BlockUpdate(data_with_tag.ToArray(), 0, data_with_tag.Count());
        //    blake2b.DoFinal(hashed_data, 0);

        //    Base58Encoder base58Encoder = new Base58Encoder();
        //    return base58Encoder.EncodeData(hashed_data);
        //}

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
                return publicKey.KeyHex.Equals(KeyHex);

            return false;
        }

        /// <summary>
        /// Required when overriding Equals method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => KeyHex.GetHashCode();

        /// <summary>
        /// TODO: Implement ToString(), should use Base64 according to TypeScript SDK
        /// </summary>
        /// <returns></returns>
        public override string ToString() => KeyBase64;

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