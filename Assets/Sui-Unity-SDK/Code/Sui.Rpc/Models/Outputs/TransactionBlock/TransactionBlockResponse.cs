//
//  TransactionBlockResponse.cs
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
    /// The resulting values after execution of the transaction block.
    /// </summary>
    [JsonObject]
    public class TransactionBlockResponse
    {
        /// <summary>
        /// An optional `TransactionBlockEffects` representing the effects of the transaction.
        /// </summary>
        [JsonProperty("effects", NullValueHandling = NullValueHandling.Include)]
        public SuiTransactionBlockEffects Effects { get; internal set; }

        /// <summary>
        /// An optional array of `Event` objects representing the events occurred during the transaction.
        /// </summary>
        [JsonProperty("events", NullValueHandling = NullValueHandling.Include)]
        public Event[] Events { get; internal set; }

        /// <summary>
        /// An optional array of `ObjectChange` representing the object changes occurred during the transaction.
        /// </summary>
        [JsonProperty("objectChanges", NullValueHandling = NullValueHandling.Include)]
        public ObjectChange[] ObjectChanges { get; internal set; }

        /// <summary>
        /// An optional array of `BalanceChange` representing the balance changes occurred during the transaction.
        /// </summary>
        [JsonProperty("balanceChanges", NullValueHandling = NullValueHandling.Include)]
        public BalanceChange[] BalanceChanges { get; internal set; }

        /// <summary>
        /// An optional `string` representing the timestamp of the transaction block response in milliseconds.
        /// </summary>
        [JsonProperty("timestampMs", NullValueHandling = NullValueHandling.Include)]
        public string TimestampMs { get; internal set; }

        /// <summary>
        /// A `string` representing the digest of the transaction block response.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        /// <summary>
        /// An optional `SuiTransactionBlock` representing the transaction block in the response.
        /// </summary>
        [JsonProperty("transaction", NullValueHandling = NullValueHandling.Include)]
        public SuiTransactionBlock Transaction { get; internal set; }
    }
}