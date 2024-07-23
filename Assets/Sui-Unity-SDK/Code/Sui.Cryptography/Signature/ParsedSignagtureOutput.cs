//
//  ParsedSignatureOutput.cs
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
    /// Models the result of parsing a base64 serialized signature
    /// </summary>
    public class ParsedSignatureOutput
    {
        /// <summary>
        /// The serialized signature represented as a base 64 string.
        /// </summary>
        public string SerializedSignature { get; set; }

        /// <summary>
        /// The serialized signature represented as a byte array.
        /// </summary>
        public byte[] SerializedSignatureBytes { get; set; }

        /// <summary>
        /// The signature represented in a byte array.
        /// </summary>
        public byte[] Signature { get; set; }

        /// <summary>
        /// The public key represented in a byte array.
        /// </summary>
        public byte[] PublicKey { get; set; }

        /// <summary>
        /// The signature scheme of the signature.
        /// </summary>
        public SignatureScheme SignatureScheme { get; set; }

        public ParsedSignatureOutput
        (
            string serialized_signature,
            byte[] serialized_signature_bytes,
            SignatureScheme signature_scheme,
            byte[] signature,
            byte[] public_key
        )
        {
            this.SerializedSignature = serialized_signature;
            this.SerializedSignatureBytes = serialized_signature_bytes;
            this.SignatureScheme = signature_scheme;
            this.Signature = signature;
            this.PublicKey = public_key;
        }
    }
}