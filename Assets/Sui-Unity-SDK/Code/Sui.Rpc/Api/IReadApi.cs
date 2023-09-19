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
        /// <returns></returns>
        Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunction();

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
    }
}