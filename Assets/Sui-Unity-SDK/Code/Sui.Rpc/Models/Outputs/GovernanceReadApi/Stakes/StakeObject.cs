//
//  StakeObject.cs
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
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a stake object, detailing information regarding a staking process or state,
    /// and conforming to the `Equatable` protocol to allow comparisons between instances.
    /// </summary>
    public abstract class StakeObject : ReturnBase
    {
        /// <summary>
        /// A `BigInteger` representing the principal amount within the stake
        /// itself, represented by SUI.
        /// </summary>
        [JsonProperty("principal")]
        public BigInteger Principal { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the epoch in which the stake became active.
        /// </summary>
        [JsonProperty("stakeActiveEpoch")]
        public BigInteger StakeActiveEpoch { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the epoch in which the stake request was made.
        /// </summary>
        [JsonProperty("stakeRequestEpoch")]
        public BigInteger StakeRequestEpoch { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the unique identifier associated with this stake in the Sui
        /// Blockchain, enabling reference and lookup of this specific stake instance.
        /// </summary>
        [JsonProperty("stakedSuiId")]
        public AccountAddress StakedSuiID { get; internal set; }

        /// <summary>
        /// An optional `BigInteger` representing the estimated reward amount for the given stake.
        /// </summary>
        [JsonProperty("estimatedReward", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? EstimatedReward { get; internal set; }

        public StakeObject
        (
            BigInteger principal,
            BigInteger stake_active_epoch,
            BigInteger stake_request_epoch,
            AccountAddress staked_sui_id,
            BigInteger? estimated_reward
        )
        {
            this.Principal = principal;
            this.StakeActiveEpoch = stake_active_epoch;
            this.StakeRequestEpoch = stake_request_epoch;
            this.StakedSuiID = staked_sui_id;
            this.EstimatedReward = estimated_reward;
        }
    }
}