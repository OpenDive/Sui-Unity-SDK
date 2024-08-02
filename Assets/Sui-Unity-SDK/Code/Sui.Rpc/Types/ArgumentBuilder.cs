//
//  ArgumentBuilder.cs
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

using System.Collections.Generic;

namespace Sui.Rpc
{
    /// <summary>
    /// Represents a builder class to build out RPC arguments.
    /// </summary>
    public static class ArgumentBuilder
    {
        /// <summary>
        /// Build an RPC argument using a given set of parameters.
        /// </summary>
        /// <param name="params">The array of parameters.</param>
        /// <returns>An `IEnumerable` object representing the built arguments</returns>
        public static IEnumerable<object> BuildArguments(params object[] @params)
        {
            return @params;
        }

        /// <summary>
        /// Build an RPC argument using a given set of parameters.
        /// </summary>
        /// <param name="params">The array of parameters.</param>
        /// <returns>An `IEnumerable` object representing the built arguments</returns>
        public static IEnumerable<string> BuildTypeArguments(params string[] @params)
        {
            return @params;
        }
    }
}