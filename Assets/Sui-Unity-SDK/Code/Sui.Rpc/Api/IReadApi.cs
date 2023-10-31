using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sui.Accounts;
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

        /// <summary>
        /// Return the sequence number of the latest checkpoint that has been executed
        /// https://docs.sui.io/sui-jsonrpc#sui_getLatestCheckpointSequenceNumber
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<string>> GetLatestCheckpointSequenceNumber();

        /// <summary>
        /// Return a structured representation of Move module
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveModule
        /// </summary>
        /// <param name="package"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        Task<RpcResult<SuiMoveNormalizedModule>> GetNormalizedMoveModule(string package, string moduleName);

        /// <summary>
        /// Return transaction events.
        /// https://docs.sui.io/sui-jsonrpc#sui_getEvents
        /// </summary>
        /// <param name="transactionDigest">The event query criteria.</param>
        /// <returns></returns>
        Task<RpcResult<Models.Event[]>> GetEvents(string transactionDigest);

        /// <summary>
        /// Return structured representations of all modules in the given package
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveModulesByPackage
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        Task<RpcResult<Dictionary<string, SuiMoveNormalizedModule>>> GetNormalizedMoveModulesByPackage(string package);

        /// <summary>
        /// Return the argument types of a Move function, based on normalized Type.
        /// https://docs.sui.io/sui-jsonrpc#sui_getMoveFunctionArgTypes
        /// </summary>
        /// <param name="package"></param>
        /// <param name="module"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        Task<RpcResult<MoveFunctionArgTypes>> GetMoveFunctionArgTypes(string package, string module, string function);

        /// <summary>
        /// Return a structured representation of Move struct
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveStruct
        /// </summary>
        /// <param name="package"></param>
        /// <param name="moduleName"></param>
        /// <param name="structName"></param>
        /// <returns></returns>
        Task<RpcResult<SuiMoveNormalizedStruct>> GetNormalizedMoveStruct(string package, string moduleName, string structName);

        /// <summary>
        /// Gets loaded child objects associated with the transaction the request provides.
        /// https://docs.sui.io/sui-jsonrpc#sui_getLoadedChildObjects
        /// </summary>
        /// <param name="digest"></param>
        /// <returns></returns>
        Task<RpcResult<ChildObjects>> GetLoadedChildObjects(string digest);

        /// <summary>
        /// Runs the transaction in dev-inspect mode. Which allows for nearly any transaction (or Move call) with any arguments. Detailed results are provided,
        /// including both the transaction effects and any return values.
        /// https://docs.sui.io/sui-jsonrpc#sui_devInspectTransactionBlock
        /// </summary>
        /// <param name="senderAddress"></param>
        /// <param name="txBytes">BCS encoded TransactionKind(as opposed to TransactionData, which include gasBudget and gasPrice)</param>
        /// <param name="gasPrice">Gas is not charged, but gas usage is still calculated. Default to use reference gas price</param>
        /// <param name="epoch">The epoch to perform the call. Will be set from the system state object if not provided</param>
        /// <returns></returns>
        Task<RpcResult<DevInspectResponse>> DevInspectTransactionBlock(AccountAddress senderAddress, string txBytes, string gasPrice, string epoch);

        /// <summary>
        /// Return the dynamic field object information for a specified object
        /// https://docs.sui.io/sui-jsonrpc#suix_getDynamicFieldObject
        /// </summary>
        /// <param name="parentObjectId">The ID of the queried parent object</param>
        /// <param name="name">The Name of the dynamic field</param>
        /// <returns></returns>
        Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObject(string parentObjectId, DynamicFieldName name);

        /// <summary>
        /// Return the list of dynamic field objects owned by an object.
        /// https://docs.sui.io/sui-jsonrpc#suix_getDynamicFields
        /// </summary>
        /// <param name="parentObjectId">The ID of the parent object</param>
        /// <param name="cursor">An optional paging cursor. If provided, the query will start from the next item after the specified cursor. Default to start from the first item if not specified.</param>
        /// <param name="limit">Maximum item returned per page, default to [QUERY_MAX_RESULT_LIMIT] if not specified.</param>
        /// <returns></returns>
        Task<RpcResult<DynamicFieldPage>> GetDynamicFields(string parentObjectId, int limit, string cursor);

        /// <summary>
        /// Note there is no software-level guarantee/SLA that objects with past versions can be retrieved by this API, even if the object and version exists/existed. The result may vary across nodes depending on their pruning policies. Return the object information for a specified version
        /// https://docs.sui.io/sui-jsonrpc#sui_tryGetPastObject
        /// </summary>
        /// <param name="objectId">the ID of the queried object</param>
        /// <param name="options">the version of the queried object. If None, default to the latest known version</param>
        /// <param name="version">options for specifying the content to be returned</param>
        /// <returns></returns>
        Task<RpcResult<PastObject>> TryGetPastObject(AccountAddress objectId, ObjectDataOptions options, string version);

        /// <summary>
        /// Note there is no software-level guarantee/SLA that objects with past versions can be retrieved by this API, even if the object and version exists/existed. The result may vary across nodes depending on their pruning policies. Return the object information for a specified version
        /// https://docs.sui.io/sui-jsonrpc#sui_tryMultiGetPastObjects
        /// </summary>
        /// <param name="pastObjects">a vector of object and versions to be queried</param>
        /// <param name="options">options for specifying the content to be returned</param>
        /// <returns></returns>
        Task<RpcResult<PastObject[]>> TryMultiGetPastObjects(PastObjectRequest pastObjects, ObjectDataOptions options);

        /// <summary>
        /// Return the object data for a list of objects
        /// https://docs.sui.io/sui-jsonrpc#sui_multiGetObjects
        /// </summary>
        /// <param name="objectIds">the IDs of the queried objects</param>
        /// <param name="options">options for specifying the content to be returned</param>
        /// <returns></returns>
        Task<RpcResult<IEnumerable<ObjectDataResponse>>> MultiGetObjects(AccountAddress[] objectIds, ObjectDataOptions options);

        /// <summary>
        /// Return the object information for a specified object
        /// https://docs.sui.io/sui-jsonrpc#sui_getObject
        /// </summary>
        /// <param name="objectId">the ID of the queried object</param>
        /// <param name="options">options for specifying the content to be returned</param>
        /// <returns></returns>
        Task<RpcResult<ObjectDataResponse>> GetObject(AccountAddress objectId, ObjectDataOptions options);
    }
}