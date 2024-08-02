//
//  SuiResult.cs
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

namespace Sui.Utilities
{
    /// <summary>
    /// Represents a return value within the Sui SDK.
    /// </summary>
    /// <typeparam name="T">Any given return value.</typeparam>
    public class SuiResult<T> : ResultBase<T>
    {
        public SuiResult(T result, ErrorBase error = null) : base(result, error) { }

        /// <summary>
        /// Return an error Sui result.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <returns>A `SuiResult<T>` object with an error object.</returns>
        public static SuiResult<T> GetSuiErrorResult(string message)
            => new SuiResult<T>(default, new SuiError(-1, message, null));
    }
}