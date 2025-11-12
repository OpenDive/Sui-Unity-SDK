//
//  SuiValidatorSummary.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The class that represents a summary of a given validator on the Sui Network.
    /// </summary>
    [JsonObject]
    public class SuiValidatorSummary
    {
        #region Metadata

        [JsonProperty("suiAddress")]
        public AccountAddress SuiAddress { get; internal set; }

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
}