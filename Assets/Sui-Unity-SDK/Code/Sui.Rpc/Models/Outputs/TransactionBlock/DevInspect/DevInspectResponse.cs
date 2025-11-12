//
//  DevInspectResponse.cs
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
    /// A class representing the results of Dev Inspect call.
    /// </summary>
    [JsonObject]
    public class DevInspectResponse
    {
        /// <summary>
        /// Summary of effects that likely would be generated if the
        /// transaction is actually run. Note however, that not all dev-
        /// inspect transactions are actually usable as transactions so it
        /// might not be possible actually generate these effects from a
        /// normal transaction.
        /// </summary>
        [JsonProperty("effects")]
        public SuiTransactionBlockEffects Effects { get; internal set; }

        /// <summary>
        /// Events that likely would be generated if the transaction is actually run.
        /// </summary>
        [JsonProperty("events")]
        public Event[] Events { get; internal set; }

        /// <summary>
        /// The raw effects of the transaction that was dev inspected.
        /// </summary>
        [JsonProperty("rawEffects", NullValueHandling = NullValueHandling.Include)]
        public string[] RawEffects { get; internal set; }

        /// <summary>
        /// The raw transaction data that was dev inspected.
        /// </summary>
        [JsonProperty("rawTxnData", NullValueHandling = NullValueHandling.Include)]
        public string[] RawTransactionData { get; internal set; }

        /// <summary>
        /// An optional list of `ExecutionResultType` representing the results of the execution.
        /// This will be `null` if there are no execution results to represent.
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Include)]
        public ExecutionResultType[] Results { get; internal set; }

        /// <summary>
        /// An optional `string` representing any error that occurred during the inspection.
        /// This will be `null` if there is no error to report.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Include)]
        public string Error { get; internal set; }
    }
}