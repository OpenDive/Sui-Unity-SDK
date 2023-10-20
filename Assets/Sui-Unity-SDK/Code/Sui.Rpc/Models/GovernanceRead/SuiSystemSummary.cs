using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Cryptography.Ed25519;
using static Sui.Rpc.Models.ValidatorsApy;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "coinType": "0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC",
    ///         "coinObjectCount": 15,
    ///         "totalBalance": "15",
    ///         "lockedBalance": {}
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class SuiSystemSummary
    {
        // Active Validators
        // AtRirskValidators

        [JsonProperty("epoch")]
        public BigInteger Epoch;

        [JsonProperty("epochDurationMs")]
        public BigInteger EpochDurationMs;

        [JsonProperty("epochStartTimestampMs")]
        public BigInteger EpochStartTimestampMs;

        [JsonProperty("inactivePoolsId"), JsonConverter(typeof(AccountAddress))]
        public AccountAddress InactivePoolsId;

        [JsonProperty("inactivePoolsSize")]
        public BigInteger InactivePoolsSize;

        [JsonProperty("maxValidatorCount")]
        public BigInteger MaxValidatorCount;

        [JsonProperty("minValidatorJoiningStake")]
        public BigInteger MinValidatorJoiningStake;

        [JsonProperty("protocolVersion")]
        public BigInteger ProtocolVersion;

        [JsonProperty("referenceGasPrice")]
        public BigInteger ReferenceGasPrice;

        [JsonProperty("safeMode")]
        public bool SafeMode;

        [JsonProperty("safeModeComputationRewards")]
        public BigInteger SafeModeComputationRewards;

        [JsonProperty("safeModeNonRefundableStorageFee")]
        public BigInteger SafeModeNonRefundableStorageFee;

        [JsonProperty("safeModeStorageRebates")]
        public BigInteger SafeModeStorageRebates;

        [JsonProperty("safeModeStorageRewards")]
        public BigInteger SafeModeStorageRewards;

        [JsonProperty("stakeSubsidyBalance")]
        public BigInteger StakeSubsidyBalance;

        [JsonProperty("stakeSubsidyCurrentDistributionAmount")]
        public BigInteger StakeSubsidyCurrentDistributionAmount;

        [JsonProperty("stakeSubsidyDecreaseRate")]
        public UInt16 StakeSubsidyDecreaseRate;

        [JsonProperty("stakeSubsidyDistributionCounter")]
        public BigInteger StakeSubsidyDistributionCounter;

        [JsonProperty("stakeSubsidyPeriodLength")]
        public BigInteger StakeSubsidyPeriodLength;

        [JsonProperty("stakeSubsidyStartEpoch")]
        public BigInteger StakeSubsidyStartEpoch;

        [JsonProperty("stakingPoolMappingsId"), JsonConverter(typeof(SuiAddressJsonConverter))]
        public AccountAddress StakingPoolMappingsId;

        [JsonProperty("stakingPoolMappingsSize")]
        public BigInteger StakingPoolMappingsSize;

        [JsonProperty("storageFundNonRefundableBalance")]
        public BigInteger StorageFundNonRefundableBalance;

        [JsonProperty("storageFundTotalObjectStorageRebates")]
        public BigInteger StorageFundTotalObjectStorageRebates;

        [JsonProperty("systemStateVersion")]
        public BigInteger SystemStateVersion;

        [JsonProperty("totalStake")]
        public BigInteger TotalStake;

        [JsonProperty("validatorCandidatesId"), JsonConverter(typeof(SuiAddressJsonConverter))]
        public AccountAddress ValidatorCandidatesId;

        [JsonProperty("validatorCandidatesSize")]
        public BigInteger ValidatorCandidatesSize;

        [JsonProperty("validatorLowStakeGracePeriod")]
        public BigInteger ValidatorLowStakeGracePeriod;

        [JsonProperty("validatorLowStakeThreshold")]
        public BigInteger ValidatorLowStakeThreshold;

        [JsonProperty("validatorReportRecords"), JsonConverter(typeof(SuiAddressJsonConverter))]
        public AccountAddress ValidatorReportRecords;

        [JsonProperty("validatorVeryLowStakeThreshold")]
        public BigInteger ValidatorVeryLowStakeThreshold;

        [JsonObject]
        public class SuiValidatorSummary
        {
            [JsonProperty("commissionRate")]
            public BigInteger CommissionRate;

            [JsonProperty("description")]
            public string Description;

            /// <summary>
            /// ID of the exchange rate table object.
            /// </summary>
            [JsonProperty("exchangeRatesId"), JsonConverter(typeof(AccountAddress))]
            public AccountAddress ExchangeRatesId; // ObjectID

            [JsonProperty("exchangeRatesSize")]
            public BigInteger ExchangeRatesSize;

            [JsonProperty("gasPrice")]
            public BigInteger GasPrice;

            [JsonProperty("imageUrl")]
            public string ImageUrl;

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("netAddress")]
            public string NetAddress;

            [JsonProperty("networkPubkeyBytes")]
            public PublicKey NetworkPubkeyBytes;

            [JsonProperty("nextEpochCommissionRate")]
            public BigInteger NextEpochCommissionRate;

            [JsonProperty("operationCapId"), JsonConverter(typeof(SuiAddressJsonConverter))]
            public AccountAddress OperationCapId; // ObjectID TODO: Look into this - dJZOTBVNNWT1ga7uHIyb7sqeKyUtD5MeqYLxSFcUXJk=

            [JsonProperty("p2pAddress")]
            public string P2pAddress;

            /// <summary>
            /// Pending pool token withdrawn during the current epoch, emptied at epoch boundaries. 
            /// </summary>
            [JsonProperty("pendingPoolTokenWithdraw")]
            public BigInteger PendingPoolTokenWithdraw;

            /// <summary>
            /// Pending stake amount for this epoch.
            /// </summary>
            [JsonProperty("pendingStake")]
            public BigInteger PendingStake;

            /// <summary>
            /// Pending stake withdrawn during the current epoch, emptied at epoch boundaries. 
            /// </summary>
            [JsonProperty("pendingTotalSuiWithdraw")]
            public BigInteger PendingTotalSuiWithdraw;

            /// <summary>
            /// Total number of pool tokens issued by the pool.
            /// </summary>
            [JsonProperty("poolTokenBalance")]
            public BigInteger PoolTokenBalance;

            [JsonProperty("primaryAddress")]
            public string PrimaryAddress;

            [JsonProperty("projectUrl")]
            public string ProjectUrl;

            [JsonProperty("proofOfPossessionBytes")]
            public string ProofOfPossessionBytes;

            [JsonProperty("protocolPubkeyBytes")]
            public PublicKey ProtocolPubkeyBytes; // 124 string / 64 bytes TODO: Look into this - gHcc2rJLuqE2EX+S1nvErlvh2CtcMkoTVJieRgH/k8gNEDjQaZ87OkdgmaR1RQjqBKwk5wCiKcMrfjeAwcSnqI4EVKTuWw12Rj6qIhjCqulY+E3y0zCvsgd81MgqTzCs

            /// <summary>
            /// The epoch stake rewards will be added here at the end of each epoch.
            /// </summary>
            [JsonProperty("rewardsPool")]
            public BigInteger RewardsPool;

            /// <summary>
            /// The epoch at which this pool became active.
            /// </summary>
            [JsonProperty("stakingPoolActivationEpoch", NullValueHandling = NullValueHandling.Ignore)]
            public BigInteger StakingPoolActivationEpoch;

            /// <summary>
            /// The epoch at which this staking pool ceased to be active. `None` = {pre-active, active}, 
            /// </summary>
            [JsonProperty("stakingPoolDeactivationEpoch", NullValueHandling = NullValueHandling.Ignore)]
            public BigInteger StakingPoolDeactivationEpoch;

            [JsonProperty("stakingPoolId"), JsonConverter(typeof(SuiAddressJsonConverter))]
            public AccountAddress StakingPoolId; // ObjectID

            [JsonProperty("stakingPoolSuiBalance")]
            public BigInteger StakingPoolSuiBalance;

            [JsonProperty("suiAddress"), JsonConverter(typeof(SuiAddressJsonConverter))]
            public AccountAddress SuiAddress; // ObjectID

            [JsonProperty("votingPower")]
            public BigInteger VotingPower;

            [JsonProperty("workerAddress")]
            public string WorkerAddress;

            [JsonProperty("workerPubkeyBytes")]
            public PublicKey WorkerPubkeyBytes; // TODO: Look into this - 5zpGGKL8rAT+rvQaKpjiacy8qe9mYGg4sj+XEnBZw4c=
        }


        [JsonObject]
        public class VotingRights
        {
            [JsonProperty("authority_name")]
            // TODO: Ask about the type of the string returned
            //public AccountAddress AuthorityName;
            public string AuthorityName;

            // TODO: FROM SUI MYSTEN LABS DOCS: the stake and voting power of a validator can be different so
            // in some places when we are actually referring to the voting power, we
            // should use a different type alias, field name, etc.
            [JsonProperty("stake_unit")]
            public BigInteger StakeUnit;
        }


        private class VotingRightsConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                //return (objectType == typeof(ValidatorApy));
                // TODO: Implement
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                List<VotingRights> votingRightsList = new List<VotingRights>();

                JToken token = JToken.Load(reader);
                JArray votingRightsArr = token as JArray;

                foreach (JArray item in votingRightsArr.Cast<JArray>())
                {
                    // NOTE: We expect it to follow the response example provided in the RPC docs
                    // "validators": [
                    //      [
                    //          "jc/20VUECmVvSBmxMRG1LFdGqGunLzlfuv4uw4R9HoFA5iSnUf32tfIFC8cgXPnTAATJCwx0Cv/TJs5nPMKyOi0k1T4q/rKG38Zo/UBgCJ1tKxe3md02+Q0zLlSnozjU",
                    //          "2500"
                    //      ],
                    //      ...
                    // ]
                    string authorityName = (string)item[0];
                    BigInteger stakeUnit = BigInteger.Parse((string)item[1]);

                    VotingRights votingRights = new VotingRights();
                    // TODO: Ask about the type of the string returned
                    //votingRights.AuthorityName = AccountAddress.FromBase58((authorityName));
                    votingRights.AuthorityName = authorityName;
                    votingRights.StakeUnit = stakeUnit;

                    votingRightsList.Add(votingRights);
                }

                return votingRightsList.ToArray();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}