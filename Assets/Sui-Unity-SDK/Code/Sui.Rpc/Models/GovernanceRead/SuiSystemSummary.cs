using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Cryptography.Ed25519;

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
        /// <summary>
        /// The list of active validators in the current epoch.
        /// </summary>
        // ActiveValidators

        /// <summary>
        /// Map storing the number of epochs for which each validator has been below the low stake threshold.
        /// </summary>
        /// pub at_risk_validators: VecMap<SuiAddress, u64>,
        /// AtRiskValidators
        /// Array<Tuple<AccountAddress, BigInteger>>
        /// Map<AccountAddress, BigInteger>
        ///

        /// <summary>
        /// The current epoch ID, starting from 0.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch;

        /// <summary>
        /// The duration of an epoch, in milliseconds.
        /// </summary>
        [JsonProperty("epochDurationMs")]
        public BigInteger EpochDurationMs;

        /// <summary>
        /// Unix timestamp of the current epoch start.
        /// </summary>
        [JsonProperty("epochStartTimestampMs")]
        public BigInteger EpochStartTimestampMs;

        /// <summary>
        /// ID of the object that maps from a staking pool ID to the inactive validator that has that pool as its staking pool.
        /// </summary>
        [JsonProperty("inactivePoolsId"), JsonConverter(typeof(AccountAddressConverter))]
        public AccountAddress InactivePoolsId;

        /// <summary>
        /// Number of inactive staking pools.
        /// </summary>
        [JsonProperty("inactivePoolsSize")]
        public BigInteger InactivePoolsSize;

        /// <summary>
        /// Maximum number of active validators at any moment.
        /// We do not allow the number of validators in any epoch to go above this.
        /// </summary>
        [JsonProperty("maxValidatorCount")]
        public BigInteger MaxValidatorCount;

        /// <summary>
        /// Lower-bound on the amount of stake required to become a validator.
        /// </summary>
        [JsonProperty("minValidatorJoiningStake")]
        public BigInteger MinValidatorJoiningStake;

        /// <summary>
        /// ID of the object that contains the list of new validators that will join at the end of the epoch.
        /// </summary>
        [JsonProperty("pendingActiveValidatorsId")]
        public AccountAddress PendingActiveValidatorsId; // ObjectId

        [JsonProperty("pendingActiveValidatorsSize")]
        public BigInteger PendingActiveValidatorsSize;

        /// <summary>
        /// Removal requests from the validators. Each element is an index
        /// pointing to `active_validators`.
        /// </summary>
        [JsonProperty("pendingRemovals")]
        public List<BigInteger> PendingRemovals;

        /// <summary>
        /// The current protocol version, starting from 1.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public BigInteger ProtocolVersion;

        /// <summary>
        /// The reference gas price for the current epoch.
        /// </summary>
        [JsonProperty("referenceGasPrice")]
        public BigInteger ReferenceGasPrice;

        /// <summary>
        /// Whether the system is running in a downgraded safe mode due to a non-recoverable bug.
        /// This is set whenever we failed to execute advance_epoch, and ended up executing advance_epoch_safe_mode.
        /// It can be reset once we are able to successfully execute advance_epoch.
        /// </summary>
        [JsonProperty("safeMode")]
        public bool SafeMode;

        /// <summary>
        /// Amount of computation rewards accumulated (and not yet distributed) during safe mode.
        /// </summary>
        [JsonProperty("safeModeComputationRewards")]
        public BigInteger SafeModeComputationRewards;

        /// <summary>
        /// Amount of non-refundable storage fee accumulated during safe mode.
        /// </summary>
        [JsonProperty("safeModeNonRefundableStorageFee")]
        public BigInteger SafeModeNonRefundableStorageFee;

        /// <summary>
        /// Amount of storage rebates accumulated (and not yet burned) during safe mode.
        /// </summary>
        [JsonProperty("safeModeStorageRebates")]
        public BigInteger SafeModeStorageRebates;

        /// <summary>
        /// Amount of storage rewards accumulated (and not yet distributed) during safe mode.
        /// </summary>
        [JsonProperty("safeModeStorageRewards")]
        public BigInteger SafeModeStorageRewards;

        /// <summary>
        /// Balance of SUI set aside for stake subsidies that will be drawn down over time.
        /// </summary>
        [JsonProperty("stakeSubsidyBalance")]
        public BigInteger StakeSubsidyBalance;

        /// <summary>
        /// The amount of stake subsidy to be drawn down per epoch.
        /// This amount decays and decreases over time.
        /// </summary>
        [JsonProperty("stakeSubsidyCurrentDistributionAmount")]
        public BigInteger StakeSubsidyCurrentDistributionAmount;

        /// <summary>
        /// The rate at which the distribution amount decays at the end of each
        /// period. Expressed in basis points.
        /// </summary>
        [JsonProperty("stakeSubsidyDecreaseRate")]
        public UInt16 StakeSubsidyDecreaseRate;

        /// <summary>
        /// This counter may be different from the current epoch number if
        /// in some epochs we decide to skip the subsidy.
        /// </summary>
        [JsonProperty("stakeSubsidyDistributionCounter")]
        public BigInteger StakeSubsidyDistributionCounter;

        /// <summary>
        /// Number of distributions to occur before the distribution amount decays.
        /// </summary>
        [JsonProperty("stakeSubsidyPeriodLength")]
        public BigInteger StakeSubsidyPeriodLength;

        /// <summary>
        /// The starting epoch in which stake subsidies start being paid out.
        /// </summary>
        [JsonProperty("stakeSubsidyStartEpoch")]
        public BigInteger StakeSubsidyStartEpoch;

        /// <summary>
        /// ID of the object that maps from staking pool's ID to the sui address of a validator.
        /// </summary>
        [JsonProperty("stakingPoolMappingsId"), JsonConverter(typeof(SuiAddressJsonConverter))]
        public AccountAddress StakingPoolMappingsId;

        /// <summary>
        /// Number of staking pool mappings.
        /// </summary>
        [JsonProperty("stakingPoolMappingsSize")]
        public BigInteger StakingPoolMappingsSize;

        /// <summary>
        /// The non-refundable portion of the storage fund coming from storage reinvestment, non-refundable
        /// storage rebates and any leftover staking rewards.
        /// </summary>
        [JsonProperty("storageFundNonRefundableBalance")]
        public BigInteger StorageFundNonRefundableBalance;

        /// <summary>
        /// The storage rebates of all the objects on-chain stored in the storage fund.
        /// </summary>
        [JsonProperty("storageFundTotalObjectStorageRebates")]
        public BigInteger StorageFundTotalObjectStorageRebates;

        /// <summary>
        /// The current version of the system state data structure type.
        /// </summary>
        [JsonProperty("systemStateVersion")]
        public BigInteger SystemStateVersion;

        /// <summary>
        /// Total amount of stake from all active validators at the beginning of the epoch.
        /// </summary>
        [JsonProperty("totalStake")]
        public BigInteger TotalStake;

        /// <summary>
        /// ID of the object that stores preactive validators, mapping their addresses to their `Validator` structs.
        /// </summary>
        [JsonProperty("validatorCandidatesId"), JsonConverter(typeof(SuiAddressJsonConverter))]
        public AccountAddress ValidatorCandidatesId;

        /// <summary>
        /// Number of preactive validators.
        /// </summary>
        [JsonProperty("validatorCandidatesSize")]
        public BigInteger ValidatorCandidatesSize;

        /// <summary>
        /// A validator can have stake below `validator_low_stake_threshold`
        /// for this many epochs before being kicked out.
        /// </summary>
        [JsonProperty("validatorLowStakeGracePeriod")]
        public BigInteger ValidatorLowStakeGracePeriod;

        /// <summary>
        /// Validators with stake amount below `validator_low_stake_threshold` are considered to
        /// have low stake and will be escorted out of the validator set after being below this
        /// threshold for more than `validator_low_stake_grace_period` number of epochs.
        /// </summary>
        [JsonProperty("validatorLowStakeThreshold")]
        public BigInteger ValidatorLowStakeThreshold;

        /// <summary>
        /// A map storing the records of validator reporting each other.
        /// <code>
        /// "validatorReportRecords":[
        ///     [
        ///         "0x3b5664bb0f8bb4a8be77f108180a9603e154711ab866de83c8344ae1f3ed4695",
        ///         [
        ///             "0x43b8f743162704af85214b0d0159fbef11aae0e996a8e9eac7fafda7fc5bd5f2",
        ///             "0x0ae4b2b4ed34dd551a01a946e51c0c431726faf5568659560f76b31642588468",
        ///             "0xec73ec4d6b2a9403937b12ca625f7b3124c4459ff4e3caae6cf6376edefb9f3a"
        ///         ]
        ///     ],
        ///     ...
        /// ]
        /// </code>
        /// </summary>
        [JsonProperty("validatorReportRecords"), JsonConverter(typeof(ValidatorReportRecordsConverter))]
        public Dictionary<AccountAddress, List<AccountAddress>> ValidatorReportRecords;

        /// <summary>
        /// Validators with stake below `validator_very_low_stake_threshold` will be removed
        /// immediately at epoch change, no grace period.
        /// </summary>
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

            /// <summary>
            /// Number of exchange rates in the table.
            /// </summary>
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

            /// <summary>
            /// Base64-encoded string of an array of bytes represented proof of possession.
            /// </summary>
            [JsonProperty("proofOfPossessionBytes")]
            public string ProofOfPossessionBytes;

            /// <summary>
            /// Public key bytes as a base64-encoded string.
            /// </summary>
            [JsonProperty("protocolPubkeyBytes"), JsonConverter(typeof(PublicKeyConverter))]
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

            /// <summary>
            /// ID of the staking pool object.
            /// </summary>
            [JsonProperty("stakingPoolId"), JsonConverter(typeof(SuiAddressJsonConverter))]
            public AccountAddress StakingPoolId; // ObjectID

            /// <summary>
            /// The total number of SUI tokens in this pool.
            /// </summary>
            [JsonProperty("stakingPoolSuiBalance")]
            public BigInteger StakingPoolSuiBalance;

            /// <summary>
            /// Hex string of a byte array.
            /// </summary>
            [JsonProperty("suiAddress"), JsonConverter(typeof(SuiAddressJsonConverter))]
            public AccountAddress SuiAddress; // ObjectID

            [JsonProperty("votingPower")]
            public BigInteger VotingPower;

            [JsonProperty("workerAddress")]
            public string WorkerAddress;

            /// <summary>
            /// Public key bytes as a base64-encoded string.
            /// </summary>
            [JsonProperty("workerPubkeyBytes"), JsonConverter(typeof(PublicKeyConverter))]
            public PublicKey WorkerPubkeyBytes; // TODO: Look into this - 5zpGGKL8rAT+rvQaKpjiacy8qe9mYGg4sj+XEnBZw4c=
        }

        public class ValidatorReportRecordsConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {

                Dictionary<AccountAddress, List<AccountAddress>> validatorReportRecords
                    = new Dictionary<AccountAddress, List<AccountAddress>>();

                JToken token = JToken.Load(reader);
                JArray validatorReportRecordsArr = token as JArray;

                foreach (JArray item in validatorReportRecordsArr)
                {
                    AccountAddress address = AccountAddress.FromHex((string)item[0]);

                    List<AccountAddress> reportingToList = new List<AccountAddress>();
                    JArray reportingToArr = (JArray)item[1];

                    foreach (JToken reportingAddress in reportingToArr)
                    {
                        AccountAddress reportingToAddress
                            = AccountAddress.FromHex((string)reportingAddress);
                        reportingToList.Add(reportingToAddress);
                    }

                    validatorReportRecords.Add(address, reportingToList);
                }

                return validatorReportRecords;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}