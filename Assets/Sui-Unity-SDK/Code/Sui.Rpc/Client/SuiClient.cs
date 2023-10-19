using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Api;
using Sui.Rpc.Models;
using UnityEngine;
using static PlasticGui.WorkspaceWindow.Items.ExpandedTreeNode;
using static UnityEngine.UI.GridLayoutGroup;

namespace Sui.Rpc
{
    public class SuiClient : IReadApi, ICoinQueryApi, IGovernanceReadApi
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

        private async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method, IEnumerable<object> @params)
        {
            //var request = BuildRequest<T>(method, @params);
            RpcRequest request = new RpcRequest(method, @params);
            return await _rpcClient.SendAsync<T>(request);
        }

        public async Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync()
        {
            return await SendRpcRequestAsync<BigInteger>("sui_getTotalTransactionBlocks");
        }

        public async Task<RpcResult<ProtocolConfig>> GetProtocolConfigAsync()
        {
            return await SendRpcRequestAsync<ProtocolConfig>("sui_getProtocolConfig");
        }

        public async Task<RpcResult<BigInteger>> GetReferenceGasPriceAsync()
        {
            return await SendRpcRequestAsync<BigInteger>("suix_getReferenceGasPrice");
        }

        public async Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunction(string package, string moduleName, string functionName)
        {
            return await SendRpcRequestAsync<NormalizedMoveFunctionResponse>("sui_getNormalizedMoveFunction",
                ArgumentBuilder.BuildArguments(package, moduleName, functionName));
        }

        public async Task<RpcResult<CoinPage>> GetCoins(
            string owner, string coinType, string objectId, int limit)
        {
            return await SendRpcRequestAsync<CoinPage>("suix_getCoins",
                ArgumentBuilder.BuildArguments(owner, coinType, objectId, limit));
        }

        public async Task<RpcResult<TransactionBlockResponse>> DryRunTransactionBlock(string txBytesBase64)
        {
            return await SendRpcRequestAsync<TransactionBlockResponse>("sui_dryRunTransactionBlock",
                ArgumentBuilder.BuildArguments(txBytesBase64));
        }

        public async Task<RpcResult<IEnumerable<SuiObjectResponse>>> GetObjectsAsync(IEnumerable<string> objectIds, ObjectDataOptions options)
        {
            //throw new System.NotImplementedException();
            return await SendRpcRequestAsync<IEnumerable<SuiObjectResponse>>("sui_multiGetObjects",
                ArgumentBuilder.BuildArguments(objectIds, options));
        }

        public async Task<RpcResult<Balance>> GetBalanceAsync(AccountAddress owner, SuiStructTag coinType = null)
        {
            Debug.Log("METHOD: " + Methods.suix_getBalance.ToString());
            return await SendRpcRequestAsync<Balance>(Methods.suix_getBalance.ToString(),
                ArgumentBuilder.BuildArguments(owner.ToHex(), coinType.ToString()));
        }

        public async Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(AccountAddress owner)
        {
            return await SendRpcRequestAsync<IEnumerable<Balance>>(
                Methods.suix_getAllBalances.ToString(),
                ArgumentBuilder.BuildArguments(owner.ToHex())
            );
        }

        public async Task<RpcResult<CoinMetadata>> GetCoinMetadata(SuiStructTag coinType)
        {
            return await SendRpcRequestAsync<CoinMetadata>(
                Methods.suix_getCoinMetadata.ToString(),
                ArgumentBuilder.BuildArguments(coinType.ToString())
            );
        }

        public async Task<RpcResult<TotalSupply>> GetTotalSupply(SuiStructTag coinType)
        {
            return await SendRpcRequestAsync<TotalSupply>(
                Methods.suix_getTotalSupply.ToString(),
                ArgumentBuilder.BuildArguments(coinType.ToString())
            );
        }

        public async Task<RpcResult<CommitteeInfo>> GetCommitteeInfo(BigInteger epoch)
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

        public async Task<RpcResult<Stakes>> GetStakes(AccountAddress owner)
        {
            return await SendRpcRequestAsync<Stakes>(
                Methods.suix_getStakes.ToString(),
                ArgumentBuilder.BuildArguments(owner.ToHex())
            );
        }
    }
}