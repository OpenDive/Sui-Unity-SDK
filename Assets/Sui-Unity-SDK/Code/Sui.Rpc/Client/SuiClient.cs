using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using Sui.Rpc.Api;
using Sui.Rpc.Models;
using Sui.Accounts;
using OpenDive.BCS;
using Sui.Transactions;
using Sui.Cryptography;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Chaos.NaCl;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using static UnityEngine.UI.GridLayoutGroup;
using NBitcoin.DataEncoders;
using Codice.CM.Common.Serialization.Replication;

namespace Sui.Rpc
{
    public enum RequestType
    {
        WaitForEffectsCert,
        WaitForLocalExecution
    }

    public class SuiClient : IReadApi, ICoinQueryApi, IGovernanceReadApi, IExtendedApi
    {
        private UnityRpcClient _rpcClient;
        public Connection Connection;

        public SuiClient(Connection connection)
        {
            this.Connection = connection;
            this._rpcClient = new UnityRpcClient(connection.FULL_NODE);
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

        public async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransactionBlock
        (
            Transactions.TransactionBlock transaction_block,
            Account account,
            TransactionBlockResponseOptions options = null,
            RequestType? request_type = null
        )
        {
            TransactionBlockResponseOptions opts = options != null ? options : new TransactionBlockResponseOptions();
            transaction_block.SetSenderIfNotSet(AccountAddress.FromHex(account.SuiAddress()));
            RpcResult<byte[]> tx_bytes = await transaction_block.Build(new BuildOptions(this));

            if (tx_bytes.Error != null)
                return RpcResult<TransactionBlockResponse>.GetErrorResult(tx_bytes.Error.Message);

            SignatureBase signature = account.SignTransactionBlock(tx_bytes.Result);
            Debug.Log($"MARCUS::: TX EXECUTE BYTES - {String.Join(", ", tx_bytes.Result)}");
            return await this.ExecuteTransactionBlock(tx_bytes.Result, account.ToSerializedSignature(signature), opts, request_type);
        }

        public async Task<RpcResult<TransactionBlockResponse>> ExecuteTransactionBlock
        (
            byte[] transaction_block,
            string signature,
            TransactionBlockResponseOptions options = null,
            RequestType? request_type = null
        )
        {
            TransactionBlockResponseOptions opts = options != null ? options : new TransactionBlockResponseOptions();
            Debug.Log($"MARCUS::: EXECUTE TRANSACTION BLOCK - {JsonConvert.SerializeObject(ArgumentBuilder.BuildArguments(transaction_block, new string[] { signature }, opts))}");
            return await SendRpcRequestAsync<TransactionBlockResponse>(
                Methods.sui_executeTransactionBlock.ToString(),
                ArgumentBuilder.BuildArguments
                (
                    transaction_block,
                    new string[] { signature },
                    opts,
                    request_type != null ? request_type.ToString() : null
                )
            );
        }

        public async Task<RpcResult<TransactionBlockResponse>> WaitForTransaction(string transaction, TransactionBlockResponseOptions options = null)
        {
            TransactionBlockResponseOptions opts = options != null ? options : new TransactionBlockResponseOptions();
            int count = 0;
            bool is_done = false;
            while (count == 0 && is_done == false)
            {
                if (count >= 60)
                    return RpcResult<TransactionBlockResponse>.GetErrorResult("Transaction Timed Out");

                await Task.Delay(TimeSpan.FromSeconds(1f));
                count += 1;
                is_done = await this.IsValidTransactionBlock(transaction);
            }
            return await this.GetTransactionBlock(transaction, opts);
        }

        private async Task<bool> IsValidTransactionBlock(string transaction)
        {
            RpcResult<TransactionBlockResponse> result = await this.GetTransactionBlock(transaction);

            if (result.Error != null)
                return false;

            return result.Result.TimestampMs != null;
        }

        public async Task<RpcResult<TransactionBlockResponse>> GetTransactionBlock(string digest, TransactionBlockResponseOptions options = null)
        {
            if (this.IsValidTransactionDigest(digest) == false)
                return RpcResult<TransactionBlockResponse>.GetErrorResult("Invalid digest.");

            TransactionBlockResponseOptions opts = options != null ? options : new TransactionBlockResponseOptions();
            return await SendRpcRequestAsync<TransactionBlockResponse>(
                Methods.sui_getTransactionBlock.ToString(),
                ArgumentBuilder.BuildArguments(digest, opts)
            );
        }

        public async Task<RpcResult<IEnumerable<TransactionBlockResponse>>> GetTransactionBlocks(string[] digests, TransactionBlockResponseOptions options = null)
        {
            foreach(string digest in digests)
                if (this.IsValidTransactionDigest(digest) == false)
                    return RpcResult<IEnumerable<TransactionBlockResponse>>.GetErrorResult("Invalid digest.");

            if (digests.Distinct().Count() != digests.Count())
                return RpcResult<IEnumerable<TransactionBlockResponse>>.GetErrorResult("Digests do not match.");

            TransactionBlockResponseOptions opts = options != null ? options : new TransactionBlockResponseOptions();
            return await SendRpcRequestAsync<IEnumerable<TransactionBlockResponse>>(
                Methods.sui_multiGetTransactionBlocks.ToString(),
                ArgumentBuilder.BuildArguments(digests, opts)
            );
        }

        public async Task<RpcResult<TransactionBlockResponsePage>> QueryTransactionBlocks
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            ITransactionFilter filter = null,
            TransactionBlockResponseOptions options = null
        )
        {
            return await SendRpcRequestAsync<TransactionBlockResponsePage>(
                Methods.suix_queryTransactionBlocks.ToString(),
                ArgumentBuilder.BuildArguments
                (
                    new TransactionBlockResponseQuery(filter, options),
                    cursor,
                    limit,
                    order == null ? null : order == SortOrder.Descending ? true : false
                )
            );
        }

