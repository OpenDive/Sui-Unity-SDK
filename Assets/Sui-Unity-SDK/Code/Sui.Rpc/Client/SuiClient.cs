using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sui.Rpc.Api;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using NBitcoin.RPC;

namespace Sui.Rpc
{
    public class SuiClient : IReadApi
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

        public async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method, IEnumerable<object> @params, JsonConverter converter)
        {
            RpcRequest request = new RpcRequest(method, @params);
            return await _rpcClient.HandleAsync<T>(request, converter);
        }

        private async Task<RpcResult<T>> SendRpcRequestAsync<T>(string method, IEnumerable<object> @params)
        {
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

        public async Task<RpcResult<string>> GetChainIdentifier()
        {
            return await SendRpcRequestAsync<string>("sui_getChainIdentifier");
        }

        public async Task<RpcResult<Checkpoint>> GetCheckpoint(string id)
        {
            return await SendRpcRequestAsync<Checkpoint>("sui_getCheckpoint",
                ArgumentBuilder.BuildArguments(id));
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
            return await SendRpcRequestAsync<SuiMoveNormalizedModule>("sui_getNormalizedMoveModule",
                ArgumentBuilder.BuildArguments(package, moduleName), new NormalizedMoveModuleConverter());
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
    }
}