//
//  IReadApi.cs
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
using System.Threading.Tasks;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Utilities;

namespace Sui.Clients.Api
{
    public interface IReadApi
    {
        #region sui_getChainIdentifier

        /// <summary>
        /// Return the first four bytes of the chain's genesis checkpoint digest.
        /// https://docs.sui.io/sui-jsonrpc#sui_getChainIdentifier
        /// </summary>
        /// <returns>An asynchronous task object containing the wrapped result of a `string` object.</returns>
        public Task<RpcResult<string>> GetChainIdentifierAsync();

        #endregion

        #region sui_getCheckpoint

        /// <summary>
        /// Return a checkpoint
        /// https://docs.sui.io/sui-jsonrpc#sui_getCheckpoint
        /// </summary>
        /// <param name="id">Checkpoint identifier, can use either checkpoint digest, or checkpoint sequence number as input.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `Checkpoint` object.</returns>
        public Task<RpcResult<Checkpoint>> GetCheckpointAsync(string id);

        /// <summary>
        /// Return a checkpoint
        /// https://docs.sui.io/sui-jsonrpc#sui_getCheckpoint
        /// </summary>
        /// <param name="id">Checkpoint identifier, can use either checkpoint digest, or checkpoint sequence number as input.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `Checkpoint` object.</returns>
        public Task<RpcResult<Checkpoint>> GetCheckpointAsync(UInt53 id);

        #endregion

        #region sui_getCheckpoints

        /// <summary>
        /// Return paginated list of checkpoints
        /// https://docs.sui.io/sui-jsonrpc#sui_getCheckpoints
        /// </summary>
        /// <param name="filter">
        /// <para>Provides the following:</para>
        /// <para>- Cursor: An optional paging cursor. If provided, the query will start from the next item after the specified cursor. Default to start from the first item if not specified.</para>
        /// <para>- Limit: Maximum item returned per page, default to [QUERY_MAX_RESULT_LIMIT_CHECKPOINTS] if not specified.</para>
        /// <para>- Order: Query result ordering, default to false (ascending order), oldest record first.</para>
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of a `Checkpoints` object.</returns>
        public Task<RpcResult<PaginatedCheckpoint>> GetCheckpointsAsync(SuiRpcFilter filter = null);

        #endregion

        #region sui_getEvents

        /// <summary>
        /// Return transaction events.
        /// https://docs.sui.io/sui-jsonrpc#sui_getEvents
        /// </summary>
        /// <param name="transaction_digest">The event query criteria.</param>
        /// <returns>An asynchronous task object containing the wrapped result of an `EventPage` object.</returns>
        public Task<RpcResult<PaginatedEvent>> GetEventsAsync(string transaction_digest);

        #endregion

        #region sui_getLatestCheckpointSequenceNumber

        /// <summary>
        /// Return the sequence number of the latest checkpoint that has been executed
        /// https://docs.sui.io/sui-jsonrpc#sui_getLatestCheckpointSequenceNumber
        /// </summary>
        /// <returns>An asynchronous task object containing the wrapped result of a `string` value.</returns>
        public Task<RpcResult<string>> GetLatestCheckpointSequenceNumberAsync();

        #endregion

        #region sui_getObject

        /// <summary>
        /// Return the object information for a specified object.
        /// https://docs.sui.io/sui-jsonrpc#sui_getObject
        /// </summary>
        /// <param name="object_id">The ID of the queried object.</param>
        /// <param name="options">Options for specifying the content of the object to be returned.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `ObjectDataResponse` object.</returns>
        public Task<RpcResult<ObjectDataResponse>> GetObjectAsync(AccountAddress object_id, ObjectDataOptions options = null);

        #endregion

        #region sui_getProtocolConfig

        /// <summary>
        /// Return the protocol config table for the given version number.
        /// If the version number is not specified, If none is specified,
        /// the node uses the version of the latest epoch it has processed.
        /// https://docs.sui.io/sui-api-ref#sui_getprotocolconfig
        /// </summary>
        /// <param name="version">
        /// An optional protocol version specifier. If omitted,
        /// the latest protocol config table for the node will be returned.
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of a `ProtocolConfig` object.</returns>
        public Task<RpcResult<ProtocolConfig>> GetProtocolConfigAsync(BigInteger? version = null);

