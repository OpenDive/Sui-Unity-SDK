//
//  AccountAddressBase.cs
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
using Chaos.NaCl;
using Sui.Cryptography;
using Sui.Utilities;
using OpenDive.BCS;
using Sui.Rpc.Client;
using System.Linq;

namespace Sui.Accounts
{
    public abstract class AccountAddressBase : CryptographyKey, ISerializable
    {
        /// <summary>
        /// Account Address represented as a byte array.
        /// </summary>
        protected byte[] _key_bytes;

        /// <summary>
        /// Account Address represented as a hex string.
        /// </summary>
        protected string _key_hex;

        /// <summary>
        /// Account Address represented as a base64 string.
        /// </summary>
        protected string _key_base64;

        /// <summary>
        /// Returns the Account Address as a hex string.
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
        /// Returns the Account Address as a base64 string.
        /// </summary>
        public string KeyBase64
        {
            get
            {
                if (this._key_base64 == null)
                {
                    if (this._key_bytes != null)
                        this._key_base64 = CryptoBytes.ToBase64String(this.KeyBytes);
                    else // _key_base64 != null -- public key was set using hex string
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
        /// Account Address represented as a byte array.
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

        public AccountAddressBase(ErrorBase error)
        {
            this.Error = error;
        }

        public AccountAddressBase() { }

        public AccountAddressBase(byte[] account_address)
        {
            if (!(this.CheckIfKeyIsExactLength(account_address) && this.CheckIfKeyIsNull(account_address)))
                return;

            this.KeyBytes = new byte[this.KeyLength];

            Array.Copy(account_address, this.KeyBytes, this.KeyLength);
        }

        public AccountAddressBase(string account_address)
        {
            if (this.CheckIfKeyIsNull(account_address) == false)
                return;

            byte[] key_value;

            if (Utils.IsValidHexKey(account_address))
            {
                if (account_address.StartsWith("0x"))
                    account_address = account_address[2..];

                key_value = CryptoBytes.FromHexString(account_address);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._key_hex = account_address;
            }
            else if (Utils.IsBase64String(account_address))
            {
                key_value = CryptoBytes.FromBase64String(account_address);

                if (this.CheckIfKeyIsExactLength(key_value) == false)
                    return;

                this._key_base64 = account_address;
            }
            else
            {
                this.SetError<SuiError>("Invalid key.", account_address);
                return;
            }

            this._key_bytes = key_value;
        }

        public AccountAddressBase(ReadOnlySpan<byte> account_address)
        {
            if (account_address == null)
            {
                this.SetError<SuiError>("Input account address is null.", account_address.ToArray());
                return;
            }

            if (this.CheckIfKeyIsExactLength(account_address.ToArray()) == false)
                return;

            this.KeyBytes = new byte[this.KeyLength];
            account_address.CopyTo(this.KeyBytes.AsSpan());
        }

        /// <summary>
        /// Return the Account Address as a hex string.
        /// </summary>
        /// <returns>The Account Address as a hex string.</returns>
        public string ToHex() => this.KeyHex;

        /// <summary>
        /// Return the Account Address as a base64 string.
        /// </summary>
        /// <returns>The Account Address as a base64 string.</returns>
        public string ToBase64() => this.KeyBase64;

        public void Serialize(Serialization serializer)
            => serializer.SerializeFixedBytes(this._key_bytes);

        public override bool Equals(object obj)
        {
            if (obj is AccountAddressBase account_address)
                return account_address.KeyBytes.SequenceEqual(this.KeyBytes);

            return false;
        }

        public static bool operator ==(AccountAddressBase lhs, AccountAddressBase rhs)
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

        public static bool operator !=(AccountAddressBase lhs, AccountAddressBase rhs)
            => !(lhs == rhs);

        public override int GetHashCode() => this.KeyHex.GetHashCode();

        public override string ToString() => this.KeyHex;
    }
}