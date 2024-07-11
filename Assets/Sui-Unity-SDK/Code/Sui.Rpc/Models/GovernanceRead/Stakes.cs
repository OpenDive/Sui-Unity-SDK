using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using UnityEngine;


namespace Sui.Rpc.Models
{
    [JsonObject]
    public class Stakes
    {
        [JsonProperty("validatorAddress"), JsonConverter(typeof(AccountAddressConverter))]
        public AccountAddress ValidatorAddress;

        [JsonProperty("stakingPool"), JsonConverter(typeof(AccountAddressConverter))]
        public AccountAddress StakingPool;

        [JsonProperty("stakes")]
        public List<Stake> StakeList;

        [JsonObject]
        public class Stake
        {
            [JsonProperty("stakedSuiId"), JsonConverter(typeof(AccountAddressConverter))]
            public AccountAddress StakedSuiId;

            [JsonProperty("stakeRequestEpoch")]
            public BigInteger StakeRequestEpoch; // u64

            [JsonProperty("stakeActiveEpoch")]
            public BigInteger StakeActiveEpoch;

            [JsonProperty("principal")]
            public BigInteger Principal;

            [JsonProperty("status")]
            public StakeStatus status;

            [JsonProperty("estimatedReward")]
            public BigInteger EstimatedReward;

            public override bool Equals(object obj)
            {
                if (obj is not Stake)
                    throw new NotImplementedException();

                Stake other_stake = (Stake)obj;

                return
                    this.StakedSuiId.Equals(other_stake.StakedSuiId) &&
                    this.StakeRequestEpoch.Equals(other_stake.StakeRequestEpoch) &&
                    this.StakeActiveEpoch.Equals(other_stake.StakeActiveEpoch) &&
                    this.Principal.Equals(other_stake.Principal) &&
                    (int)this.status == (int)other_stake.status &&
                    this.EstimatedReward.Equals(other_stake.EstimatedReward);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum StakeStatus
        {
            Pending,
            Active,
            Unstaked
        }
    }
}