using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Api;
using Sui.Rpc.Models;

namespace Sui.Rpc
{
    public class SuiClient : IReadApi, ICoinQueryApi, IGovernanceReadApi, IExtendedApi
    {
        private UnityRpcClient _rpcClient;
        public SuiClient(UnityRpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        private async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method)
        {
            RpcRequest request = new RpcRequest(method, null);
            return await _rpcClient.SendAsync<T>(request);
        }

        private async Task<RpcResult<T>> SendRpcRequestAsync<T>(
            string method, IEnumerable<object> @params)
        {
            RpcRequest request = new RpcRequest(method, @params);
            return await _rpcClient.SendAsync<T>(request);
        }

        public async Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync()
        {
            return await SendRpcRequestAsync<BigInteger>(
                Methods.sui_getTotalTransactionBlocks.ToString()
            );
        }

        public async Task<RpcResult<ProtocolConfig>> GetProtocolConfigAsync()
        {
            return await SendRpcRequestAsync<ProtocolConfig>(
                Methods.sui_getProtocolConfig.ToString()
            );
        }

        public async Task<RpcResult<BigInteger>> GetReferenceGasPriceAsync()
        {
            return await SendRpcRequestAsync<BigInteger>(
                Methods.suix_getReferenceGasPrice.ToString()
            );
        }

        public async Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunction(
            string package, string moduleName, string functionName)
        {
            return await SendRpcRequestAsync<NormalizedMoveFunctionResponse>(
                Methods.sui_getNormalizedMoveFunction.ToString(),
                ArgumentBuilder.BuildArguments(package, moduleName, functionName)
            );
        }

        public async Task<RpcResult<CoinPage>> GetCoins(
            string owner, string coinType, string objectId, int limit)
        {
            return await SendRpcRequestAsync<CoinPage>(
                Methods.suix_getCoins.ToString(),
                ArgumentBuilder.BuildArguments(owner, coinType, objectId, limit)
            );
        }

        public async Task<RpcResult<TransactionBlockResponse>> DryRunTransactionBlock(
            string txBytesBase64)
        {
            return await SendRpcRequestAsync<TransactionBlockResponse>(
                Methods.sui_dryRunTransactionBlock.ToString(),
                ArgumentBuilder.BuildArguments(txBytesBase64)
            );
        }

        public async Task<RpcResult<IEnumerable<SuiObjectResponse>>> GetObjectsAsync(
            IEnumerable<string> objectIds, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<IEnumerable<SuiObjectResponse>>(
                Methods.sui_multiGetObjects.ToString(),
                ArgumentBuilder.BuildArguments(objectIds, options)
            );
        }

        public async Task<RpcResult<Balance>> GetBalanceAsync(
            AccountAddress owner, SuiStructTag coinType = null)
        {
            return await SendRpcRequestAsync<Balance>(
                Methods.suix_getBalance.ToString(),
                ArgumentBuilder.BuildArguments(owner.ToHex(), coinType.ToString()));
        }

        public async Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(
            AccountAddress owner)
        {
            return await SendRpcRequestAsync<IEnumerable<Balance>>(
                Methods.suix_getAllBalances.ToString(),
                ArgumentBuilder.BuildArguments(owner.ToHex())
            );
        }

        public async Task<RpcResult<CoinMetadata>> GetCoinMetadata(
            SuiStructTag coinType)
        {
            return await SendRpcRequestAsync<CoinMetadata>(
                Methods.suix_getCoinMetadata.ToString(),
                ArgumentBuilder.BuildArguments(coinType.ToString())
            );
        }

        public async Task<RpcResult<TotalSupply>> GetTotalSupply(
            SuiStructTag coinType)
        {
            return await SendRpcRequestAsync<TotalSupply>(
                Methods.suix_getTotalSupply.ToString(),
                ArgumentBuilder.BuildArguments(coinType.ToString())
            );
        }

        public async Task<RpcResult<CommitteeInfo>> GetCommitteeInfo(
            BigInteger epoch)
        {
            return await SendRpcRequestAsync<CommitteeInfo>(
                Methods.suix_getCommitteeInfo.ToString(),
                ArgumentBuilder.BuildArguments(epoch.ToString())
            );
        }

        public async Task<RpcResult<CommitteeInfo>> GetCommitteeInfo()
        {
            return await SendRpcRequestAsync<CommitteeInfo>(
                Methods.suix_getCommitteeInfo.ToString()
            );
        }

        public async Task<RpcResult<ValidatorsApy>> GetValidatorsApy()
        {
            return await SendRpcRequestAsync<ValidatorsApy>(
                Methods.suix_getValidatorsApy.ToString()
            );
        }

        public async Task<RpcResult<IEnumerable<Stakes>>> GetStakes(
            AccountAddress owner)
        {
            return await SendRpcRequestAsync<IEnumerable<Stakes>>(
                Methods.suix_getStakes.ToString(),
                ArgumentBuilder.BuildArguments(owner.ToHex())
            );
        }

        public async Task<RpcResult<IEnumerable<Stakes>>> GetStakesByIds(
            List<AccountAddress> stakedSuiId)
        {
            return await SendRpcRequestAsync<IEnumerable<Stakes>>(
                Methods.suix_getStakesByIds.ToString(),
                ArgumentBuilder.BuildTypeArguments(
                    stakedSuiId.Select(x => x.ToHex()).ToArray()
                )
            );
        }

        public async Task<RpcResult<SuiSystemSummary>> GetLatestSuiSystemState()
        {
            return await SendRpcRequestAsync<SuiSystemSummary>(
                Methods.suix_getLatestSuiSystemState.ToString()
            );
        }

        public async Task<RpcResult<AccountAddress>> ResolveNameServiceAddress(
            string name)
        {
            return await SendRpcRequestAsync<AccountAddress>(
                Methods.suix_resolveNameServiceAddress.ToString(),
                ArgumentBuilder.BuildArguments(name)
            );
        }
    }
}