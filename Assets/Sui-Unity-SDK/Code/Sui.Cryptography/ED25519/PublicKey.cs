//
//  PublicKey.cs
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

using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography.Ed25519
{
    /// <summary>
    /// Implements an ED25519 public key functionality.
    /// </summary>
    public class PublicKey : SuiPublicKeyBase
    {
        public override SignatureScheme SignatureScheme { get => SignatureScheme.ED25519; }

        public override int KeyLength { get => SignatureSchemeToSize.ED25519; }

        public PublicKey(byte[] public_key) : base(public_key) { }

        public PublicKey(string public_key) : base(public_key) { }

        public override byte Flag() => SignatureSchemeToFlag.ED25519;

        public override bool Verify(byte[] message, byte[] signature)
            => Chaos.NaCl.Ed25519.Verify(signature, message, this._key_bytes);
    }
}