//
//  CryptographyKey.cs
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

using Sui.Utilities;
using System.Linq;

namespace Sui.Cryptography
{
    public abstract class CryptographyKey : ReturnBase
    {
        /// <summary>
        /// The lenght of the public key.
        /// </summary>
        public virtual int KeyLength { get; }

        /// <summary>
        /// Check whether the inputted Public Key is not null.
        /// </summary>
        /// <param name="public_key">The Public Key object.</param>
        /// <returns>A bool representing whether the Public Key is not null.</returns>
        protected internal bool CheckIfKeyIsNull(object public_key)
        {
            if (public_key == null)
            {
                this.SetError<SuiError>("Input key is null.", public_key);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether the inputted Public Key matches the key's length.
        /// </summary>
        /// <param name="public_key">The Public Key byte array.</param>
        /// <returns>A bool representing if the inputted Public Key matches the key's length.</returns>
        protected internal bool CheckIfKeyIsExactLength(byte[] public_key)
        {
            if (public_key.Length != this.KeyLength)
            {
                this.SetError<SuiError>($"Invalid key length: {public_key.Length}", public_key.ToArray());
                return false;
            }
            return true;
        }
    }
}
