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
    }
}