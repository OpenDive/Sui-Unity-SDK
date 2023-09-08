using System;
using Chaos.NaCl;
using NBitcoin;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;
//using Utils = Sui.Utilities.Utils;

namespace Sui.Cryptography.Ed25519
{
    /// <summary>
    /// Implements the functionality of a private key.
    /// The key may be viewed as a Hex or Base58 string as supported by Sui.
    /// </summary>
    public class PrivateKey : PrivateKeyBase, IPrivateKey
    {
        /// <summary>
        /// ED25519 signature scheme identifier.
        /// </summary>
        public override SignatureScheme SignatureScheme => SignatureScheme.ED25519;

        /// <summary>
        /// Byte array representations of a ED25519 private key
        /// </summary>
        public override byte[] KeyBytes
        {
            get
            {
                // if the private key bytes have not being initialized,
                // but the 32-byte (64 character) string private key has been set
                // decode the string based on the key format (i.e. Hex or Base58)
                if (_keyBytes == null && _extendedKeyBytes == null)
                {
                    if (_keyHex != null)
                    {
                        string key = _keyHex;
                        if (_keyHex[0..2].Equals("0x")) { key = _keyHex[2..]; } // Trim the private key hex string

                        byte[] seed = key.HexStringToByteArray(); // Turn private key hex string into byte to be used a seed to derive the extended key
                        _keyBytes = seed;
                    }
                    else // _keyBase64 is not null
                    {
                        string key = _keyBase64;
                        byte[] seed = CryptoBytes.FromBase64String(key);
                        _keyBytes = seed;
                    }
                    _extendedKeyBytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(_keyBytes);
                }
                return _keyBytes;
            }
            set
            {
                if (value.Length != KeyLength)
                    throw new ArgumentException("Invalid key length: ", nameof(value));

                _keyBytes = value;
                _extendedKeyBytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(value);
            }
        }

        /// <summary>
        /// Create a PrivateKey object from a byte array representation
        /// </summary>
        /// <param name="privateKey"></param>
        public PrivateKey(byte[] privateKey)
        {
            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));
            if (privateKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(privateKey));

            KeyBytes = new byte[KeyLength];
            Array.Copy(
                privateKey,
                KeyBytes,
                KeyLength
            );

            _extendedKeyBytes = new byte[Chaos.NaCl.Ed25519.ExpandedPrivateKeySizeInBytes];
            Array.Copy(
                Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(KeyBytes),
                _extendedKeyBytes,
                Chaos.NaCl.Ed25519.ExpandedPrivateKeySizeInBytes
            );
        }

        /// <summary>
        /// Create a PrivateKey object from a byte array representation.
        /// </summary>
        /// <param name="privateKey"></param>
        public PrivateKey(ReadOnlySpan<byte> privateKey)
        {
            if (privateKey.Length != KeyLength)
                throw new ArgumentException("Usage: Invalid key length: ", nameof(privateKey)
                    + ". Length must be of length: " + KeyLength + ".");
            KeyBytes = new byte[KeyLength];
            privateKey.CopyTo(KeyBytes.AsSpan());

            _extendedKeyBytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(KeyBytes);
        }

        /// <summary>
        /// Create a PrivateKey object from a hex or base64 string.
        /// </summary>
        /// <param name="privateKey"></param>
        public PrivateKey(string privateKey)
        {
            // TODO: Check valid lenghts and throw respective exception
            if (Utilities.Utils.IsValidEd25519HexKey(privateKey))
            {
                if (!Utilities.Utils.IsValidEd25519HexKey(privateKey))
                    throw new ArgumentException("Invalid key", nameof(privateKey));
                KeyHex = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            }
            else if(Utilities.Utils.IsBase64String(privateKey))
            {
                if (!Utilities.Utils.IsBase64String(privateKey))
                    throw new ArgumentException("Invalid key", nameof(privateKey));
                KeyBase64 = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            }
            else
            {
                throw new ArgumentException("Invalid key: ", nameof(privateKey));
            }
        }

        /// <summary>
        /// Derive public key from the private key.
        /// </summary>
        /// <returns></returns>
        //public PublicKey PublicKey()
        //{
        //    PublicKey publicKey = new PublicKey(
        //        Chaos.NaCl.Ed25519.PublicKeyFromSeed(KeyBytes)
        //    );
        //    return publicKey;
        //}
        public override PublicKeyBase PublicKey()
        {
            PublicKey publicKey = new PublicKey(
                Chaos.NaCl.Ed25519.PublicKeyFromSeed(KeyBytes)
            );
            return publicKey;
        }

        /// <summary>
        /// Sign an arbitrary message represented as a byte array using
        /// the ED25519 private key.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override SignatureBase Sign(byte[] message)
        {
            ArraySegment<byte> signature = new(new byte[64]);
            Chaos.NaCl.Ed25519.Sign(signature,
                new ArraySegment<byte>(message),
                new ArraySegment<byte>(_extendedKeyBytes));
            return new Signature(signature.Array);
        }

        /// <summary>
        /// Return a signature as a base64 string.
        /// </summary>
        /// <param name="b64Message"></param>
        /// <returns></returns>
        public override byte[] Sign(string b64Message)
        {
            byte[] bytes = CryptoBytes.FromBase64String(b64Message);
            Signature signature = (Signature)Sign(bytes);
            //return CryptoBytes.ToBase64String(signature.Data());
            return signature.Data();
        }

        //string IPrivateKey.Sign(string b64Message)
        //{
        //    byte[] bytes = Sign(b64Message);
        //    return CryptoBytes.ToBase64String(bytes);
        //}

        //public static PrivateKeyBase FromSecretKey(int[] secretKey)
        //{
        //    throw new NotImplementedException();
        //}

        //public static PrivateKeyBase FromHex(string hexStr)
        //{
        //    throw new NotImplementedException();
        //}

        //public static PrivateKeyBase FromBase64(string base64Str)
        //{
        //    throw new NotImplementedException();
        //}

        public static PrivateKeyBase Random()
        {
            byte[] seed = new byte[Chaos.NaCl.Ed25519.PrivateKeySeedSizeInBytes];
            RandomUtils.GetBytes(seed); // TODO: Remove NBitcoin dependency
            return new PrivateKey(seed);
        }
    }
}