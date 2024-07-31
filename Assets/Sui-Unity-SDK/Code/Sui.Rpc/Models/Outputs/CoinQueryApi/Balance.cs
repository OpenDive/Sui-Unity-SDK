//
//  Balance.cs
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
using Sui.Types;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents the balance details of a specific coin type in a user's account or wallet.
    /// </summary>
    [JsonObject]
    public class Balance
    {
        /// <summary>
        /// A `SuiStructTag` that represents the type of coin.
        /// This could be the name or the identifier of the coin / cryptocurrency.
        /// </summary>
        [JsonProperty("coinType")]
        public SuiStructTag CoinType { get; internal set; }

        /// <summary>
        /// An integer representing the count of coin objects within the specified coin type.
        /// Coin objects are instances or units of the coin type.
        /// </summary>
        [JsonProperty("coinObjectCount")]
        public BigInteger CoinObjectCount { get; internal set; }

        /// <summary>
        /// A string representing the total balance of the coin type in the account or wallet.
        /// This is typically the sum of the values of all coin objects of this coin type.
        /// </summary>
        [JsonProperty("totalBalance")]
        public BigInteger TotalBalance { get; internal set; }

        /// <summary>
        /// An optional `LockedBalance` instance representing any locked balance of the coin type.
        /// Locked balance is the portion of the total balance that is restricted or not readily available for use.
        /// </summary>
        [JsonProperty("lockedBalance", NullValueHandling = NullValueHandling.Include)]
        public LockedBalance LockedBalance { get; internal set; }
    }
}