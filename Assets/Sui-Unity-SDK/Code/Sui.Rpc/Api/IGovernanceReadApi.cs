using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Sui.Rpc.Api
{
    public interface IGovernanceReadApi
    {
        /// <summary>
        /// https://docs.sui.io/sui-jsonrpc#suix_getCommitteeInfo
        /// </summary>
        /// <returns></returns>
        public Task<RpcResult<CommitteeInfo>> GetCommitteeInfo(BigInteger epoch);
        public Task<RpcResult<CommitteeInfo>> GetCommitteeInfo();

        /// <summary>
        /// https://docs.sui.io/sui-jsonrpc#suix_getValidatorsApy
        /// </summary>
        /// <returns></returns>
        public Task<RpcResult<ValidatorsApy>> GetValidatorsApy();

        /// <summary>
        /// Return all [DelegatedStake].
        /// https://docs.sui.io/sui-jsonrpc#suix_getStakes
        /// </summary>
        /// <returns></returns>
        public Task<RpcResult<IEnumerable<Stakes>>> GetStakes(AccountAddress owner);

        /// <summary>
        /// Return one or more [DelegatedStake]. If a Stake was withdrawn its status will be Unstaked.
        /// https://docs.sui.io/sui-jsonrpc#suix_getStakesByIds
        /// </summary>
        /// <returns></returns>
        public Task<RpcResult<IEnumerable<Stakes>>> GetStakesByIds(List<AccountAddress> stakedSuiId);

        /// <summary>
        /// Return the latest SUI system state object on-chain.
        /// https://docs.sui.io/sui-jsonrpc#suix_getLatestSuiSystemState
        /// </summary>
        /// <returns></returns>
        public Task<RpcResult<SuiSystemSummary>> GetLatestSuiSystemState();
    }
}