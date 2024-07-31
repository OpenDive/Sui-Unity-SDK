//
//  DelegatedStake.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A class representing the information of delegated stakes.
    /// </summary>
    [JsonObject]
    public class DelegatedStake
    {
        /// <summary>
        /// An `AccountAddress` representing the address of the validator managing the delegated stakes.
        /// </summary>
        [JsonProperty("validatorAddress")]
        public AccountAddress ValidatorAddress { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the staking pool to which the stakes are delegated.
        /// </summary>
        [JsonProperty("stakingPool")]
        public AccountAddress StakingPool { get; internal set; }

        /// <summary>
        /// A list of `StakeStatus` representing the status of each stake delegated.
        /// </summary>
        [JsonProperty("stakes")]
        public StakeStatus[] Stakes { get; internal set; }
    }
}