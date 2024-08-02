//
//  AccountBase.cs
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

using Sui.Cryptography;
using Sui.Utilities;

namespace Sui.Accounts
{
    public abstract class AccountBase<T, U> : ReturnBase
        where T : PrivateKeyBase
        where U : PublicKeyBase
    {
        /// <summary>
        /// Represents a `PrivateKey` object.
        /// </summary>
        public T PrivateKey { get; set; }

        /// <summary>
        /// Represents a `PublicKey` object.
        /// </summary>
        public U PublicKey { get; set; }

        public AccountBase() { }

        public AccountBase
        (
            T private_key,
            U public_key
        )
        {
            this.PrivateKey = private_key;
            this.PublicKey = public_key;
        }

        /// <summary>
        /// Sign an arbitrary byte array.
        /// </summary>
        /// <param name="message">The byte array representation of the message.</param>
        /// <returns>A `SignatureBase` object representing the signature.</returns>
        public SignatureBase Sign(byte[] message) => this.PrivateKey.Sign(message);

        /// <summary>
        /// Verify whether the inputted byte array signature was
        /// comitted given the inputted byte array message.
        /// </summary>
        /// <param name="message">The byte array of the message.</param>
        /// <param name="signature">The `SignatureBase` that represents the signature.</param>
        /// <returns>A bool representing whether the inputted message corresponds to the committed signature.</returns>
        public bool Verify(byte[] message, SignatureBase signature)
            => this.PublicKey.Verify(message, signature.SignatureBytes);
    }
}