//
//  SignatureBase.cs
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
    /// Represents all the properties and basic functions of a signature.
    /// </summary>
    public abstract class SignatureBase : CryptographyKey
    {
        public override int KeyLength { get => 64; }

        /// <summary>
        /// Signature represented as a byte array.
        /// </summary>
        protected byte[] _signature_bytes;

        /// <summary>
        /// Signature represented as a hex string.
        /// </summary>
        protected string _signature_hex;

        /// <summary>
        /// Signature represented as a base64 string.
        /// </summary>
        protected string _signature_base64;

        /// <summary>
        /// Returns the signature as a hex string.
        /// </summary>
        public string SignatureHex
        {
            get
            {
                if (this._signature_hex == null)
                {
                    if (this._signature_bytes != null)
                    {
                        string address_hex = CryptoBytes.ToHexStringLower(this._signature_bytes);
                        this._signature_hex = "0x" + address_hex;
                    }
                    else // _signature_base64 != null -- signature was set using base64 string
                    {
                        byte[] bytes = Convert.FromBase64String(this._signature_base64);
                        string hex = BitConverter.ToString(bytes);
                        hex = hex.Replace("-", "").ToLowerInvariant();
                        this._signature_hex = "0x" + hex;
                    }
                }

                if (this._signature_hex.StartsWith("0x") == false)
                    this._signature_hex = $"0x{this._signature_hex}";

                return this._signature_hex;
            }

            set => this._signature_hex = value;
        }

        /// <summary>
        /// Returns the signature as a base64 string.
        /// </summary>
        public string SignatureBase64
        {
            get
            {
                if (this._signature_base64 == null)
                {
                    if (this._signature_bytes != null)
                        this._signature_base64 = CryptoBytes.ToBase64String(this.SignatureBytes);
                    else // _signature_hex != null -- signature was set using hex string
                    {
                        string key = this._signature_hex;
                        if (this._signature_hex[0..2].Equals("0x")) key = this._signature_hex[2..];
                        byte[] key_bytes = key.HexStringToByteArray();
                        this._signature_base64 = CryptoBytes.ToBase64String(key_bytes);
                    }

                }
                return this._signature_base64;
            }
            set => this._signature_base64 = value;
        }

        /// <summary>
        /// Signature represented as a byte array.
        /// </summary>
        public virtual byte[] SignatureBytes
        {
            get
            {
                if (this._signature_bytes == null)
                {
                    if (this._signature_hex != null)
                    {
                        string key = this._signature_hex;
                        if (this._signature_hex[0..2].Equals("0x")) key = this._signature_hex[2..];
                        this._signature_bytes = key.HexStringToByteArray();
                    }
                    else // _keyBase64 is not null
                        this._signature_bytes = CryptoBytes.FromBase64String(_signature_base64);
                }
                return this._signature_bytes;
            }

            set => this._signature_bytes = value;
        }

        public SignatureBase(byte[] signature)
        {
            if (!(this.CheckIfKeyIsExactLength(signature) && this.CheckIfKeyIsNull(signature)))
                return;

            this.SignatureBytes = new byte[this.KeyLength];

            Array.Copy(signature, this.SignatureBytes, this.KeyLength);
        }

        public SignatureBase(string signature)
        {
            if (this.CheckIfKeyIsNull(signature) == false)
                return;

            byte[] key_value;

            if (Utils.IsValidHexKey(signature))
            {
                if (signature.StartsWith("0x"))
                    signature = signature[2..];

                key_value = CryptoBytes.FromHexString(signature);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._signature_hex = signature;
            }
            else if (Utils.IsBase64String(signature))
            {
                key_value = CryptoBytes.FromBase64String(signature);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._signature_base64 = signature;
            }
            else
            {
                this.SetError<SuiError>("Invalid signature.", signature);
                return;
            }

            this._signature_bytes = key_value;
        }

        public SignatureBase(ReadOnlySpan<byte> signature)
        {
            if (signature == null)
            {
                this.SetError<SuiError>("Input signature is null.", signature.ToArray());
                return;
            }

            if (this.CheckIfKeyIsExactLength(signature.ToArray()) == false)
                return;

            this.SignatureBytes = new byte[this.KeyLength];
            signature.CopyTo(this.SignatureBytes.AsSpan());
        }

        /// <summary>
        /// Return the signature as a hex string.
        /// </summary>
        /// <returns>The signature as a hex string.</returns>
        public string ToHex() => this.SignatureHex;

        /// <summary>
        /// Return the signature as a base64 string.
        /// </summary>
        /// <returns>The signature as a base64 string.</returns>
        public string ToBase64() => this.SignatureBase64;

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() => this.ToBase64();

        public override bool Equals(object obj)
        {
            if (obj is Signature signature)
                return signature.SignatureBytes.SequenceEqual(this.SignatureBytes);

            return false;
        }
    }
}