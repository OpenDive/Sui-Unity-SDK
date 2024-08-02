//
//  ValidatorApy.cs
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
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents the Annual Percentage Yield (APY) related information for a validator.
    /// </summary>
    [JsonObject]
    public class ValidatorApy : ReturnBase
    {
        /// <summary>
        /// An `AccountAddress` representing the address of the validator.
        /// </summary>
        [JsonProperty("address")]
        public AccountAddress Address { get; internal set; }

        /// <summary>
        /// A `ulong` value representing the Annual Percentage Yield (APY) of the validator.
        /// The APY is a percentage that indicates the profitability of the validator on an annual basis,
        /// allowing delegators to assess the potential return on their staked assets.
        /// </summary>
        [JsonProperty("apy")]
        public double APY { get; internal set; }
    }
}