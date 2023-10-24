using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using Sui.Rpc.Api;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Sui.Accounts;
using OpenDive.BCS;

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

        public async Task<RpcResult<T>> SendRpcRequestAsync<T>(
            string method, IEnumerable<object> @params, JsonConverter converter)
        {
            RpcRequest request = new RpcRequest(method, @params);
            return await _rpcClient.HandleAsync<T>(request, converter);
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

        public async Task<RpcResult<string>> GetChainIdentifier()
        {
            return await SendRpcRequestAsync<string>("sui_getChainIdentifier");

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

        public async Task<RpcResult<Checkpoint>> GetCheckpoint(string id)
        {
            return await SendRpcRequestAsync<Checkpoint>(
                "sui_getCheckpoint",
                ArgumentBuilder.BuildArguments(id)
            );
        }

        public async Task<RpcResult<Checkpoints>> GetCheckpoints(string cursor, int limit, bool descendingOrder)
        {
            return await SendRpcRequestAsync<Checkpoints>("sui_getCheckpoints",
                ArgumentBuilder.BuildArguments(cursor, limit, descendingOrder));
        }

        public async Task<RpcResult<string>> GetLatestCheckpointSequenceNumber()
        {
            return await SendRpcRequestAsync<string>("sui_getLatestCheckpointSequenceNumber");
        }

        public async Task<RpcResult<SuiMoveNormalizedModule>> GetNormalizedMoveModule(string package, string moduleName)
        {
            //return await SendRpcRequestAsync<SuiMoveNormalizedModule>("sui_getNormalizedMoveModule",
            //    ArgumentBuilder.BuildArguments(package, moduleName), new NormalizedMoveModuleConverter());
            return await SendRpcRequestAsync<SuiMoveNormalizedModule>("sui_getNormalizedMoveModule",
                ArgumentBuilder.BuildArguments(package, moduleName));
        }

        public async Task<RpcResult<Models.Event[]>> GetEvents(string transactionDigest)
        {
            return await SendRpcRequestAsync<Models.Event[]>("sui_getEvents",
                ArgumentBuilder.BuildArguments(transactionDigest));
        }

        public async Task<RpcResult<Dictionary<string, SuiMoveNormalizedModule>>> GetNormalizedMoveModulesByPackage(string package)
        {
            return await SendRpcRequestAsync<Dictionary<string, SuiMoveNormalizedModule>>("sui_getNormalizedMoveModulesByPackage",
                ArgumentBuilder.BuildArguments(package), new NormalizedModulesByPackageConverter());
        }

        public async Task<RpcResult<MoveFunctionArgTypes>> GetMoveFunctionArgTypes(string package, string module, string function)
        {
            return await SendRpcRequestAsync<MoveFunctionArgTypes>("sui_getMoveFunctionArgTypes",
                ArgumentBuilder.BuildArguments(package, module, function), new MoveFunctionArgTypesConverter());
        }

        public async Task<RpcResult<SuiMoveNormalizedStruct>> GetNormalizedMoveStruct(string package, string moduleName, string structName)
        {
            //return await SendRpcRequestAsync<SuiMoveNormalizedStruct>("sui_getNormalizedMoveStruct",
            //    ArgumentBuilder.BuildArguments(package, moduleName, structName), new MoveStructConverter());
            return await SendRpcRequestAsync<SuiMoveNormalizedStruct>("sui_getNormalizedMoveStruct",
                ArgumentBuilder.BuildArguments(package, moduleName, structName));
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

        public async Task<RpcResult<ChildObjects>> GetLoadedChildObjects(string digest)
        {
            return await SendRpcRequestAsync<ChildObjects>(
                Methods.sui_getLoadedChildObjects.ToString(),
                ArgumentBuilder.BuildTypeArguments(digest)
            );
        }

        public async Task<RpcResult<DevInspectResponse>> DevInspectTransactionBlock(AccountAddress senderAddress, string txBytes, string gasPrice, string epoch = null)
        {
            return await SendRpcRequestAsync<DevInspectResponse>(
                Methods.sui_devInspectTransactionBlock.ToString(),
                ArgumentBuilder.BuildArguments(senderAddress.ToHex(), txBytes, gasPrice, epoch)
            );
        }

        public async Task<RpcResult<ObjectData>> GetDynamicFieldObject(string parentObjectId, string name)
        {
            return await SendRpcRequestAsync<ObjectData>(
                Methods.suix_getDynamicFieldObject.ToString(),
                ArgumentBuilder.BuildArguments(parentObjectId, name)
            );
        }

        public async Task<RpcResult<DynamicFieldPage>> GetDynamicFields(string parentObjectId, int limit, string cursor = null)
        {
            return await SendRpcRequestAsync<DynamicFieldPage>(
                Methods.suix_getDynamicFields.ToString(),
                ArgumentBuilder.BuildArguments(parentObjectId, cursor, limit)
            );
        }

        public async Task<RpcResult<PastObject>> TryGetPastObject(AccountAddress objectId, ObjectDataOptions options, string version = null)
        {
            return await SendRpcRequestAsync<PastObject>(
                Methods.sui_tryGetPastObject.ToString(),
                ArgumentBuilder.BuildArguments(objectId.ToHex(), version, options)
            );
        }

        public async Task<RpcResult<PastObject[]>> TryMultiGetPastObjects(PastObjectRequest pastObjects, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<PastObject[]>(
                Methods.sui_tryMultiGetPastObjects.ToString(),
                ArgumentBuilder.BuildArguments(pastObjects, options)
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