//
//  DynamicFieldInfo.cs
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
using System.Numerics;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A structure representing information about a dynamic field.
    /// </summary>
    [JsonObject]
    public class DynamicFieldInfo
    {
        /// <summary>
        /// An instance of `DynamicFieldName` representing the name of the dynamic field.
        /// </summary>
        [JsonProperty("name")]
        public DynamicFieldName Name { get; internal set; }

        /// <summary>
        /// A `string` representing the BCS (Binary Canonical Serialization) name of the dynamic field.
        /// </summary>
        [JsonProperty("bcsName")]
        public string BCSName { get; internal set; }

        /// <summary>
        /// An instance of `string` representing the type of the dynamic field.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; internal set; }

        /// <summary>
        /// A `string` representing the type of the object associated with the dynamic field.
        /// </summary>
        [JsonProperty("objectType")]
        public string ObjectType { get; internal set; }

        /// <summary>
        /// A `string` representing the object ID associated with the dynamic field.
        /// </summary>
        [JsonProperty("objectId")]
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the version of the dynamic field.
        /// </summary>
        [JsonProperty("version")]
        public BigInteger Version { get; internal set; }

        /// <summary>
        /// A `string` representing the digest of the dynamic field.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }
    }
}