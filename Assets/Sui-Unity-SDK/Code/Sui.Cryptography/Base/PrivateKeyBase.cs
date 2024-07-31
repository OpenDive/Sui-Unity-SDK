//
//  PrivateKeyBase.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
using System.Linq;
using Chaos.NaCl;
using Sui.Rpc.Client;
using Sui.Utilities;

namespace Sui.Cryptography
{
    /// <summary>
    /// Represents all the properties and basic functions of a private key.
    /// </summary>
    public abstract class PrivateKeyBase : CryptographyKey
    {
        public override int KeyLength { get => 32; }

        /// <summary>
        /// The lenght of the signature outputted from Sign().
        /// </summary>
        public const int SignatureLength = 64;

        /// <summary>
        /// The length of the extended private key.
        /// </summary>
        public const int ExtendedKeyLength = 64;

        /// <summary>
        /// Extended private key as a byte array.
        /// </summary>
        protected byte[] _extended_key_bytes;

        /// <summary>
        /// Private key represented as a byte array.
        /// </summary>
        protected byte[] _key_bytes;

        /// <summary>
        /// Public key represented as a hex string.
        /// </summary>
        protected string _key_hex;

        /// <summary>
        /// Public key represented as a base64 string.
        /// </summary>
        protected string _key_base64;

        /// <summary>
        /// Returns the private key as a hex string.
        /// </summary>
        public string KeyHex
        {
            get
            {
                if (this._key_hex == null)
                {
                    if (this._key_bytes != null)
                    {
                        string address_hex = CryptoBytes.ToHexStringLower(this._key_bytes);
                        this._key_hex = "0x" + address_hex;
                    }
                    else // _key_base64 != null -- public key was set using base64 string
                    {
                        byte[] bytes = Convert.FromBase64String(this._key_base64);
                        string hex = BitConverter.ToString(bytes);
                        hex = hex.Replace("-", "").ToLowerInvariant();
                        this._key_hex = "0x" + hex;
                    }
                }

                if (this._key_hex.StartsWith("0x") == false)
                    this._key_hex = $"0x{this._key_hex}";

                return this._key_hex;
            }

            set => this._key_hex = value;
        }

        /// <summary>
        /// Returns the private key as a base64 string.
        /// </summary>
        public string KeyBase64
        {
            get
            {
                if (this._key_base64 == null)
                {
                    if (this._key_bytes != null)
                        this._key_base64 = CryptoBytes.ToBase64String(this.KeyBytes);
                    else // _key_hex != null -- public key was set using hex string
                    {
                        string key = this._key_hex;
                        if (this._key_hex[0..2].Equals("0x")) key = this._key_hex[2..];
                        byte[] key_bytes = key.HexStringToByteArray();
                        this._key_base64 = CryptoBytes.ToBase64String(key_bytes);
                    }

                }
                return this._key_base64;
            }
            set => this._key_base64 = value;
        }

        /// <summary>
        /// Private key represented as a byte array.
        /// </summary>
        public virtual byte[] KeyBytes
        {
            get
            {
                if (this._key_bytes == null)
                {
                    if (this._key_hex != null)
                    {
                        string key = this._key_hex;
                        if (this._key_hex[0..2].Equals("0x")) key = this._key_hex[2..];
                        this._key_bytes = key.HexStringToByteArray();
                    }
                    else // _key_base64 is not null
                        this._key_bytes = CryptoBytes.FromBase64String(_key_base64);
                }
                return this._key_bytes;
            }

            set => this._key_bytes = value;
        }

        public PrivateKeyBase(byte[] private_key)
        {
            if (!(this.CheckIfKeyIsExactLength(private_key) && this.CheckIfKeyIsNull(private_key)))
                return;

            this.KeyBytes = new byte[this.KeyLength];

            Array.Copy(private_key, this.KeyBytes, this.KeyLength);
        }

        public PrivateKeyBase(string private_key)
        {
            if (this.CheckIfKeyIsNull(private_key) == false)
                return;

            byte[] key_value;

            if (Utils.IsValidHexKey(private_key))
            {
                if (private_key.StartsWith("0x"))
                    private_key = private_key[2..];

                key_value = CryptoBytes.FromHexString(private_key);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._key_hex = private_key;
            }
            else if (Utils.IsBase64String(private_key))
            {
                key_value = CryptoBytes.FromBase64String(private_key);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._key_base64 = private_key;
            }
            else
            {
                this.SetError<SuiError>("Invalid key.", private_key);
                return;
            }

            this._key_bytes = key_value;
        }

        public PrivateKeyBase(ReadOnlySpan<byte> private_key)
        {
            if (private_key == null)
            {
                this.SetError<SuiError>("Input private key is null.", private_key.ToArray());
                return;
            }

            if (this.CheckIfKeyIsExactLength(private_key.ToArray()) == false)
                return;

            this.KeyBytes = new byte[this.KeyLength];
            private_key.CopyTo(this.KeyBytes.AsSpan());
        }

        /// <summary>
        /// Sign an arbitrary byte array.
        /// </summary>
        /// <param name="message">The byte array representation of the message.</param>
        /// <returns>A `SignatureBase` object representing the signature.</returns>
        public abstract SignatureBase Sign(byte[] message);

        /// <summary>
        /// Sign an arbitrary b64 string.
        /// </summary>
        /// <param name="b64_message">The base 64 encoded message as a string value.</param>
        /// <returns>A `SignatureBase` object representing the signature.</returns>
        public abstract SignatureBase Sign(string b64_message);

        /// <summary>
        /// Return the private key as a hex string.
        /// </summary>
        /// <returns>The private key as a hex string.</returns>
        public string ToHex() => this.KeyHex;

        /// <summary>
        /// Return the private key as a base64 string.
        /// </summary>
        /// <returns>The private key as a base64 string.</returns>
        public string ToBase64() => this.KeyBase64;

        /// <summary>
        /// Derives a public key from the current private key.
        /// </summary>
        /// <returns></returns>
        public abstract PublicKeyBase PublicKey();

        public override bool Equals(object obj)
        {
            if (obj is PrivateKeyBase private_key)
                return private_key.KeyBytes.SequenceEqual(this.KeyBytes);

            return false;
        }

        public static bool operator ==(PrivateKeyBase lhs, PrivateKeyBase rhs)
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

        public static bool operator !=(PrivateKeyBase lhs, PrivateKeyBase rhs) => !(lhs == rhs);

        public override int GetHashCode() => this.KeyHex.GetHashCode();

        public override string ToString() => this.KeyBase64;
    }
}