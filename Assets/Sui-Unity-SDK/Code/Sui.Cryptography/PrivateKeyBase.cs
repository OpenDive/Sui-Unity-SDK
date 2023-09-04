using System;
using Chaos.NaCl;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    /// <summary>
    /// Represents all the properties and basic functions of a private key.
    /// </summary>
    public abstract class PrivateKeyBase
    {
        /// <summary>
        /// Signature scheme for the private key.
        /// </summary>
        public abstract SignatureScheme SignatureScheme { get; }

        /// <summary>
        /// The length of the private key.
        /// </summary>
        public const int KeyLength = 32;

        /// <summary>
        /// The length of the extended private key.
        /// </summary>
        public const int ExtendedKeyLength = 64;

        /// <summary>
        /// Extended private key as a byte array.
        /// </summary>
        protected byte[] _extendedKeyBytes;

        /// <summary>
        /// Private key represented as a byte array.
        /// </summary>
        protected byte[] _keyBytes;

        /// <summary>
        /// Public key represented as a hex string.
        /// </summary>
        protected string _keyHex;

        /// <summary>
        /// Public key represented as a base64 string.
        /// </summary>
        protected string _keyBase64;

        /// <summary>
        /// Returns the private key as a hex string.
        /// </summary>
        public string KeyHex
        {
            get
            {
                if (_keyHex == null)
                {
                    if (_keyBytes != null)
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
        /// Returns the private key as a base64 string.
        /// </summary>
        public string KeyBase64
        {
            get
            {
                if (_keyBase64 == null)
                {
                    if (_keyBytes != null)
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
        /// Returns the private key as a byte array.
        /// </summary>
        public abstract byte[] KeyBytes { get; set; }

        /// <summary>
        /// Sign an arbitrary byte array.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract SignatureBase Sign(byte[] message);

        /// <summary>
        /// Sign an arbitrary b64 string.
        /// </summary>
        /// <param name="b64Message"></param>
        /// <returns></returns>
        public abstract string Sign(string b64Message);

        /// <summary>
        /// Return the private key as a hex string.
        /// </summary>
        /// <returns>The private key as a hex string.</returns>
        public string ToHex() => KeyHex;

        /// <summary>
        /// Return the private key as a base64 string.
        /// </summary>
        /// <returns>The private key as a base64 string.</returns>
        public string ToBase64() => KeyBase64;

        //public void Serialize(Serializer serializer);
    }
}