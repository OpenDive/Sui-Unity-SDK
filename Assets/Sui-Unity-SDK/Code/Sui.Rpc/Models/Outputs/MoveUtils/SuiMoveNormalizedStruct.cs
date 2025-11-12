//
//  SuiMoveNormalizedStruct.cs
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
    /// Represents a normalized Sui Move Struct, containing details about the struct including its abilities,
    /// type parameters, and fields.
    /// </summary>
    [JsonObject]
    public class SuiMoveNormalizedStruct
    {
        /// <summary>
        /// A `SuiMoveAbilitySet` representing the abilities of the struct.
        /// </summary>
        [JsonProperty("abilities")]
        public SuiMoveAbilitySet Abilities { get; internal set; }

        /// <summary>
        /// A list of `SuiMoveStructTypeParameter` representing the type parameters of the struct.
        /// </summary>
        [JsonProperty("typeParameters")]
        public SuiMoveStructTypeParameter[] TypeParameters { get; internal set; }

        /// <summary>
        /// A list of `SuiMoveNormalizedField` representing the fields within the struct.
        /// </summary>
        [JsonProperty("fields")]
        public SuiMoveNormalizedField[] Fields { get; internal set; }
    }
}