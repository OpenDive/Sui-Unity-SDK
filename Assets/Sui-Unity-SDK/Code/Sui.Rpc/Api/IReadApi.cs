using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sui.Rpc.Models;
using UnityEngine;

namespace Sui.Rpc.Api
{
    public interface IReadApi
    {
        /// <summary>
        /// Return the total number of transaction blocks known to the server.
        /// https://docs.sui.io/sui-jsonrpc#sui_getTotalTransactionBlocks
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<BigInteger>> GetTotalTransactionBlocksAsync();

        /// <summary>
        /// Return the protocol config table for the given version number.
        /// If the version number is not specified, If none is specified,
        /// the node uses the version of the latest epoch it has processed.
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<ProtocolConfig>> GetProtocolConfigAsync();

        /// <summary>
        /// Return the reference gas price for the network.
        /// https://docs.sui.io/sui-jsonrpc#suix_getReferenceGasPrice
        /// <code>
        /// {
        ///     "jsonrpc": "2.0",
        ///     "result": 1000
        /// }
        /// </code>
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<BigInteger>> GetReferenceGasPriceAsync();

        /// <summary>
        /// Return a structured representation of Move function
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveFunction
        /// </summary>
        /// <param name="package">ObjectID / package name</param>
        /// <param name="moduleName"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunction(string package, string moduleName, string functionName);

        /// <summary>
        /// TODO: Needs to be tested
        /// </summary>
        /// <param name="objectIds"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<RpcResult<IEnumerable<SuiObjectResponse>>> GetObjectsAsync(IEnumerable<string> objectIds, ObjectDataOptions options);

        /// <summary>
        /// Return all Coin<`coin_type`> objects owned by an address.
        /// https://docs.sui.io/sui-jsonrpc#suix_getCoins
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="coinType"></param>
        /// <param name="objectId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<RpcResult<CoinPage>> GetCoins(string owner, string coinType, string objectId, int limit);

        /// <summary>
        /// Return transaction execution effects including the gas cost summary,
        /// while the effects are not committed to the chain.
        /// https://docs.sui.io/sui-jsonrpc#sui_dryRunTransactionBlock
        /// </summary>
        /// <param name="txBytes"></param>
        /// <returns></returns>
        Task<RpcResult<TransactionBlockResponse>> DryRunTransactionBlock(string txBytesBase64);

        /// <summary>
        /// Return the first four bytes of the chain's genesis checkpoint digest.
        /// https://docs.sui.io/sui-jsonrpc#sui_getChainIdentifier
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<string>> GetChainIdentifier();

        /// <summary>
        /// Return a checkpoint
        /// https://docs.sui.io/sui-jsonrpc#sui_getCheckpoint
        /// </summary>
        /// <param name="id">Checkpoint identifier, can use either checkpoint digest, or checkpoint sequence number as input.</param>
        /// <returns></returns>
        Task<RpcResult<Checkpoint>> GetCheckpoint(string id);

        /// <summary>
        /// Return paginated list of checkpoints
        /// https://docs.sui.io/sui-jsonrpc#sui_getCheckpoints
        /// </summary>
        /// <param name="cursor">An optional paging cursor. If provided, the query will start from the next item after the specified cursor. Default to start from the first item if not specified.</param>
        /// <param name="limit">Maximum item returned per page, default to [QUERY_MAX_RESULT_LIMIT_CHECKPOINTS] if not specified.</param>
        /// <param name="descendingOrder">query result ordering, default to false (ascending order), oldest record first.</param>
        /// <returns></returns>
        Task<RpcResult<Checkpoints>> GetCheckpoints(string cursor, int limit, bool descendingOrder);
    }
}