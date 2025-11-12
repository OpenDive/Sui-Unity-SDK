//
//  TransactionMoveFunction.cs
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

using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Move Module used for transactions.
    /// </summary>
    [JsonObject]
    public class TransactionMoveFunction
    {
        /// <summary>
        /// The function name.
        /// </summary>
        [JsonProperty("function", NullValueHandling = NullValueHandling.Include)]
        public string Function { get; set; }

        /// <summary>
        /// The module name.
        /// </summary>
        [JsonProperty("module", NullValueHandling = NullValueHandling.Include)]
        public string Module { get; set; }

        /// <summary>
        /// The package name
        /// </summary>
        [JsonProperty("package", NullValueHandling = NullValueHandling.Include)]
        public string Package { get; set; }

        public TransactionMoveFunction
        (
            string function = null,
            string module = null,
            string package = null
        )
        {
            this.Function = function;
            this.Module = module;
            this.Package = package;
        }
    }
}