//
//  SuiClient.cs
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

using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Transactions;
using Sui.Cryptography;
using System;
using Chaos.NaCl;
using Sui.Utilities;
using Sui.Types;
using Sui.Clients.Api;

namespace Sui.Rpc.Client
{
    /// <summary>
    /// The RPC Provider used to interact with the Sui Network.
    /// </summary>
    public class SuiClient : ICoinQueryApi, IExtendedApi, IGovernanceReadApi, IMoveUtils, IReadApi, IWriteApi
    {
        /// <summary>
        /// The RPC client to handle sending asynchronous tasks.
        /// </summary>
        private readonly UnityRpcClient _rpc_client;

        /// <summary>
        /// The type of connection used for the Sui Network.
        /// </summary>
        public Connection Connection;

        public SuiClient(Connection connection)
        {
            this.Connection = connection;
            this._rpc_client = new UnityRpcClient(connection.FULL_NODE);
        }

        #region ICoinQueryApi

        public async Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(Account owner)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<Balance>>
               (
                   Methods.suix_getAllBalances.ToString(),
                   ArgumentBuilder.BuildArguments(owner.SuiAddress())
               );

        public async Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(SuiPublicKeyBase owner)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<Balance>>
               (
                   Methods.suix_getAllBalances.ToString(),
                   ArgumentBuilder.BuildArguments(owner.ToSuiAddress())
               );

        public async Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(AccountAddress owner)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<Balance>>
               (
                   Methods.suix_getAllBalances.ToString(),
                   ArgumentBuilder.BuildArguments(owner.KeyHex)
               );

