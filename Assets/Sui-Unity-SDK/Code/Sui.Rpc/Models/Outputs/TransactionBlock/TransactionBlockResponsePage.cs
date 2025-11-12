//
//  TransactionBlockResponsePage.cs
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
    /// Represents a paginated response containing a list of `TransactionBlockResponse` instances,
    /// which represent transaction blocks retrieved from the Sui blockchain, along with
    /// pagination information.
    /// </summary>
    [JsonObject]
    public class TransactionBlockResponsePage
    {
        /// <summary>
        /// A list of `TransactionBlockResponse` instances, each representing the response
        /// for a specific transaction block.
        /// </summary>
        [JsonProperty("data")]
        public TransactionBlockResponse[] Data { get; internal set; }

        /// <summary>
        /// An optional string representing the cursor for the next page of transaction blocks.
        /// `null` if there are no more pages available.
        /// </summary>
        [JsonProperty("nextCursor")]
        public string NextCursor { get; internal set; }

        /// <summary>
        /// A `bool` value indicating whether there are more pages of transaction blocks available to be retrieved.
        /// `true` if there are more pages available, otherwise `false`.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }
}