using System;
using Chaos.NaCl;
using Sui.Utils;

namespace Sui.Cryptography.Ed25519
{
    public class PrivateKey : IPrivateKey
    {
        public const int ExtendedKeyLength = 64;
        public const int KeyLength = 32;
        private string _key;
        private byte[] _extendedKeyBytes;
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
                // if the private key bytes have not being initialized, but a 32-byte (64 character) string private has been set
                if (_keyBytes == null && _extendedKeyBytes == null && _key != null)
                {
                    string key = _key;
                    if (_key[0..2].Equals("0x")) { key = _key[2..]; } // Trim the private key hex string

                    byte[] seed = key.HexStringToByteArray(); // Turn private key hex string into byte to be used a seed to derive the extended key
                    _keyBytes = seed;
                    _extendedKeyBytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(seed);
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

        public PrivateKey(byte[] privateKey)
        {
            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));
            if (privateKey.Length != KeyLength)
                throw new ArgumentException("Invalid key length: ", nameof(privateKey));
            KeyBytes = new byte[KeyLength];
            Array.Copy(privateKey, KeyBytes, KeyLength);

            _extendedKeyBytes = new byte[Chaos.NaCl.Ed25519.ExpandedPrivateKeySizeInBytes];
            Array.Copy(Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(KeyBytes), _extendedKeyBytes, Chaos.NaCl.Ed25519.ExpandedPrivateKeySizeInBytes);
        }

        public string Hex()
        {
            throw new NotImplementedException();
        }

        public string Base64()
        {
            throw new NotImplementedException();
        }

        public ISignature Sign(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}