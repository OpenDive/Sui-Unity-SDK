//
//  PublicKeyBase.cs
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
    public abstract class PublicKeyBase : CryptographyKey
    {
        /// <summary>
        /// Public key represented as a byte array.
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
        /// Public key represented as a hex string.
        /// </summary>
        public string KeyHex
        {
            get
            {
                if (this._key_hex == null)
                {
                    if(this._key_bytes != null)
                    {
                        string address_hex = CryptoBytes.ToHexStringLower(this._key_bytes);
                        this._key_hex = "0x" + address_hex;
                    }
                    else // _keyBase64 != null -- public key was set using base64 string
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
        /// Public key represented as a base64 string.
        /// </summary>
        public string KeyBase64
        {
            get
            {
                if (this._key_base64 == null)
                {
                    if(this._key_bytes != null)
                        this._key_base64 = CryptoBytes.ToBase64String(this.KeyBytes);
                    else // _keyHex != null -- public key was set using hex string
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
        /// Public key represented as a byte array.
        /// </summary>
        public byte[] KeyBytes
        {
            get
            {
                if (this._key_bytes == null)
                {
                    if(this._key_hex != null)
                    {
                        string key = this._key_hex;
                        if (this._key_hex[0..2].Equals("0x")) key = this._key_hex[2..];
                        this._key_bytes = key.HexStringToByteArray();
                    }
                    else // _keyBase64 is not null
                        this._key_bytes = CryptoBytes.FromBase64String(this._key_base64);
                }
                return this._key_bytes;
            }

            set => this._key_bytes = value;
        }

        public PublicKeyBase(byte[] public_key)
        {
            if (!(this.CheckIfKeyIsExactLength(public_key) && this.CheckIfKeyIsNull(public_key)))
                return;

            this.KeyBytes = new byte[this.KeyLength];

            Array.Copy(public_key, this.KeyBytes, this.KeyLength);
        }

        public PublicKeyBase(string public_key)
        {
            if (this.CheckIfKeyIsNull(public_key) == false)
                return;

            byte[] key_value;

            if (Utils.IsValidHexKey(public_key))
            {
                if (public_key.StartsWith("0x"))
                    public_key = public_key[2..];

                key_value = CryptoBytes.FromHexString(public_key);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._key_hex = public_key;
            }
            else if (Utils.IsBase64String(public_key))
            {
                key_value = CryptoBytes.FromBase64String(public_key);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._key_base64 = public_key;
            }
            else
            {
                this.SetError<SuiError>("Invalid key.", public_key);
                return;
            }

            this._key_bytes = key_value;
        }

        public PublicKeyBase(ReadOnlySpan<byte> public_key)
        {
            if (public_key == null)
            {
                this.SetError<SuiError>("Input public key is null.", public_key.ToArray());
                return;
            }

            if (this.CheckIfKeyIsExactLength(public_key.ToArray()) == false)
                return;

            this.KeyBytes = new byte[this.KeyLength];
            public_key.CopyTo(this.KeyBytes.AsSpan());
        }

        /// <summary>
        /// Return the base-64 representation of the raw public key.
        /// </summary>
        /// <returns>The public key as a base 64 string.</returns>
        public string ToBase64() => this.KeyBase64;

        /// <summary>
        /// Return the public key as a hex string.
        /// </summary>
        /// <returns>The public key as a hex string.</returns>
        public string ToHex() => this.KeyHex;

        /// <summary>
        /// Verify whether the inputted byte array signature was
        /// comitted given the inputted byte array message.
        /// </summary>
        /// <param name="message">The byte array of the message.</param>
        /// <param name="signature">The byte array of the signature.</param>
        /// <returns>A bool representing whether the inputted message corresponds to the committed signature.</returns>
        abstract public bool Verify(byte[] message, byte[] signature);

        /// <summary>
        /// Return the byte representation of the signature scheme.
        /// </summary>
        /// <returns>A byte value corresponding to the signature scheme.</returns>
        abstract public byte Flag();

        public override bool Equals(object obj)
        {
            if (obj is PublicKeyBase public_key)
                return public_key.KeyBytes.SequenceEqual(this.KeyBytes);

            return false;
        }

        public override int GetHashCode() => this.KeyHex.GetHashCode();

        public override string ToString() => this.KeyBase64;

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

        public static bool operator !=(PublicKeyBase lhs, PublicKeyBase rhs) => !(lhs == rhs);
    }
}