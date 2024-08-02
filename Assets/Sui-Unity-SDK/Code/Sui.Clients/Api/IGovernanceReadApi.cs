//
//  IGovernanceReadApi.cs
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

using Sui.Accounts;
using Sui.Cryptography;
using Sui.Rpc.Models;
using Sui.Rpc;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Sui.Clients.Api
{
    public interface IGovernanceReadApi
    {
        #region suix_getCommitteeInfo

        /// <summary>
        /// Return the committee information for the asked `epoch`.
        /// https://docs.sui.io/sui-jsonrpc#suix_getCommitteeInfo
        /// </summary>
        /// <param name="epoch">The epoch of interest. If None, default to the latest epoch</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CommitteeInfo` object.</returns>
        public Task<RpcResult<CommitteeInfo>> GetCommitteeInfoAsync(BigInteger? epoch);

        #endregion

        #region suix_getLatestSuiSystemState

        /// <summary>
        /// Return the latest SUI system state object on-chain.
        /// https://docs.sui.io/sui-jsonrpc#suix_getLatestSuiSystemState
        /// </summary>
        /// <returns>An asynchronous task object containing the wrapped result of the `SuiSystemSummary` object.</returns>
        public Task<RpcResult<SuiSystemSummary>> GetLatestSuiSystemStateAsync();

        #endregion

        #region suix_getReferenceGasPrice

        /// <summary>
        /// Return the reference gas price for the network.
        /// https://docs.sui.io/sui-api-ref#suix_getreferencegasprice
        /// </summary>
        /// <returns>An asynchronous task object containing the wrapped result of the `BigInteger`.</returns>
        Task<RpcResult<BigInteger>> GetReferenceGasPriceAsync();

        #endregion

        #region suix_getStakes

        /// <summary>
        /// Return all [DelegatedStake].
        /// https://docs.sui.io/sui-jsonrpc#suix_getStakes
        /// </summary>
        /// <param name="owner">The owner of the stakes.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the IEnumerable of `DelegatedStake`.</returns>
        public Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesAsync(Account owner);

        /// <summary>
        /// Return all [DelegatedStake].
        /// https://docs.sui.io/sui-jsonrpc#suix_getStakes
        /// </summary>
        /// <param name="owner">The owner of the stakes.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the IEnumerable of `DelegatedStake`.</returns>
        public Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesAsync(SuiPublicKeyBase owner);

        /// <summary>
        /// Return all [DelegatedStake].
        /// https://docs.sui.io/sui-jsonrpc#suix_getStakes
        /// </summary>
        /// <param name="owner">The owner of the stakes.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the IEnumerable of `DelegatedStake`.</returns>
        public Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesAsync(AccountAddress owner);

        #endregion

        #region suix_getStakesByIds

        /// <summary>
        /// Return one or more [DelegatedStake]. If a Stake was withdrawn its status will be Unstaked.
        /// https://docs.sui.io/sui-jsonrpc#suix_getStakesByIds
        /// </summary>
        /// <param name="staked_sui_ids">The addresses associated with the stakes.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the IEnumerable of `DelegatedStake`.</returns>
        public Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesByIdsAsync(List<AccountAddress> staked_sui_ids);

        #endregion

        #region suix_getValidatorsApy

        /// <summary>
        /// Return the validator APY.
        /// https://docs.sui.io/sui-jsonrpc#suix_getValidatorsApy
        /// </summary>
        /// <returns>An asynchronous task object containing the wrapped result of the `ValidatorsApy` object.</returns>
        public Task<RpcResult<ValidatorsApy>> GetValidatorsApyAsync();

        #endregion
    }
}