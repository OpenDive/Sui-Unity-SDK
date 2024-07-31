//
//  CoinMetadata.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents metadata associated with a Sui Coin (0x2::sui::SUI), providing details and information
    /// related to a specific coin in the system.
    /// </summary>
    [JsonObject]
    public class CoinMetadata
    {
        /// A `BigInteger` value representing the number of decimals to which the coin can be divided.
        /// This attribute is useful for representing fractional values of the coin.
        [JsonProperty("decimals")]
        public BigInteger Decimals { get; internal set; }

        /// A `string` representing the name of the coin. This is typically a full or formal name
        /// that is used to refer to the coin in official or detailed contexts.
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// A `string` representing the symbol of the coin. This is usually a short, concise representation
        /// used for quick reference or display, such as in trading pairs or price listings.
        [JsonProperty("symbol")]
        public string Symbol { get; internal set; }

        /// A `string` providing a human-readable description of the coin, explaining its purpose,
        /// usage, or any other relevant information that helps in understanding the coin's context.
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// An optional `string` containing the URL of the coin's icon. This URL can be used to
        /// retrieve the visual representation or logo associated with the coin.
        [JsonProperty("iconUrl", NullValueHandling = NullValueHandling.Include)]
        public string IconURL { get; internal set; }

        /// A `string` serving as a unique identifier for the coin, allowing for differentiation and
        /// reference to this specific coin instance within the system.
        [JsonProperty("id")]
        public AccountAddress ID { get; internal set; }
    }
}
