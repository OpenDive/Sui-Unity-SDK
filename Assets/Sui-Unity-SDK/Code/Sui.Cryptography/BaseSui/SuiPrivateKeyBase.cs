//
//  SuiPrivateKeyBase.cs
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
using Org.BouncyCastle.Crypto.Digests;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    public abstract class SuiPrivateKeyBase : PrivateKeyBase
    {
        /// <summary>
        /// Signature scheme for the private key.
        /// </summary>
        public abstract SignatureScheme SignatureScheme { get; }

        protected SuiPrivateKeyBase(byte[] private_key) : base(private_key) { }

        protected SuiPrivateKeyBase(string private_key) : base(private_key) { }

        protected SuiPrivateKeyBase(ReadOnlySpan<byte> private_key) : base(private_key) { }

        /// <summary>
        /// Sign messages with a specific intent. By combining
        /// the message bytes with the intent before hashing and signing,
        /// it ensures that a signed message is tied to a specific purpose
        /// and domain separator is provided
        /// </summary>
        /// <param name="bytes">The intent message itself to sign.</param>
        /// <param name="intent">The intent scope of the message.</param>
        /// <returns>A `SignatureBase` object representing the signature.</returns>
        public SignatureBase SignWithIntent(byte[] bytes, IntentScope intent)
        {
            byte[] intent_message = SignatureUtils.CreateMessageWithIntent(intent, bytes);

            // BLAKE2b hash
            byte[] digest = new byte[32];
            Blake2bDigest blake2b = new Blake2bDigest(256);
            blake2b.BlockUpdate(intent_message, 0, intent_message.Length);
            blake2b.DoFinal(digest, 0);

            return this.Sign(digest);
        }
    }
}

