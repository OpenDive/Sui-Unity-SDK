//
//  Event.cs
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
using Newtonsoft.Json.Linq;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A move event that happens on the Sui Network.
    /// </summary>
	[JsonObject]
	public class Event
	{
        /// <summary>
        /// The unique identifier of the event.
        /// </summary>
		[JsonProperty("id")]
		public EventId ID { get; internal set; }

        /// <summary>
        /// The identifier of the package associated with the event.
        /// </summary>
		[JsonProperty("packageId")]
		public AccountAddress PackageID { get; internal set; }

        /// <summary>
        /// A `string` representing the module associated with the transaction.
        /// </summary>
		[JsonProperty("transactionModule")]
		public string TransactionModule { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the sender of the event.
        /// </summary>
		[JsonProperty("sender")]
		public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// A `string` representing the type of the event.
        /// </summary>
		[JsonProperty("type")]
		public string Type { get; internal set; }

        /// <summary>
        /// A `JObject` object representing the parsed JSON associated with the event.
        /// </summary>
		[JsonProperty("parsedJson")]
		public JObject ParsedJson { get; internal set; }

        /// <summary>
        /// An optional `string` representing the BCS (Binary Canonical Serialization) of the event.
        /// </summary>
		[JsonProperty("bcs", NullValueHandling = NullValueHandling.Include)]
		public string BCS { get; internal set; }

        /// <summary>
        /// An optional `string` representing the timestamp of the event in milliseconds.
        /// </summary>
		[JsonProperty("timestampMs", NullValueHandling = NullValueHandling.Include)]
		public string TimestampMs { get; internal set; }
    }
}