        public async Task<RpcResult<CoinPage>> GetAllCoinsAsync(Account owner, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<CoinPage>
               (
                   Methods.suix_getAllCoins.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.SuiAddress(),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<CoinPage>> GetAllCoinsAsync(SuiPublicKeyBase owner, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<CoinPage>
               (
                   Methods.suix_getAllCoins.ToString(),
                   ArgumentBuilder.BuildArguments(owner.ToSuiAddress(), filter.Cursor ?? null, filter.Limit ?? null)
               );

        public async Task<RpcResult<CoinPage>> GetAllCoinsAsync(AccountAddress owner, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<CoinPage>
               (
                   Methods.suix_getAllCoins.ToString(),
                   ArgumentBuilder.BuildArguments(owner.KeyHex, filter.Cursor ?? null, filter.Limit ?? null)
               );

        public async Task<RpcResult<Balance>> GetBalanceAsync(Account owner, SuiStructTag coin_type = null)
            => await this._rpc_client.SendRpcRequestAsync<Balance>
               (
                   Methods.suix_getBalance.ToString(),
                   ArgumentBuilder.BuildArguments(owner.SuiAddress(), coin_type == null ? null : coin_type.ToString() ?? null)
               );

        public async Task<RpcResult<Balance>> GetBalanceAsync(SuiPublicKeyBase owner, SuiStructTag coin_type = null)
            => await this._rpc_client.SendRpcRequestAsync<Balance>
               (
                   Methods.suix_getBalance.ToString(),
                   ArgumentBuilder.BuildArguments(owner.ToSuiAddress(), coin_type == null ? null : coin_type.ToString() ?? null)
               );

        public async Task<RpcResult<Balance>> GetBalanceAsync(AccountAddress owner, SuiStructTag coin_type = null)
            => await this._rpc_client.SendRpcRequestAsync<Balance>
               (
                   Methods.suix_getBalance.ToString(),
                   ArgumentBuilder.BuildArguments(owner.KeyHex, coin_type == null ? null : coin_type.ToString() ?? null)
               );

        public async Task<RpcResult<CoinMetadata>> GetCoinMetadataAsync(SuiStructTag coin_type)
            => await this._rpc_client.SendRpcRequestAsync<CoinMetadata>
               (
                   Methods.suix_getCoinMetadata.ToString(),
                   ArgumentBuilder.BuildArguments(coin_type.ToString())
               );

        public async Task<RpcResult<CoinPage>> GetCoinsAsync(Account owner, SuiStructTag coin_type = null, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<CoinPage>
               (
                   Methods.suix_getCoins.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.SuiAddress(),
                       coin_type == null ? null : coin_type.ToString(),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<CoinPage>> GetCoinsAsync(SuiPublicKeyBase owner, SuiStructTag coin_type = null, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<CoinPage>
               (
                   Methods.suix_getCoins.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.ToSuiAddress(),
                       coin_type == null ? null : coin_type.ToString(),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<CoinPage>> GetCoinsAsync(AccountAddress owner, SuiStructTag coin_type = null, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<CoinPage>
               (
                   Methods.suix_getCoins.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.KeyHex,
                       coin_type == null ? null : coin_type.ToString(),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<TotalSupply>> GetTotalSupplyAsync(SuiStructTag coin_type)
            => await this._rpc_client.SendRpcRequestAsync<TotalSupply>
               (
                   Methods.suix_getTotalSupply.ToString(),
                   ArgumentBuilder.BuildArguments(coin_type)
               );

        #endregion

        #region IExtendedApi

        public async Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObjectAsync(Account parent_object_id, DynamicFieldNameInput name)
            => await this._rpc_client.SendRpcRequestAsync<ObjectDataResponse>
               (
                   Methods.suix_getDynamicFieldObject.ToString(),
                   ArgumentBuilder.BuildArguments(parent_object_id.SuiAddress(), name)
               );

        public async Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObjectAsync(SuiPublicKeyBase parent_object_id, DynamicFieldNameInput name)
            => await this._rpc_client.SendRpcRequestAsync<ObjectDataResponse>
               (
                   Methods.suix_getDynamicFieldObject.ToString(),
                   ArgumentBuilder.BuildArguments(parent_object_id.ToSuiAddress(), name)
               );

        public async Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObjectAsync(AccountAddress parent_object_id, DynamicFieldNameInput name)
            => await this._rpc_client.SendRpcRequestAsync<ObjectDataResponse>
               (
                   Methods.suix_getDynamicFieldObject.ToString(),
                   ArgumentBuilder.BuildArguments(parent_object_id.KeyHex, name)
               );

        public async Task<RpcResult<PaginatedDynamicFieldInfo>> GetDynamicFieldsAsync(AccountAddress parent_object_id, ObjectQuery filter = null)
            => await this._rpc_client.SendRpcRequestAsync<PaginatedDynamicFieldInfo>
               (
                   Methods.suix_getDynamicFields.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       parent_object_id.KeyHex,
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null,
                       filter == null ? null : filter.ObjectDataFilter ?? null,
                       filter == null ? null : filter.ObjectDataOptions ?? null
                   )
               );

        public async Task<RpcResult<PaginatedObjectDataResponse>> GetOwnedObjectsAsync(Account owner, ObjectQuery filter = null)
            => await this._rpc_client.SendRpcRequestAsync<PaginatedObjectDataResponse>
               (
                   Methods.suix_getOwnedObjects.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.SuiAddress(),
                       new ObjectResponseQuery
                       (
                           filter == null ? null : filter.ObjectDataFilter ?? null,
                           filter == null ? null : filter.ObjectDataOptions ?? null
                       ),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<PaginatedObjectDataResponse>> GetOwnedObjectsAsync(SuiPublicKeyBase owner, ObjectQuery filter = null)
            => await this._rpc_client.SendRpcRequestAsync<PaginatedObjectDataResponse>
               (
                   Methods.suix_getOwnedObjects.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.ToSuiAddress(),
                       new ObjectResponseQuery
                       (
                           filter == null ? null : filter.ObjectDataFilter ?? null,
                           filter == null ? null : filter.ObjectDataOptions ?? null
                       ),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<PaginatedObjectDataResponse>> GetOwnedObjectsAsync(AccountAddress owner, ObjectQuery filter = null)
            => await this._rpc_client.SendRpcRequestAsync<PaginatedObjectDataResponse>
               (
                   Methods.suix_getOwnedObjects.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       owner.KeyHex,
                       new ObjectResponseQuery
                       (
                           filter == null ? null : filter.ObjectDataFilter ?? null,
                           filter == null ? null : filter.ObjectDataOptions ?? null
                       ),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<PaginatedEvent>> QueryEventsAsync(EventQuery query = null)
            => await this._rpc_client.SendRpcRequestAsync<PaginatedEvent>
               (
                   Methods.suix_queryEvents.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       query == null ? new AllEventFilter() : query.EventFilter ?? new AllEventFilter(),
                       query == null ? null : query.Cursor ?? null,
                       query == null ? null : query.Limit ?? null,
                       query == null ? null : query.Order == null ? null : query.Order == SortOrder.Descending
                   )
               );

        public async Task<RpcResult<TransactionBlockResponsePage>> QueryTransactionBlocksAsync(TransactionBlockResponseQueryInput query = null)
            => await this._rpc_client.SendRpcRequestAsync<TransactionBlockResponsePage>
               (
                   Methods.suix_queryTransactionBlocks.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       new TransactionBlockResponseQuery
                       (
                           query == null ? null : query.TransactionFilter ?? null,
                           query == null ? null : query.TransactionBlockResponseOptions ?? null
                       ),
                       query == null ? null : query.Cursor ?? null,
                       query == null ? null : query.Limit ?? null,
                       query == null ? null : query.Order == null ? null : query.Order == SortOrder.Descending
                   )
               );

        public async Task<RpcResult<AccountAddress>> ResolveNameServiceAddressAsync(string name)
            => await this._rpc_client.SendRpcRequestAsync<AccountAddress>
               (
                   Methods.suix_resolveNameServiceAddress.ToString(),
                   ArgumentBuilder.BuildArguments(name)
               );

        public async Task<RpcResult<NameServicePage>> ResolveNameServiceNamesAsync(Account address, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<NameServicePage>
               (
                   Methods.suix_resolveNameServiceNames.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       address.SuiAddress(),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<NameServicePage>> ResolveNameServiceNamesAsync(SuiPublicKeyBase address, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<NameServicePage>
               (
                   Methods.suix_resolveNameServiceNames.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       address.ToSuiAddress(),
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        public async Task<RpcResult<NameServicePage>> ResolveNameServiceNamesAsync(AccountAddress address, SuiRpcFilter filter = null)
            => await this._rpc_client.SendRpcRequestAsync<NameServicePage>
               (
                   Methods.suix_resolveNameServiceNames.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       address.KeyHex,
                       filter == null ? null : filter.Cursor ?? null,
                       filter == null ? null : filter.Limit ?? null
                   )
               );

        #endregion

        #region IGovernanceReadApi

        public async Task<RpcResult<CommitteeInfo>> GetCommitteeInfoAsync(BigInteger? epoch)
            => await this._rpc_client.SendRpcRequestAsync<CommitteeInfo>
               (
                   Methods.suix_getCommitteeInfo.ToString(),
                   ArgumentBuilder.BuildArguments(epoch == null ? null : epoch.ToString() ?? null)
               );

        public async Task<RpcResult<SuiSystemSummary>> GetLatestSuiSystemStateAsync()
            => await this._rpc_client.SendRpcRequestAsync<SuiSystemSummary>
               (
                   Methods.suix_getLatestSuiSystemState.ToString()
               );

        public async Task<RpcResult<BigInteger>> GetReferenceGasPriceAsync()
            => await this._rpc_client.SendRpcRequestAsync<BigInteger>
               (
                   Methods.suix_getReferenceGasPrice.ToString()
               );

        public async Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesAsync(Account owner)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<DelegatedStake>>
               (
                   Methods.suix_getStakes.ToString(),
                   ArgumentBuilder.BuildArguments(owner.SuiAddress())
               );

        public async Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesAsync(SuiPublicKeyBase owner)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<DelegatedStake>>
               (
                   Methods.suix_getStakes.ToString(),
                   ArgumentBuilder.BuildArguments(owner.ToSuiAddress())
               );

        public async Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesAsync(AccountAddress owner)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<DelegatedStake>>
               (
                   Methods.suix_getStakes.ToString(),
                   ArgumentBuilder.BuildArguments(owner.KeyHex)
               );

        public async Task<RpcResult<IEnumerable<DelegatedStake>>> GetStakesByIdsAsync(List<AccountAddress> staked_sui_ids)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<DelegatedStake>>
               (
                   Methods.suix_getStakesByIds.ToString(),
                   ArgumentBuilder.BuildArguments(staked_sui_ids.Select(id => id.KeyHex))
               );

        public async Task<RpcResult<ValidatorsApy>> GetValidatorsApyAsync()
            => await this._rpc_client.SendRpcRequestAsync<ValidatorsApy>
               (
                   Methods.suix_getValidatorsApy.ToString()
               );

        #endregion

        #region IMoveUtils

        public async Task<RpcResult<IEnumerable<MoveFunctionArgType>>> GetMoveFunctionArgTypesAsync(SuiStructTag struct_tag)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<MoveFunctionArgType>>
               (
                   Methods.sui_getMoveFunctionArgTypes.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       struct_tag.Address.KeyHex,
                       struct_tag.Module,
                       struct_tag.Name
                   )
               );

        public async Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunctionAsync(SuiStructTag struct_tag)
            => await this._rpc_client.SendRpcRequestAsync<NormalizedMoveFunctionResponse>
               (
                   Methods.sui_getNormalizedMoveFunction.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       struct_tag.Address.KeyHex,
                       struct_tag.Module,
                       struct_tag.Name
                   )
               );

        public async Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunctionAsync(SuiMoveNormalizedStructType struct_tag)
            => await this._rpc_client.SendRpcRequestAsync<NormalizedMoveFunctionResponse>
               (
                   Methods.sui_getNormalizedMoveFunction.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       struct_tag.Address.KeyHex,
                       struct_tag.Module,
                       struct_tag.Name
                   )
               );

        public async Task<RpcResult<SuiMoveNormalizedModule>> GetNormalizedMoveModuleAsync(AccountAddress package, string module_name)
            => await this._rpc_client.SendRpcRequestAsync<SuiMoveNormalizedModule>
               (
                   Methods.sui_getNormalizedMoveModule.ToString(),
                   ArgumentBuilder.BuildArguments(package.KeyHex, module_name)
               );

        public async Task<RpcResult<Dictionary<string, SuiMoveNormalizedModule>>> GetNormalizedMoveModulesByPackageAsync(AccountAddress package)
            => await this._rpc_client.SendRpcRequestAsync<Dictionary<string, SuiMoveNormalizedModule>>
               (
                   Methods.sui_getNormalizedMoveModulesByPackage.ToString(),
                   ArgumentBuilder.BuildArguments(package.KeyHex)
               );

        public async Task<RpcResult<SuiMoveNormalizedStruct>> GetNormalizedMoveStructAsync(SuiStructTag struct_tag)
            => await this._rpc_client.SendRpcRequestAsync<SuiMoveNormalizedStruct>
               (
                   Methods.sui_getNormalizedMoveStruct.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       struct_tag.Address.KeyHex,
                       struct_tag.Module,
                       struct_tag.Name
                   )
               );

        #endregion

        #region IReadApi

        public async Task<RpcResult<string>> GetChainIdentifierAsync()
            => await this._rpc_client.SendRpcRequestAsync<string>
               (
                   Methods.sui_getChainIdentifier.ToString()
               );

        public async Task<RpcResult<Checkpoint>> GetCheckpointAsync(string id)
            => await this._rpc_client.SendRpcRequestAsync<Checkpoint>
               (
                   Methods.sui_getCheckpoint.ToString(),
                   ArgumentBuilder.BuildArguments(id)
               );

        public async Task<RpcResult<Checkpoint>> GetCheckpointAsync(UInt53 id)
            => await this._rpc_client.SendRpcRequestAsync<Checkpoint>
               (
                   Methods.sui_getCheckpoint.ToString(),
                   ArgumentBuilder.BuildArguments(id)
               );

        public async Task<RpcResult<PaginatedCheckpoint>> GetCheckpointsAsync(SuiRpcFilter filter = null)
        {
            return await this._rpc_client.SendRpcRequestAsync<PaginatedCheckpoint>(
                Methods.sui_getCheckpoints.ToString(),
                ArgumentBuilder.BuildArguments
                (
                    filter == null ? null : filter.Cursor ?? null,
                    filter == null ? null : filter.Limit ?? null,
                    filter == null ? null : filter.Order == null ? null : filter.Order == SortOrder.Descending
                )
            );
        }

        public async Task<RpcResult<PaginatedEvent>> GetEventsAsync(string transaction_digest)
            => await this._rpc_client.SendRpcRequestAsync<PaginatedEvent>
               (
                   Methods.sui_getEvents.ToString(),
                   ArgumentBuilder.BuildArguments(transaction_digest)
               );

        public async Task<RpcResult<string>> GetLatestCheckpointSequenceNumberAsync()
            => await this._rpc_client.SendRpcRequestAsync<string>
               (
                   Methods.sui_getLatestCheckpointSequenceNumber.ToString()
               );

        public async Task<RpcResult<ObjectDataResponse>> GetObjectAsync(AccountAddress object_id, ObjectDataOptions options = null)
            => await this._rpc_client.SendRpcRequestAsync<ObjectDataResponse>
               (
                   Methods.sui_getObject.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       object_id.KeyHex,
                       options
                   )
               );

        public async Task<RpcResult<ProtocolConfig>> GetProtocolConfigAsync(BigInteger? version = null)
            => await this._rpc_client.SendRpcRequestAsync<ProtocolConfig>
               (
                   Methods.sui_getProtocolConfig.ToString(),
                   ArgumentBuilder.BuildArguments(version == null ? null : version.ToString())
               );

        public async Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync()
            => await this._rpc_client.SendRpcRequestAsync<BigInteger>
               (
                   Methods.sui_getTotalTransactionBlocks.ToString()
               );

        public async Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest, TransactionBlockResponseOptions options = null)
            => Utils.IsValidTransactionDigest(digest) == false ?
                   RpcResult<TransactionBlockResponse>.GetErrorResult("Invalid digest.") :
                   await this._rpc_client.SendRpcRequestAsync<TransactionBlockResponse>
                   (
                       Methods.sui_getTransactionBlock.ToString(),
                       ArgumentBuilder.BuildArguments(digest, options)
                   );

        public async Task<RpcResult<IEnumerable<ObjectDataResponse>>> MultiGetObjectsAsync(List<AccountAddress> object_ids, ObjectDataOptions options = null)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<ObjectDataResponse>>
               (
                   Methods.sui_multiGetObjects.ToString(),
                   ArgumentBuilder.BuildArguments(object_ids.Select(id => id.KeyHex), options)
               );

        public async Task<RpcResult<IEnumerable<TransactionBlockResponse>>> MultiGetTransactionBlocksAsync(List<string> digests, TransactionBlockResponseOptions options = null)
            => Utils.AreValidTransactionDigests(digests) == false ?
                   RpcResult<IEnumerable<TransactionBlockResponse>>.GetErrorResult("Invalid digests.") :
                   await this._rpc_client.SendRpcRequestAsync<IEnumerable<TransactionBlockResponse>>
                   (
                       Methods.sui_multiGetTransactionBlocks.ToString(),
                       ArgumentBuilder.BuildArguments(digests, options)
                   );

        public async Task<RpcResult<ObjectRead>> TryGetPastObjectAsync(AccountAddress object_id, BigInteger? version = null, ObjectDataOptions options = null)
            => await this._rpc_client.SendRpcRequestAsync<ObjectRead>
               (
                   Methods.sui_tryGetPastObject.ToString(),
                   ArgumentBuilder.BuildArguments(object_id.KeyHex, version == null ? null : (ulong)version, options)
               );

        public async Task<RpcResult<IEnumerable<ObjectRead>>> TryMultiGetPastObjectsAsync(List<PastObjectsInput> objects, ObjectDataOptions options = null)
            => await this._rpc_client.SendRpcRequestAsync<IEnumerable<ObjectRead>>
                (
                    Methods.sui_tryMultiGetPastObjects.ToString(),
                    ArgumentBuilder.BuildArguments
                    (
                        objects.Select(obj => obj.ToQueryObject()),
                        options
                    )
                );

        #endregion

        #region IWriteApi

        public async Task<RpcResult<DevInspectResponse>> DevInspectTransactionBlockAsync
        (
            Account sender,
            TransactionBlock transaction_block,
            BigInteger? gas_price = null,
            BigInteger? epoch = null,
            DevInspectArgs additional_args = null
        )
        {
            string sender_address = sender.SuiAddress().KeyHex;
            transaction_block.SetSenderIfNotSet(AccountAddress.FromHex(sender_address));
            byte[] tx_bytes = await transaction_block.Build(new BuildOptions(this, null, true));

            if (transaction_block.Error != null)
                return RpcResult<DevInspectResponse>.GetErrorResult(transaction_block.Error.Message);

            string dev_inspect_tx_bytes = CryptoBytes.ToBase64String(tx_bytes);
            return await this._rpc_client.SendRpcRequestAsync<DevInspectResponse>
            (
                Methods.sui_devInspectTransactionBlock.ToString(),
                ArgumentBuilder.BuildArguments
                (
                    sender_address,
                    dev_inspect_tx_bytes,
                    gas_price == null ? null : gas_price.ToString(),
                    epoch == null ? null : epoch.ToString(),
                    additional_args
                )
            );
        }

        public async Task<RpcResult<TransactionBlockResponse>> DryRunTransactionBlockAsync(string tx_bytes)
            => await this._rpc_client.SendRpcRequestAsync<TransactionBlockResponse>
               (
                   Methods.sui_dryRunTransactionBlock.ToString(),
                   ArgumentBuilder.BuildArguments(tx_bytes)
               );

        public async Task<RpcResult<TransactionBlockResponse>> ExecuteTransactionBlockAsync(byte[] tx_bytes, List<string> signature, TransactionBlockResponseOptions options = null, RequestType? request_type = null)
            => await this._rpc_client.SendRpcRequestAsync<TransactionBlockResponse>
               (
                   Methods.sui_executeTransactionBlock.ToString(),
                   ArgumentBuilder.BuildArguments
                   (
                       tx_bytes,
                       signature.ToArray(),
                       options,
                       request_type == null ? null : request_type.ToString() ?? null
                   )
               );

        #endregion

        #region Helper Functions

        /// <summary>
        /// Signs and executes a transaction.
        /// </summary>
        /// <param name="transaction_block">The transaction block to execute.</param>
        /// <param name="account">The account to sign the transaction.</param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <param name="request_type">The request type, derived from SuiTransactionBlockResponseOptions if None.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `TransactionBlockResponse` object.</returns>
        public async Task<RpcResult<TransactionBlockResponse>> SignAndExecuteTransactionBlockAsync
        (
            TransactionBlock transaction_block,
            Account account,
            TransactionBlockResponseOptions options = null,
            RequestType? request_type = null
        )
        {
            transaction_block.SetSenderIfNotSet(AccountAddress.FromHex(account.SuiAddress().KeyHex));
            byte[] tx_bytes = await transaction_block.Build(new BuildOptions(this));

            if (transaction_block.Error != null)
                return RpcResult<TransactionBlockResponse>.GetErrorResult(transaction_block.Error.Message);

            SignatureBase signature = account.SignTransactionBlock(tx_bytes);
            SuiResult<string> signature_result = account.ToSerializedSignature(signature);

            if (signature_result.Error != null)
                return RpcResult<TransactionBlockResponse>.GetErrorResult(signature_result.Error.Message);

            return await this.ExecuteTransactionBlockAsync
            (
                tx_bytes,
                new List<string> { signature_result.Result },
                options,
                request_type
            );
        }

        /// <summary>
        /// Waits for a transaction to fully execute.
        /// </summary>
        /// <param name="transaction">The transaction's digest.</param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `TransactionBlockResponse` object.</returns>
        public async Task<RpcResult<TransactionBlockResponse>> WaitForTransaction(string transaction, TransactionBlockResponseOptions options = null)
        {
            int count = 0;
            bool is_done = false;

            while (count == 0 && is_done == false)
            {
                if (count >= 60)
                    return RpcResult<TransactionBlockResponse>.GetErrorResult("Transaction Timed Out");

                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));

                SuiResult<bool> is_done_result = await this.IsValidTransactionBlock(transaction);

                if (is_done_result.Error != null)
                    return RpcResult<TransactionBlockResponse>.GetErrorResult(is_done_result.Error.Message);

                is_done = is_done_result.Result;
                count += 1;
            }

            return await this.GetTransactionBlockAsync(transaction, options);
        }

        /// <summary>
        /// Verifies whether or not the transaction digest points to a valid and fully executed transaction on the Sui Network.
        /// </summary>
        /// <param name="transaction">The transaction digest.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `bool` value.</returns>
        private async Task<SuiResult<bool>> IsValidTransactionBlock(string transaction)
        {
            RpcResult<TransactionBlockResponse> result = await this.GetTransactionBlockAsync(transaction);

            if (result.Error != null)
                return new SuiResult<bool>(false, new SuiError(result.Error.Code, result.Error.Message, result.Error.Data));

            return new SuiResult<bool>(result.Result.TimestampMs != null);
        }

        #endregion
    }
}