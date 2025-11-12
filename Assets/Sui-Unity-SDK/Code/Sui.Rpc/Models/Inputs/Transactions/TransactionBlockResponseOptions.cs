//
//  TransactionBlockResponseOptions.cs
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
    /// A class representing the various return options for `TransactionBlockResponse`.
    /// </summary>
    [JsonObject]
    public class TransactionBlockResponseOptions
    {
        /// <summary>
        /// Indicates whether to show the input in the transaction block response.
        /// </summary>
        [JsonProperty("showInput")]
        public bool ShowInput { get; set; }

        /// <summary>
        /// Indicates whether to show the effects in the transaction block response.
        /// </summary>
        [JsonProperty("showEffects")]
        public bool ShowEffects { get; set; }

        /// <summary>
        /// Indicates whether to show the events in the transaction block response.
        /// </summary>
        [JsonProperty("showEvents")]
        public bool ShowEvents { get; set; }

        /// <summary>
        /// Indicates whether to show the object changes in the transaction block response.
        /// </summary>
        [JsonProperty("showObjectChanges")]
        public bool ShowObjectChanges { get; set; }

        /// <summary>
        /// Indicates whether to show the balance changes in the transaction block response.
        /// </summary>
        [JsonProperty("showBalanceChanges")]
        public bool ShowBalanceChanges { get; set; }

        public TransactionBlockResponseOptions
        (
            bool showInput = false,
            bool showEffects = false,
            bool showEvents = false,
            bool showObjectChanges = false,
            bool showBalanceChanges = false
        )
        {
            this.ShowInput = showInput;
            this.ShowEffects = showEffects;
            this.ShowEvents = showEvents;
            this.ShowObjectChanges = showObjectChanges;
            this.ShowBalanceChanges = showBalanceChanges;
        }
    }
}
