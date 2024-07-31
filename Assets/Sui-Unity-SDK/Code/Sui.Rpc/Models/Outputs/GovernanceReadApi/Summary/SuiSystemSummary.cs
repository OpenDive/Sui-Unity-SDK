//
//  SuiSystemSummary.cs
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
}