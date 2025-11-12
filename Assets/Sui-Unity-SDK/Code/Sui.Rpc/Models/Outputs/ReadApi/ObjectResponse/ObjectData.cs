//
//  ObjectData.cs
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

using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Types;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A structure representing ObjectData,
    /// containing various information about an object on the Sui Network.
    /// </summary>
    [JsonObject]
    public class ObjectData
    {
        /// <summary>
        /// A `string` representing the object ID.
        /// </summary>
        [JsonProperty("objectId")]
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the version of the object.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? Version { get; internal set; }

        /// <summary>
        /// A `string` representing the digest of the object.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        /// <summary>
        /// An optional `string` representing the type of the object.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Include)]
        public SuiStructTag Type { get; internal set; }

        /// <summary>
        /// An optional `Owner` representing the owner of the object.
        /// </summary>
        [JsonProperty("owner", NullValueHandling = NullValueHandling.Include)]
        public Owner Owner { get; internal set; }

        /// <summary>
        /// An optional `string` representing the previous transaction of the object.
        /// </summary>
        [JsonProperty("previousTransaction", NullValueHandling = NullValueHandling.Include)]
        public string PreviousTransaction { get; internal set; }

        /// <summary>
        /// An optional `BigInteger` representing the storage rebate of the object.
        /// </summary>
        [JsonProperty("storageRebate", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? StorageRebate { get; internal set; }

        /// <summary>
        /// An optional `Data` representing the parsed data of the object.
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Include)]
        public Data Content { get; internal set; }

        /// <summary>
        /// An optional `RawData` representing the Binary Canonical Serialization (BCS) of the object.
        /// </summary>
        [JsonProperty("bcs", NullValueHandling = NullValueHandling.Include)]
        public RawData BCS { get; internal set; }

        /// <summary>
        /// An optional `DisplayFieldsResponse` representing the display fields of the object.
        /// </summary>
        [JsonProperty("display", NullValueHandling = NullValueHandling.Include)]
        public DisplayFieldsResponse Display { get; internal set; }

        public ObjectData
        (
            AccountAddress object_id,
            string digest,
            BigInteger? version = null,
            SuiStructTag type = null,
            Owner owner = null,
            string previous_transaction = null,
            BigInteger? storage_rebate = null,
            RawData bcs = null,
            Data content = null,
            DisplayFieldsResponse display = null
        )
        {
            this.ObjectID = object_id;
            this.Digest = digest;
            this.Version = version;
            this.Type = type;
            this.Owner = owner;
            this.PreviousTransaction = previous_transaction;
            this.StorageRebate = storage_rebate;
            this.BCS = bcs;
            this.Content = content;
            this.Display = display;
        }
    }
}