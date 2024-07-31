//
//  AccountAddress.cs
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

using Org.BouncyCastle.Crypto.Digests;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Cryptography;
using Sui.Rpc.Models;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Accounts
{
    /// <summary>
    /// Represents a Sui address associated with a give public key.
    /// 
    /// For deriving a 32-byte Sui address,
    /// Sui hashes the signature scheme flag 1-byte concatenated with public key bytes
    /// using the BLAKE2b (256 bits output) hashing function.
    ///
    /// Sui address currently supports pure Ed25519, Secp256k1, Secp256r1,
    /// and MultiSig with corresponding flag bytes of 0x00, 0x01, 0x02, and 0x03, respectively.
    /// https://docs.sui.io/learn/cryptography/sui-wallet-specs#address-format
    /// </summary>
    [JsonConverter(typeof(AccountAddressConverter))]
    public class AccountAddress : AccountAddressBase
    {
        public override int KeyLength { get => 32; }

        /// <summary>
        /// Signature scheme of the account address.
        /// </summary>
        protected SignatureScheme _signature_scheme;

        // TODO: Look into how to check if an enum property is set
        /// <summary>
        /// Signature scheme of the account address.
        /// </summary>
        public SignatureScheme SignatureScheme
        {
            get
            {
                if (this._signature_scheme <= 0)
                    this._signature_scheme = this.Flag();

                return this._signature_scheme;
            }
            set => this._signature_scheme = value;
        }

        public AccountAddress(byte[] account_address) : base(account_address) { }

        public AccountAddress(string account_address) : base(account_address) { }

        public AccountAddress() : base() { }

        public AccountAddress(ErrorBase error) : base(error) { }

        public AccountAddress(SuiPublicKeyBase public_key) : base(public_key.KeyBytes)
        {
            this.SignatureScheme = public_key.SignatureScheme;

            byte[] sui_bytes = public_key.ToSuiBytes();

            // BLAKE2b hash
            byte[] result = new byte[public_key.KeyLength * 2];
            Blake2bDigest blake2b = new Blake2bDigest(256);
            blake2b.BlockUpdate(sui_bytes, 0, sui_bytes.Length);
            blake2b.DoFinal(result, 0);

            this._key_bytes = result;
        }

        public static AccountAddress FromHex(string sui_address)
        {
            string address = Utils.NormalizeSuiAddress(sui_address);

            if (address == null)
                return new AccountAddress(new SuiError(0, "Address string is too long.", sui_address));

            address = address[2..];

            return new AccountAddress(address);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            AccountAddress account_address = new AccountAddress();
            if (deserializer.PeekByte() == account_address.KeyLength)
            {
                account_address.KeyBytes = deserializer.ToBytes();
                return account_address;
            }
            account_address.KeyBytes = deserializer.FixedBytes(32);
            return account_address;
        }

        /// <summary>
        /// Return the byte representation of the signature scheme.
        /// </summary>
        /// <returns>A byte value corresponding to the signature scheme.</returns>
        public SignatureScheme Flag() => SignatureFlagToScheme.GetScheme(this.KeyBytes[0]);
    }
}