        public async Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync()
        {
            return await SendRpcRequestAsync<BigInteger>(
                Methods.sui_getTotalTransactionBlocks.ToString()
            );
        }

        public async Task<RpcResult<Models.ProtocolConfig>> GetProtocolConfigAsync()
        {
            return await SendRpcRequestAsync<Models.ProtocolConfig>(
                Methods.sui_getProtocolConfig.ToString()
            );
        }

        public async Task<RpcResult<ulong>> GetReferenceGasPriceAsync()
        {
            return await SendRpcRequestAsync<ulong>(
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
            string owner, string coinType = null, string cursor = null, int? limit = null)
        {
            return await SendRpcRequestAsync<CoinPage>(
                Methods.suix_getCoins.ToString(),
                ArgumentBuilder.BuildArguments(owner, coinType, cursor, limit)
            );
        }

        public async Task<RpcResult<CoinPage>> GetAllCoins
        (
            string account,
            int? limit = null
        )
        {
            return await SendRpcRequestAsync<CoinPage>(
                Methods.suix_getAllCoins.ToString(),
                ArgumentBuilder.BuildArguments(account, limit)
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
            return await SendRpcRequestAsync<string>(
                Methods.sui_getChainIdentifier.ToString()
            );

        }

        public async Task<RpcResult<Balance>> GetBalanceAsync(
            string owner, string coinType = null)
        {
            return await SendRpcRequestAsync<Balance>(
                Methods.suix_getBalance.ToString(),
                ArgumentBuilder.BuildArguments(owner, coinType)
            );
        }

        public async Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(
            string owner)
        {
            return await SendRpcRequestAsync<IEnumerable<Balance>>(
                Methods.suix_getAllBalances.ToString(),
                ArgumentBuilder.BuildArguments(owner)
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

        public async Task<RpcResult<CoinMetadata>> GetCoinMetadata(
            string coinType)
        {
            return await SendRpcRequestAsync<CoinMetadata>(
                Methods.suix_getCoinMetadata.ToString(),
                ArgumentBuilder.BuildArguments(coinType)
            );
        }

        public async Task<RpcResult<Checkpoint>> GetCheckpoint(string id)
        {
            return await SendRpcRequestAsync<Checkpoint>(
                Methods.sui_getCheckpoint.ToString(),
                ArgumentBuilder.BuildArguments(id)
            );
        }

        public async Task<RpcResult<Checkpoints>> GetCheckpoints(
            string cursor, int limit, bool is_descending)
        {
            return await SendRpcRequestAsync<Checkpoints>(
                Methods.sui_getCheckpoints.ToString(),
                ArgumentBuilder.BuildArguments(cursor, limit, is_descending)
            );
        }

        public async Task<RpcResult<string>> GetLatestCheckpointSequenceNumber()
        {
            return await SendRpcRequestAsync<string>(
                Methods.sui_getLatestCheckpointSequenceNumber.ToString()
            );
        }

        public async Task<RpcResult<SuiMoveNormalizedModule>> GetNormalizedMoveModule(
            string package, string moduleName)
        {
            //return await SendRpcRequestAsync<SuiMoveNormalizedModule>("sui_getNormalizedMoveModule",
            //    ArgumentBuilder.BuildArguments(package, moduleName), new NormalizedMoveModuleConverter());
            return await SendRpcRequestAsync<SuiMoveNormalizedModule>(
                Methods.sui_getNormalizedMoveModule.ToString(),
                ArgumentBuilder.BuildArguments(package, moduleName)
            );
        }

        public async Task<RpcResult<Models.Event[]>> GetEvents(string transactionDigest)
        {
            return await SendRpcRequestAsync<Models.Event[]>(
                Methods.sui_getEvents.ToString(),
                ArgumentBuilder.BuildArguments(transactionDigest)
            );
        }

        public async Task<RpcResult<EventPage>> QueryEvents
        (
            IEventFilter query = null,
            EventId cursor = null,
            int? limit = null,
            SortOrder? order = null
        )
        {
            return await SendRpcRequestAsync<EventPage>(
                Methods.suix_queryEvents.ToString(),
                ArgumentBuilder.BuildArguments
                (
                    query ??= new AllEventFilter(new IEventFilter[] { }),
                    cursor,
                    limit,
                    order == null ? null : order == SortOrder.Descending ? true : false
                )
            );
        }

        public async Task<RpcResult<Dictionary<string, SuiMoveNormalizedModule>>> GetNormalizedMoveModulesByPackage(string package)
        {
            //return await SendRpcRequestAsync<Dictionary<string, SuiMoveNormalizedModule>>("sui_getNormalizedMoveModulesByPackage",
            //    ArgumentBuilder.BuildArguments(package), new NormalizedModulesByPackageConverter());

            return await SendRpcRequestAsync<Dictionary<string, SuiMoveNormalizedModule>>(
                Methods.sui_getNormalizedMoveModulesByPackage.ToString(),
                ArgumentBuilder.BuildArguments(package)
            );
        }

        public async Task<RpcResult<MoveFunctionArgTypes>> GetMoveFunctionArgTypes(string package, string module, string function)
        {
            //return await SendRpcRequestAsync<MoveFunctionArgTypes>("sui_getMoveFunctionArgTypes",
            //    ArgumentBuilder.BuildArguments(package, module, function), new MoveFunctionArgTypesConverter());

            return await SendRpcRequestAsync<MoveFunctionArgTypes>(
                Methods.sui_getMoveFunctionArgTypes.ToString(),
                ArgumentBuilder.BuildArguments(package, module, function)
            );
        }

        public async Task<RpcResult<SuiMoveNormalizedStruct>> GetNormalizedMoveStruct(string package, string moduleName, string structName)
        {
            //return await SendRpcRequestAsync<SuiMoveNormalizedStruct>("sui_getNormalizedMoveStruct",
            //    ArgumentBuilder.BuildArguments(package, moduleName, structName), new MoveStructConverter());
            return await SendRpcRequestAsync<SuiMoveNormalizedStruct>(
                Methods.sui_getNormalizedMoveStruct.ToString(),
                ArgumentBuilder.BuildArguments(package, moduleName, structName)
            );
        }

        public async Task<RpcResult<TotalSupply>> GetTotalSupply(
            string coinType)
        {
            return await SendRpcRequestAsync<TotalSupply>(
                Methods.suix_getTotalSupply.ToString(),
                ArgumentBuilder.BuildArguments(coinType.ToString())
            );
        }

        public async Task<RpcResult<CommitteeInfo>> GetCommitteeInfo(
            BigInteger? epoch = null)
        {
            return await SendRpcRequestAsync<CommitteeInfo>(
                Methods.suix_getCommitteeInfo.ToString(),
                ArgumentBuilder.BuildArguments(epoch.ToString())
            );
        }

        public async Task<RpcResult<ValidatorsApy>> GetValidatorsApy()
        {
            return await SendRpcRequestAsync<ValidatorsApy>(
                Methods.suix_getValidatorsApy.ToString()
            );
        }

        public async Task<RpcResult<IEnumerable<Stakes>>> GetStakes(
            string owner)
        {
            return await SendRpcRequestAsync<IEnumerable<Stakes>>(
                Methods.suix_getStakes.ToString(),
                ArgumentBuilder.BuildArguments(owner)
            );
        }

        public async Task<RpcResult<IEnumerable<Stakes>>> GetStakesByIds(List<string> stakedSuiId)
        {
            return await SendRpcRequestAsync<IEnumerable<Stakes>>(
                Methods.suix_getStakesByIds.ToString(),
                ArgumentBuilder.BuildArguments(
                    stakedSuiId
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

        public async Task<RpcResult<DevInspectResponse>> DevInspectTransactionBlock(
            Account sender, Transactions.TransactionBlock transaction_block, int? gasPrice = null, string epoch = null)
        {
            string sender_address = sender.SuiAddress();
            transaction_block.SetSenderIfNotSet(AccountAddress.FromHex(sender_address));
            Debug.Log($"MARCUS::: DEV INSPECT TX BLOCK BUILDER BEFORE - {JsonConvert.SerializeObject(transaction_block.BlockDataBuilder.Builder.Transactions)}");
            RpcResult<byte[]> tx_bytes = await transaction_block.Build(new BuildOptions(this, null, true));

            Debug.Log($"MARCUS::: DEV INSPECT TX BLOCK BUILDER AFTER - {JsonConvert.SerializeObject(transaction_block.BlockDataBuilder.Builder.Transactions)}");

            Debug.Log($"MARCUS::: DEV INSPECT TX BLOCK BUILDER BYTES - {String.Join(", ", tx_bytes.Result)}");

            if (tx_bytes.Error != null)
                return RpcResult<DevInspectResponse>.GetErrorResult(tx_bytes.Error.Message);

            string dev_inspect_tx_bytes = CryptoBytes.ToBase64String(tx_bytes.Result);
            return await SendRpcRequestAsync<DevInspectResponse>(
                Methods.sui_devInspectTransactionBlock.ToString(),
                ArgumentBuilder.BuildArguments(sender_address, dev_inspect_tx_bytes, gasPrice, epoch)
            );
        }

        public async Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObject(
            string parentObjectId, DynamicFieldName name)
        {
            return await SendRpcRequestAsync<ObjectDataResponse>(
                Methods.suix_getDynamicFieldObject.ToString(),
                ArgumentBuilder.BuildArguments(parentObjectId, name)
            );
        }

        public async Task<RpcResult<DynamicFieldPage>> GetDynamicFields(
            string parentObjectId, IObjectDataFilter filter = null, ObjectDataOptions options = null, string cursor = null, int? limit = null)
        {
            if (SuiClient.IsValidSuiAddress(parentObjectId) == false)
                return RpcResult<DynamicFieldPage>.GetErrorResult("Unable to validate the address.");

            return await SendRpcRequestAsync<DynamicFieldPage>(
                Methods.suix_getDynamicFields.ToString(),
                ArgumentBuilder.BuildArguments(parentObjectId, cursor, limit, filter, options)
            );
        }

        public async Task<RpcResult<ObjectRead>> TryGetPastObject(
            AccountAddress objectId, ObjectDataOptions options = null, string version = null)
        {
            return await SendRpcRequestAsync<ObjectRead>(
                Methods.sui_tryGetPastObject.ToString(),
                ArgumentBuilder.BuildArguments(objectId.ToHex(), version, options)
            );
        }

        public async Task<RpcResult<ObjectRead>> TryGetPastObject(
            string objectId, int version, ObjectDataOptions options = null)
        {
            return await SendRpcRequestAsync<ObjectRead>(
                Methods.sui_tryGetPastObject.ToString(),
                ArgumentBuilder.BuildArguments(objectId, version, options)
            );
        }

        public async Task<RpcResult<ObjectRead[]>> TryMultiGetPastObjects(
            PastObjectRequest pastObjects, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<ObjectRead[]>(
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

        public async Task<RpcResult<ObjectDataResponse>> GetObject(AccountAddress objectId, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<ObjectDataResponse>(
                Methods.sui_getObject.ToString(),
                ArgumentBuilder.BuildArguments(
                    objectId.ToHex(),
                    options
                )
            );
        }

        public async Task<RpcResult<ObjectDataResponse>> GetObject(string objectId, ObjectDataOptions options)
        {
            return await SendRpcRequestAsync<ObjectDataResponse>(
                Methods.sui_getObject.ToString(),
                ArgumentBuilder.BuildArguments(
                    objectId,
                    options
                )
            );
        }

        public async Task<RpcResult<IEnumerable<ObjectDataResponse>>> MultiGetObjects(AccountAddress[] objectIds, ObjectDataOptions options = null)
        {
            return await SendRpcRequestAsync<IEnumerable<ObjectDataResponse>>(
                Methods.sui_multiGetObjects.ToString(),
                ArgumentBuilder.BuildArguments(
                    objectIds.Select(x => x.ToHex()).ToArray(),
                    options
                )
            );
        }

        public async Task<RpcResult<IEnumerable<ObjectDataResponse>>> MultiGetObjects(string[] objectIds, ObjectDataOptions options = null)
        {
            foreach(string id in objectIds)
                if (SuiClient.IsValidSuiAddress(id) == false)
                    return RpcResult<IEnumerable<ObjectDataResponse>>.GetErrorResult("Unable to validate the address.");

            return await SendRpcRequestAsync<IEnumerable<ObjectDataResponse>>(
                Methods.sui_multiGetObjects.ToString(),
                ArgumentBuilder.BuildArguments(
                    objectIds,
                    options
                )
            );
        }

        public async Task<RpcResult<PaginatedObjectsResponse>> GetOwnedObjects(string owner, IObjectDataFilter filter = null, ObjectDataOptions options = null, string cursor = null, int? limit = null)
        {
            if (IsValidSuiAddress(owner) == false)
                return RpcResult<PaginatedObjectsResponse>.GetErrorResult("Unable to validate the address.");

            return await SendRpcRequestAsync<PaginatedObjectsResponse>(
                Methods.suix_getOwnedObjects.ToString(),
                ArgumentBuilder.BuildArguments(
                    owner,
                    new ObjectResponseQuery(filter, options),
                    cursor,
                    limit
                )
            );
        }

        public static bool IsValidSuiAddress(string address)
        {
            if (NormalizedTypeConverter.NormalizeSuiAddress(address) == null)
                return false;

            if (Regex.IsMatch(address, @"^(0x)?[0-9a-fA-F]{32,64}$") == false)
                return false;

            return true;
        }

        private bool IsValidTransactionDigest(string digest)
        {
            if (Utilities.Base58Encoder.IsValidEncoding(digest) == false)
                return false;

            Base58Encoder base58Encoder = new Base58Encoder();
            byte[] digest_data = base58Encoder.DecodeData(digest);

            return digest_data.Count() == 32;
        }
    }
}