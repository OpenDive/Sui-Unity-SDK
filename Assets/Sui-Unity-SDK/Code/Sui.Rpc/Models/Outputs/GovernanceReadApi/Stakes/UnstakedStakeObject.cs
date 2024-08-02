//
//  UnstakedStakeObject.cs
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
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The stake has been unstaked and is no longer participating.
    /// </summary>
    public class UnstakedStakeObject : StakeObject
    {
        public UnstakedStakeObject
        (
            BigInteger principal,
            BigInteger stake_active_epoch,
            BigInteger stake_request_epoch,
            AccountAddress staked_sui_id,
            BigInteger? estimated_reward
        ) :
        base
        (
            principal,
            stake_active_epoch,
            stake_request_epoch,
            staked_sui_id,
            estimated_reward
        )
        { }

        public override bool Equals(object obj)
        {
            if (obj is not UnstakedStakeObject)
                return this.SetError<bool, SuiError>(false, "Compared object is not an UnstakedStakeObject.", obj);

            UnstakedStakeObject other_stake_object_unstaked = (UnstakedStakeObject)obj;

            return
                this.Principal.Equals(other_stake_object_unstaked.Principal) &&
                this.StakeActiveEpoch.Equals(other_stake_object_unstaked.StakeActiveEpoch) &&
                this.StakeRequestEpoch.Equals(other_stake_object_unstaked.StakeRequestEpoch) &&
                this.StakedSuiID.Equals(other_stake_object_unstaked.StakedSuiID) &&
                this.EstimatedReward.Equals(other_stake_object_unstaked.EstimatedReward);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}