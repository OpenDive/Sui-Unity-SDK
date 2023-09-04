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
        public const SignatureScheme signatureScheme = SignatureScheme.ED25519;

        /// <summary>
        /// Used to define the format of the string passed to generate private key
        /// </summary>
        //public enum KeyFormat
        //{
        //    HEX,
        //    BASE64
        //};
        //private KeyFormat _keyFormat;

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

        public override SignatureScheme SignatureScheme => throw new NotImplementedException();

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

        public PrivateKey(ReadOnlySpan<byte> privateKey)
        {
            if (privateKey.Length != KeyLength)
                throw new ArgumentException("Usage: Invalid key length: ", nameof(privateKey)
                    + ". Length must be of length: " + KeyLength + ".");
            KeyBytes = new byte[KeyLength];
            privateKey.CopyTo(KeyBytes.AsSpan());

            _extendedKeyBytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(KeyBytes);
        }

        public PrivateKey(string key)
        {
            if (Utilities.Utils.IsValidEd25519Key(key))
            {
                if (!Utilities.Utils.IsValidHexAddress(key))
                    throw new ArgumentException("Invalid key", nameof(key));
                KeyHex = key ?? throw new ArgumentNullException(nameof(key));
            }
            else if(Utilities.Utils.IsBase64String(key))
            {
                if (!Utilities.Utils.IsBase64String(key))
                    throw new ArgumentException("Invalid key", nameof(key));
                KeyBase64 = key ?? throw new ArgumentNullException(nameof(key));
            }
            else
            {
                throw new ArgumentException("Invalid key: ", nameof(key));
            }
        }

        public string Hex() => KeyHex;

        public string Base64() => KeyBase64;

        public SignatureBase Sign(byte[] message)
        {
            ArraySegment<byte> signature = new(new byte[64]);
            Chaos.NaCl.Ed25519.Sign(signature,
                new ArraySegment<byte>(message),
                new ArraySegment<byte>(_extendedKeyBytes));
            return new Signature(signature.Array);
        }

        /// <summary>
        /// Return a signature as a base64 string
        /// </summary>
        /// <param name="b64Message"></param>
        /// <returns></returns>
        public string Sign(string b64Message)
        {
            byte[] bytes = CryptoBytes.FromBase64String(b64Message);
            Signature signature = (Signature)Sign(bytes);
            return CryptoBytes.ToBase64String(signature.Data());
        }

        public static bool operator ==(PrivateKey lhs, PrivateKey rhs)
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

        public static bool operator !=(PrivateKey lhs, PrivateKey rhs) => !(lhs == rhs);

        public override bool Equals(object obj)
        {
            if (obj is PrivateKey privateKey)
            {
                return privateKey.KeyHex == this.KeyHex;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return KeyHex.GetHashCode();
        }

        public override string ToString()
        {
            return KeyHex;
        }

        public static PrivateKeyBase FromHex(string hexStr)
        {
            throw new NotImplementedException();
        }

        public static PrivateKeyBase FromBase64(string base64Str)
        {
            throw new NotImplementedException();
        }

        public static PrivateKeyBase Random()
        {
            byte[] seed = new byte[Chaos.NaCl.Ed25519.PrivateKeySeedSizeInBytes];
            RandomUtils.GetBytes(seed); // TODO: Remove NBitcoin dependency
            return new PrivateKey(seed);
        }
    }
}