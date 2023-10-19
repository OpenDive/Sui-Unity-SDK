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
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "apys": [
    ///             {
    ///                 "address": "0xb7d1cb695b9491893f88a5ae1b9d4f235b3c7e00acf5386662fa062483ba507b",
    ///                 "apy": 0.06
    ///             },
    ///             {
    ///                 "address": "0x1e9e3039750f0a270f2e12441ad7f611a5f7fd0b2c4326c56b1fec231d73038d",
    ///                 "apy": 0.02
    ///             },
    ///             {
    ///                 "address": "0xba0f0885b97982f5fcac3ec6f5c8cae16743671832358f25bfacde706e528df4",
    ///                 "apy": 0.05
    ///             }
    ///         ],
    ///         "epoch": "420"
    ///     }
    /// }
    /// </code>
    /// </summary>
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