        #endregion

        #region sui_getTotalTransactionBlocks

        /// <summary>
        /// Return the total number of transaction blocks known to the server.
        /// https://docs.sui.io/sui-jsonrpc#sui_getTotalTransactionBlocks
        /// </summary>
        /// <returns>An asynchronous task object containing the wrapped result of a `BigInteger` object.</returns>
        public Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync();

        #endregion

        #region sui_getTransactionBlock

        /// <summary>
        /// Return the transaction response object.
        /// https://docs.sui.io/sui-api-ref#sui_gettransactionblock
        /// </summary>
        /// <param name="digest">The digest of the queried transaction.</param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `TransactionBlockResponse` object.</returns>
        public Task<RpcResult<TransactionBlockResponse>> GetTransactionBlockAsync(string digest, TransactionBlockResponseOptions options = null);

        #endregion

        #region sui_multiGetObjects

        /// <summary>
        /// Return the object data for a list of objects.
        /// https://docs.sui.io/sui-jsonrpc#sui_multiGetObjects
        /// </summary>
        /// <param name="object_ids">The IDs of the queried objects.</param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <returns>An asynchronous task object containing the wrapped result of an IEnumerable `ObjectDataResponse`.</returns>
        public Task<RpcResult<IEnumerable<ObjectDataResponse>>> MultiGetObjectsAsync(List<AccountAddress> object_ids, ObjectDataOptions options = null);

        #endregion

        #region sui_multiGetTransactionBlocks

        /// <summary>
        /// Returns an ordered list of transaction responses The method will throw an error
        /// if the input contains any duplicate or the input size exceeds QUERY_MAX_RESULT_LIMIT.
        /// </summary>
        /// <param name="digests">A list of transaction digests.</param>
        /// <param name="options">Config options to control which fields to fetch</param>
        /// <returns>An asynchronous task object containing the wrapped result of an IEnumerable `TransactionBlockResponse`.</returns>
        public Task<RpcResult<IEnumerable<TransactionBlockResponse>>> MultiGetTransactionBlocksAsync(List<string> digests, TransactionBlockResponseOptions options = null);

        #endregion

        #region sui_tryGetPastObject

        /// <summary>
        /// <para>Note there is no software-level guarantee/SLA that objects with past versions can be retrieved by this API,
        /// even if the object and version exists/existed. The result may vary across nodes depending
        /// on their pruning policies.</para>
        /// 
        /// Return the object information for a specified version.
        /// https://docs.sui.io/sui-jsonrpc#sui_tryGetPastObject
        /// </summary>
        /// <param name="object_id">The ID of the queried object.</param>
        /// <param name="version">The version of the queried object. If None, default to the latest known version.</param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `ObjectRead` object.</returns>
        public Task<RpcResult<ObjectRead>> TryGetPastObjectAsync(AccountAddress object_id, BigInteger? version = null, ObjectDataOptions options = null);

        #endregion

        #region sui_tryMultiGetPastObjects

        /// <summary>
        /// <para>Note there is no software-level guarantee/SLA that objects with past versions can be retrieved by this API,
        /// even if the object and version exists/existed. The result may vary across nodes depending
        /// on their pruning policies.</para>
        ///
        /// Return the object information for a specified version.
        /// https://docs.sui.io/sui-jsonrpc#sui_tryMultiGetPastObjects
        /// </summary>
        /// <param name="objects">A vector of versions and objects to be queried.</param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <returns>An asynchronous task object containing the wrapped result of an IEnumerable `ObjectRead`.</returns>
        public Task<RpcResult<IEnumerable<ObjectRead>>> TryMultiGetPastObjectsAsync(List<PastObjectsInput> objects, ObjectDataOptions options = null);

        #endregion
    }
}