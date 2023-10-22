using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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