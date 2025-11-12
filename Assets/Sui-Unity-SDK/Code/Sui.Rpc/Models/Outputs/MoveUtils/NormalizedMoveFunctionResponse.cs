//
//  NormalizedMoveFunctionResponse.cs
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

using System.Linq;
using Newtonsoft.Json;
using Sui.Transactions;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a normalized function within a Sui Move entity.
    /// </summary>
    [JsonObject]
    public class NormalizedMoveFunctionResponse
    {
        /// <summary>
        /// Defines the visibility of the function (e.g. public, private).
        /// </summary>
        [JsonProperty("visibility")]
        public string Visibility { get; internal set; }

        /// <summary>
        /// Indicates whether the function is an entry point to its containing entity.
        /// </summary>
        [JsonProperty("isEntry")]
        public bool IsEntry { get; internal set; }

        /// <summary>
        /// Holds the ability sets for the function's type parameters, detailing the capabilities of those types.
        /// </summary>
        [JsonProperty("typeParameters")]
        public TypeParameter[] TypeParameters { get; internal set; }

        /// <summary>
        /// Represents the types of the parameters that the function can receive.
        /// </summary>
        [JsonProperty("parameters")]
        public SuiMoveNormalizedType[] Parameters { get; internal set; }

        /// <summary>
        /// Represents the types of the values that the function can return.
        /// </summary>
        [JsonProperty("return")]
        public SuiMoveNormalizedType[] Return { get; internal set; }

        /// <summary>
        /// Returns whether or not that this object contains a TxContext parameter or not.
        /// </summary>
        /// <returns>A `bool` value indiciating whether or not the function parameter has a TxContext.</returns>
        public bool HasTxContext()
        {
            if (this.Parameters.Length == 0)
                return false;

            SuiMoveNormalizedType possibly_tx_context = this.Parameters.Last();
            SuiMoveNormalziedTypeStruct struct_tag = NormalizedUtilities.ExtractStructType(possibly_tx_context);

            if (struct_tag == null)
                return false;

            return
                struct_tag.Struct.Address.ToHex() == Utils.NormalizeSuiAddress("0x2") &&
                struct_tag.Struct.Module == "tx_context" &&
                struct_tag.Struct.Name == "TxContext";
        }
    }
}