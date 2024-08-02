//
//  PrivateKey.cs
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
using NBitcoin;
using Sui.Rpc.Client;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography.Ed25519
{
    /// <summary>
    /// Implements the functionality of a private key.
    /// The key may be viewed as a Hex or Base64 string as supported by Sui.
    /// </summary>
    public class PrivateKey : SuiPrivateKeyBase
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
                // decode the string based on the key format (i.e. Hex or Base64)
                if (this._key_bytes == null && this._extended_key_bytes == null)
                {
                    if (this._key_hex != null)
                    {
                        string key = this._key_hex;
                        if (_key_hex[0..2].Equals("0x")) key = this._key_hex[2..]; // Trim the private key hex string

                        byte[] seed = key.HexStringToByteArray(); // Turn private key hex string into byte to be used a seed to derive the extended key
                        this._key_bytes = seed;
                    }
                    else // _keyBase64 is not null
                    {
                        string key = this._key_base64;
                        byte[] seed = CryptoBytes.FromBase64String(key);
                        this._key_bytes = seed;
                    }
                    this._extended_key_bytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(this._key_bytes);
                }
                return this._key_bytes;
            }
            set
            {
                if (value.Length != this.KeyLength)
                {
                    this.SetError<SuiError>($"Invalid key length: {value.Length}");
                    return;
                }

                this._key_bytes = value;
                this._extended_key_bytes = Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(value);
            }
        }

        public PrivateKey(byte[] private_key) : base(private_key) => this.SetExpandedKey();

        public PrivateKey(ReadOnlySpan<byte> private_key) : base(private_key) => this.SetExpandedKey();

        public PrivateKey(string private_key) : base(private_key) => this.SetExpandedKey();

        public PrivateKey() : base(PrivateKey.GetRandomSeed()) => this.SetExpandedKey();

        public override PublicKeyBase PublicKey()
            => new PublicKey(Chaos.NaCl.Ed25519.PublicKeyFromSeed(this.KeyBytes));

        public override SignatureBase Sign(byte[] message)
        {
            ArraySegment<byte> signature = new ArraySegment<byte>(new byte[PrivateKeyBase.SignatureLength]);

            Chaos.NaCl.Ed25519.Sign
            (
                signature,
                new ArraySegment<byte>(message),
                new ArraySegment<byte>(this._extended_key_bytes)
            );

            return new Signature(signature.Array);
        }

        public override SignatureBase Sign(string b64_message)
            => (Signature)this.Sign(CryptoBytes.FromBase64String(b64_message));

        // TODO: Remove NBitcoin dependency
        /// <summary>
        /// Get a random seed for the given private key.
        /// </summary>
        /// <returns>A byte array representing the randomized seed.</returns>
        private static byte[] GetRandomSeed()
        {
            byte[] seed = new byte[Chaos.NaCl.Ed25519.PrivateKeySeedSizeInBytes];
            RandomUtils.GetBytes(seed);
            return seed;
        }

        /// <summary>
        /// Set the expanded private key using this.KeyBytes
        /// </summary>
        private void SetExpandedKey()
        {
            if (this.Error != null)
                return;

            this._extended_key_bytes = new byte[Chaos.NaCl.Ed25519.ExpandedPrivateKeySizeInBytes];
            Array.Copy(
                Chaos.NaCl.Ed25519.ExpandedPrivateKeyFromSeed(this.KeyBytes),
                this._extended_key_bytes,
                Chaos.NaCl.Ed25519.ExpandedPrivateKeySizeInBytes
            );
        }
    }
}