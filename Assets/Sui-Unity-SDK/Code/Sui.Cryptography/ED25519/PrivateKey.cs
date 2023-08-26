using System;
using Chaos.NaCl;
using Sui.Utils;

namespace Sui.Cryptography.Ed25519
{
    public class PrivateKey : IPrivateKey
    {
        public const int ExtendedKeyLength = 64;
        public const int KeyLength = 32;
        private byte[] _extendedKeyBytes;
        private byte[] _keyBytes;
        private string _key;

        /// <summary>
        /// Used to define the format of the string passed to generate private key
        /// </summary>
        public enum KeyFormat
        {
            HEX,
            BASE58
        };
        private KeyFormat _keyFormat;

        //public string Key
        //{
        //    get
        //    {
        //        if (_key == null && _keyBytes != null)
        //        {
        //            string addressHex = CryptoBytes.ToHexStringLower(_keyBytes);
        //            _key = "0x" + addressHex;
        //        }

        //        return _key;
        //    }

        //    set
        //    {
        //        _key = value;
        //    }
        //}


        public string KeyHex
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


        public string KeyBase58
        {
            get
            {
                if(_key == null)
                {
                    KeyBase58 = Encoders.Base58.EncodeData(KeyBytes);
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
                // if the private key bytes have not being initialized,
                // but the 32-byte (64 character) string private key has been set
                // decode the string based on the key format (i.e. Hex or Base58)
                if (_keyBytes == null && _extendedKeyBytes == null && _key != null)
                {
                    string key = _key;
                    if(_keyFormat == KeyFormat.HEX)
                    {
                        if (_key[0..2].Equals("0x")) { key = _key[2..]; } // Trim the private key hex string

                        byte[] seed = key.HexStringToByteArray(); // Turn private key hex string into byte to be used a seed to derive the extended key
                        _keyBytes = seed;
                    }
                    else
                    {
                        byte[] seed = Encoders.Base58.DecodeData(key);
                        _keyBytes = seed;
                    }
                }
                _extendedKeyBytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(_keyBytes);
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

        public PrivateKey(byte[] privateKey)
        {
            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));
            if (privateKey.Length != KeyLength)
                throw new ArgumentException("Usage: Invalid key length: ", nameof(privateKey)
                    + ". Length must be of length: " + KeyLength + ".");

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

        public PrivateKey(string key, KeyFormat keyFormat = KeyFormat.HEX)
        {
            if(keyFormat == KeyFormat.HEX)
            {
                if (!Utils.Utils.IsValidHexAddress(key))
                    throw new ArgumentException("Invalid key", nameof(key));
                _keyFormat = keyFormat;
                KeyHex = key ?? throw new ArgumentNullException(nameof(key));
            }
            else
            {
                if (!Base58Encoder.IsValidEncoding(key))
                    throw new ArgumentException("Invalid key", nameof(key));
                _keyFormat = keyFormat;
                KeyBase58 = key ?? throw new ArgumentNullException(nameof(key));
            }
        }

        public string Hex()
        {
            throw new NotImplementedException();
        }

        public string Base64()
        {
            throw new NotImplementedException();
        }

        public static PrivateKey FromHex(string hexStr)
        {
            throw new NotImplementedException();
        }

        public static PrivateKey FromBase64(string base64Str)
        {
            throw new NotImplementedException();
        }

        public static PrivateKey Random()
        {
            throw new NotImplementedException();
        }

        public ISignature Sign(byte[] message)
        {
            ArraySegment<byte> signature = new(new byte[64]);
            Chaos.NaCl.Ed25519.Sign(signature,
                new ArraySegment<byte>(message),
                new ArraySegment<byte>(_extendedKeyBytes));
            return new Signature(signature.Array);
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
    }
}