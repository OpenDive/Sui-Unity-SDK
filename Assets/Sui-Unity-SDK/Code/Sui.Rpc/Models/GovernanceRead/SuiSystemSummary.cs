using System;
using System.Linq;
using System.Numerics;
using Chaos.NaCl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A class that represents the state of the Sui Network.
    /// </summary>
    [JsonObject]
    public class SuiSystemSummary
    {
        /// <summary>
        /// The current epoch ID, starting from 0.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch { get; internal set; }

        /// <summary>
        /// The current protocol version, starting from 1.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public BigInteger ProtocolVersion { get; internal set; }

        /// <summary>
        /// The current version of the system state data structure type.
        /// </summary>
        [JsonProperty("systemStateVersion")]
        public BigInteger SystemStateVersion { get; internal set; }

        /// <summary>
        /// The storage rebates of all the objects on-chain stored in the storage fund.
        /// </summary>
        [JsonProperty("storageFundTotalObjectStorageRebates")]
        public BigInteger StorageFundTotalObjectStorageRebates { get; internal set; }

        /// <summary>
        /// The non-refundable portion of the storage fund coming from storage reinvestment, non-refundable
        /// storage rebates and any leftover staking rewards.
        /// </summary>
        [JsonProperty("storageFundNonRefundableBalance")]
        public BigInteger StorageFundNonRefundableBalance { get; internal set; }

        /// <summary>
        /// The reference gas price for the current epoch.
        /// </summary>
        [JsonProperty("referenceGasPrice")]
        public BigInteger ReferenceGasPrice { get; internal set; }

        /// <summary>
        /// Whether the system is running in a downgraded safe mode due to a non-recoverable bug.
        /// This is set whenever we failed to execute advance_epoch, and ended up executing advance_epoch_safe_mode.
        /// It can be reset once we are able to successfully execute advance_epoch.
        /// </summary>
        [JsonProperty("safeMode")]
        public bool SafeMode { get; internal set; }

        /// <summary>
        /// Amount of storage rewards accumulated (and not yet distributed) during safe mode.
        /// </summary>
        [JsonProperty("safeModeStorageRewards")]
        public BigInteger SafeModeStorageRewards { get; internal set; }

        /// <summary>
        /// Amount of computation rewards accumulated (and not yet distributed) during safe mode.
        /// </summary>
        [JsonProperty("safeModeComputationRewards")]
        public BigInteger SafeModeComputationRewards { get; internal set; }

        /// <summary>
        /// Amount of storage rebates accumulated (and not yet burned) during safe mode.
        /// </summary>
        [JsonProperty("safeModeStorageRebates")]
        public BigInteger SafeModeStorageRebates { get; internal set; }

        /// <summary>
        /// Amount of non-refundable storage fee accumulated during safe mode.
        /// </summary>
        [JsonProperty("safeModeNonRefundableStorageFee")]
        public BigInteger SafeModeNonRefundableStorageFee { get; internal set; }

        /// <summary>
        /// Unix timestamp of the current epoch start.
        /// </summary>
        [JsonProperty("epochStartTimestampMs")]
        public BigInteger EpochStartTimestampMs { get; internal set; }

        #region System Parameters

        /// <summary>
        /// The duration of an epoch, in milliseconds.
        /// </summary>
        [JsonProperty("epochDurationMs")]
        public BigInteger EpochDurationMs { get; internal set; }

        /// <summary>
        /// The starting epoch in which stake subsidies start being paid out.
        /// </summary>
        [JsonProperty("stakeSubsidyStartEpoch")]
        public BigInteger StakeSubsidyStartEpoch { get; internal set; }

        /// <summary>
        /// Maximum number of active validators at any moment.
        /// We do not allow the number of validators in any epoch to go above this.
        /// </summary>
        [JsonProperty("maxValidatorCount")]
        public BigInteger MaxValidatorCount { get; internal set; }

        /// <summary>
        /// Lower-bound on the amount of stake required to become a validator.
        /// </summary>
        [JsonProperty("minValidatorJoiningStake")]
        public BigInteger MinValidatorJoiningStake { get; internal set; }

        /// <summary>
        /// Validators with stake amount below `validator_low_stake_threshold` are considered to
        /// have low stake and will be escorted out of the validator set after being below this
        /// threshold for more than `validator_low_stake_grace_period` number of epochs.
        /// </summary>
        [JsonProperty("validatorLowStakeThreshold")]
        public BigInteger ValidatorLowStakeThreshold { get; internal set; }

        /// <summary>
        /// Validators with stake below `validator_very_low_stake_threshold` will be removed
        /// immediately at epoch change, no grace period.
        /// </summary>
        [JsonProperty("validatorVeryLowStakeThreshold")]
        public BigInteger ValidatorVeryLowStakeThreshold { get; internal set; }

        /// <summary>
        /// A validator can have stake below `validator_low_stake_threshold`
        /// for this many epochs before being kicked out.
        /// </summary>
        [JsonProperty("validatorLowStakeGracePeriod")]
        public BigInteger ValidatorLowStakeGracePeriod { get; internal set; }

        /// <summary>
        /// Balance of SUI set aside for stake subsidies that will be drawn down over time.
        /// </summary>
        [JsonProperty("stakeSubsidyBalance")]
        public BigInteger StakeSubsidyBalance { get; internal set; }

        /// <summary>
        /// This counter may be different from the current epoch number if
        /// in some epochs we decide to skip the subsidy.
        /// </summary>
        [JsonProperty("stakeSubsidyDistributionCounter")]
        public BigInteger StakeSubsidyDistributionCounter { get; internal set; }

        /// <summary>
        /// The amount of stake subsidy to be drawn down per epoch.
        /// This amount decays and decreases over time.
        /// </summary>
        [JsonProperty("stakeSubsidyCurrentDistributionAmount")]
        public BigInteger StakeSubsidyCurrentDistributionAmount { get; internal set; }

        /// <summary>
        /// Number of distributions to occur before the distribution amount decays.
        /// </summary>
        [JsonProperty("stakeSubsidyPeriodLength")]
        public BigInteger StakeSubsidyPeriodLength { get; internal set; }

        /// <summary>
        /// The rate at which the distribution amount decays at the end of each
        /// period. Expressed in basis points.
        /// </summary>
        [JsonProperty("stakeSubsidyDecreaseRate")]
        public ushort StakeSubsidyDecreaseRate { get; internal set; }

        #endregion

        #region Validator Set

        /// <summary>
        /// Total amount of stake from all active validators at the beginning of the epoch.
        /// </summary>
        [JsonProperty("totalStake")]
        public BigInteger TotalStake { get; internal set; }

        /// <summary>
        /// The list of active validators in the current epoch.
        /// </summary>
        [JsonProperty("activeValidators")]
        public SuiValidatorSummary[] ActiveValidators { get; internal set; }

        /// <summary>
        /// ID of the object that contains the list of new validators that will join at the end of the epoch.
        /// </summary>
        [JsonProperty("pendingActiveValidatorsId")]
        public string PendingActiveValidatorsID { get; internal set; }

        /// <summary>
        /// Number of new validators that will join at the end of the epoch.
        /// </summary>
        [JsonProperty("pendingActiveValidatorsSize")]
        public BigInteger PendingActiveValidatorsSize { get; internal set; }

        /// <summary>
        /// Removal requests from the validators. Each element is an index
        /// pointing to `active_validators`.
        /// </summary>
        [JsonProperty("pendingRemovals")]
        public BigInteger[] PendingRemovals { get; internal set; }

        /// <summary>
        /// ID of the object that maps from staking pool's ID to the sui address of a validator.
        /// </summary>
        [JsonProperty("stakingPoolMappingsId")]
        public string StakingPoolMappingsID { get; internal set; }

        /// <summary>
        /// Number of staking pool mappings.
        /// </summary>
        [JsonProperty("stakingPoolMappingsSize")]
        public BigInteger StakingPoolMappingsSize { get; internal set; }

        /// <summary>
        /// ID of the object that maps from a staking pool ID to the inactive validator that has that pool as its staking pool.
        /// </summary>
        [JsonProperty("inactivePoolsId")]
        public string InactivePoolsID { get; internal set; }

        /// <summary>
        /// Number of inactive staking pools.
        /// </summary>
        [JsonProperty("inactivePoolsSize")]
        public BigInteger InactivePoolsSize { get; internal set; }

        /// <summary>
        /// ID of the object that stores preactive validators, mapping their addresses to their `Validator` structs.
        /// </summary>
        [JsonProperty("validatorCandidatesId")]
        public string ValidatorCandidatesID { get; internal set; }

        /// <summary>
        /// Number of preactive validators.
        /// </summary>
        [JsonProperty("validatorCandidatesSize")]
        public BigInteger ValidatorCandidatesSize { get; internal set; }

        /// <summary>
        /// Map storing the number of epochs for which each validator has been below the low stake threshold.
        /// </summary>
        [JsonProperty("atRiskValidators")]
        public AtRiskValidator[] AtRiskValidators { get; internal set; }

        /// <summary>
        /// A map storing the records of validator reporting each other.
        /// </summary>
        [JsonProperty("validatorReportRecords")]
        public ValidatorReportRecord[] ValidatorReportRecords { get; internal set; }

        #endregion
    }

    /// <summary>
    /// The class that represents a summary of a given validator on the Sui Network.
    /// </summary>
    [JsonObject]
    public class SuiValidatorSummary
    {
        #region Metadata

        [JsonProperty("suiAddress")]
        public string SuiAddress { get; internal set; }

        [JsonProperty("protocolPubkeyBytes")]
        public string ProtocolPubkeyBytes { get; internal set; }

        [JsonProperty("networkPubkeyBytes")]
        public string NetworkPubkeyBytes { get; internal set; }

        [JsonProperty("workerPubkeyBytes")]
        public string WorkerPubkeyBytes { get; internal set; }

        [JsonProperty("proofOfPossessionBytes")]
        public string ProofOfPossessionBytes { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("description")]
        public string Description { get; internal set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; internal set; }

        [JsonProperty("projectUrl")]
        public string ProjectUrl { get; internal set; }

        [JsonProperty("netAddress")]
        public string NetAddress { get; internal set; }

        [JsonProperty("p2pAddress")]
        public string P2PAddress { get; internal set; }

        [JsonProperty("primaryAddress")]
        public string PrimaryAddress { get; internal set; }

        [JsonProperty("workerAddress")]
        public string WorkerAddress { get; internal set; }

        [JsonProperty("nextEpochProtocolPubkeyBytes", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochProtocolPubkeyBytes { get; internal set; }

        [JsonProperty("nextEpochProofOfPossession", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochProofOfPossession { get; internal set; }

        [JsonProperty("nextEpochNetworkPubkeyBytes", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochNetworkPubkeyBytes { get; internal set; }

        [JsonProperty("nextEpochWorkerPubkeyBytes", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochWorkerPubkeyBytes { get; internal set; }

        [JsonProperty("nextEpochNetAddress", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochNetAddress { get; internal set; }

        [JsonProperty("nextEpochP2pAddress", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochP2PAddress { get; internal set; }

        [JsonProperty("nextEpochPrimaryAddress", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochPrimaryAddress { get; internal set; }

        [JsonProperty("nextEpochWorkerAddress", NullValueHandling = NullValueHandling.Include)]
        public string NextEpochWorkerAddress { get; internal set; }

        [JsonProperty("votingPower")]
        public BigInteger VotingPower { get; internal set; }

        [JsonProperty("operationCapId")]
        public AccountAddress OperationCapID { get; internal set; }

        [JsonProperty("gasPrice")]
        public BigInteger GasPrice { get; internal set; }

        [JsonProperty("commissionRate")]
        public BigInteger CommissionRate { get; internal set; }

        [JsonProperty("nextEpochStake")]
        public BigInteger NextEpochStake { get; internal set; }

        [JsonProperty("nextEpochGasPrice")]
        public BigInteger NextEpochGasPrice { get; internal set; }

        [JsonProperty("nextEpochCommissionRate")]
        public BigInteger NextEpochCommissionRate { get; internal set; }

        #endregion

        #region Staking Pool Information

        /// <summary>
        /// ID of the staking pool object.
        /// </summary>
        [JsonProperty("stakingPoolId")]
        public string StakingPoolId { get; internal set; }

        /// <summary>
        /// The epoch at which this pool became active.
        /// </summary>
        [JsonProperty("stakingPoolActivationEpoch", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? StakingPoolActivationEpoch { get; internal set; }

        /// <summary>
        /// The epoch at which this staking pool ceased to be active.
        /// `null` = {pre-active, active}
        /// </summary>
        [JsonProperty("stakingPoolDeactivationEpoch", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? StakingPoolDeactivationEpoch { get; internal set; }

        /// <summary>
        /// The total number of SUI tokens in this pool.
        /// </summary>
        [JsonProperty("stakingPoolSuiBalance", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? StakingPoolSuiBalance { get; internal set; }

        /// <summary>
        /// The epoch stake rewards will be added here at the end of each epoch.
        /// </summary>
        [JsonProperty("rewardsPool", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? RewardsPool { get; internal set; }

        /// <summary>
        /// Total number of pool tokens issued by the pool.
        /// </summary>
        [JsonProperty("poolTokenBalance")]
        public BigInteger PoolTokenBalance { get; internal set; }

        /// <summary>
        /// Pending stake amount for this epoch.
        /// </summary>
        [JsonProperty("pendingStake")]
        public BigInteger PendingStake { get; internal set; }

        /// <summary>
        /// Pending stake withdrawn during the current epoch, emptied at epoch boundaries.
        /// </summary>
        [JsonProperty("pendingTotalSuiWithdraw")]
        public BigInteger PendingTotalSuiWithdraw { get; internal set; }

        /// <summary>
        /// Pending pool token withdrawn during the current epoch, emptied at epoch boundaries.
        /// </summary>
        [JsonProperty("pendingPoolTokenWithdraw")]
        public BigInteger PendingPoolTokenWithdraw { get; internal set; }

        /// <summary>
        /// ID of the exchange rate table object.
        /// </summary>
        [JsonProperty("exchangeRatesId")]
        public string ExchangeRatesID { get; internal set; }

        /// <summary>
        /// Number of exchange rates in the table.
        /// </summary>
        [JsonProperty("exchangeRatesSize")]
        public BigInteger ExchangeRatesSize { get; internal set; }

        #endregion
    }

    [JsonConverter(typeof(ValidatorReportRecordConverter))]
    public class ValidatorReportRecord : ReturnBase
    {
        public AccountAddress Validator { get; internal set; }

        public AccountAddress[] ReportedValidators { get; internal set; }

        public ValidatorReportRecord
        (
            AccountAddress validator,
            AccountAddress[] reported_validators
        )
        {
            this.Validator = validator;
            this.ReportedValidators = reported_validators;
        }

        public ValidatorReportRecord(SuiError error)
        {
            this.Error = error;
        }
    }

    [JsonConverter(typeof(AtRiskValidatorConverter))]
    public class AtRiskValidator : ReturnBase
    {
        public AccountAddress Validator { get; internal set; }

        public BigInteger NumberOfEpochs { get; internal set; }

        public AtRiskValidator
        (
            AccountAddress validator,
            BigInteger number_of_epochs
        )
        {
            this.Validator = validator;
            this.NumberOfEpochs = number_of_epochs;
        }

        public AtRiskValidator(SuiError error)
        {
            this.Error = error;
        }
    }

    public class Base64ByteArrayResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SuiResult<byte[]>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return new SuiResult<byte[]>(CryptoBytes.FromBase64String(reader.Value.ToString()));

            return new SuiResult<byte[]>(null, new SuiError(0, "Unable to convert JSON to a byte array.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                SuiResult<byte[]> byte_array_result = (SuiResult<byte[]>)value;

                if (byte_array_result.Error != null)
                    writer.WriteValue(JsonConvert.SerializeObject(byte_array_result.Error));
                else
                    writer.WriteValue(CryptoBytes.ToBase64String(byte_array_result.Result));
            }
        }
    }

    public class AtRiskValidatorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(AtRiskValidator);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray at_risk_validator = JArray.Load(reader);

                return new AtRiskValidator
                (
                    (AccountAddress)at_risk_validator[0].ToObject(typeof(AccountAddress)),
                    BigInteger.Parse(at_risk_validator[1].Value<string>())
                );
            }

            return new AtRiskValidator(new SuiError(0, "Unable to convert JSON to AtRiskValidator.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                AtRiskValidator at_risk_validator = (AtRiskValidator)value;

                writer.WriteStartArray();

                writer.WriteValue(at_risk_validator.Validator.KeyHex);
                writer.WriteValue(at_risk_validator.NumberOfEpochs);

                writer.WriteEndArray();
            }
        }
    }

    public class ValidatorReportRecordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ValidatorReportRecord);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray validator_report_record = JArray.Load(reader);

                return new ValidatorReportRecord
                (
                    (AccountAddress)validator_report_record[0].ToObject(typeof(AccountAddress)),
                    ((JArray)validator_report_record[1]).Select(record => (AccountAddress)record.ToObject(typeof(AccountAddress))).ToArray()
                );
            }

            return new ValidatorReportRecord(new SuiError(0, "Unable to convert JSON to ValidatorReportRecord.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                ValidatorReportRecord validator_report_record = (ValidatorReportRecord)value;

                writer.WriteStartArray();

                writer.WriteValue(validator_report_record.Validator.KeyHex);
                writer.WriteValue(validator_report_record.ReportedValidators.Select(validator => validator.KeyHex).ToArray());

                writer.WriteEndArray();
            }
        }
    }
}