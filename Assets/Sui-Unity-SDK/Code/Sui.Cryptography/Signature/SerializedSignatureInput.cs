//
//  SerializedSignatureInput.cs
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

namespace Sui.Cryptography
{
    /// <summary>
    /// Represents a Sui signature
    /// https://docs.sui.io/learn/cryptography/sui-signatures#user-signature
    /// </summary>
    public class SerializeSignatureInput
    {
        /// <summary>
        /// The signature scheme of the input.
        /// </summary>
        public SignatureScheme SignatureScheme { get; set; }

        /// <summary>
        /// The signature as a byte array.
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// The public key associated with the signature.
        /// </summary>
        public SuiPublicKeyBase PublicKey { get; set; }

        public SerializeSignatureInput
        (
            SignatureScheme signature_scheme,
            byte[] signature,
            SuiPublicKeyBase public_key
        )
        {
            this.SignatureScheme = signature_scheme;
            this.Signature = signature;
            this.PublicKey = public_key;
        }
    }
}