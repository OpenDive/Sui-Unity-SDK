//
//  IExtendedApi.cs
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

using System.Threading.Tasks;
using Sui.Accounts;
using Sui.Cryptography;
using Sui.Rpc.Models;
using Sui.Rpc;

namespace Sui.Clients.Api
{
    public interface IExtendedApi
    {
        #region suix_getDynamicFieldObject

        ///// <summary>
        ///// Return the dynamic field object information for a specified object
        ///// https://docs.sui.io/sui-api-ref#suix_getdynamicfieldobject
        ///// </summary>
        ///// <param name="parent_object_id">The ID of the queried parent object.</param>
        ///// <param name="name">The Name of the dynamic field.</param>
        ///// <returns>An asynchronous task object containing the wrapped result of the `ObjectDataResponse` object.</returns>
        //public Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObjectAsync(AccountAddress parent_object_id, DynamicFieldNameInput name);

        /// <summary>
        /// Return the dynamic field object information for a specified object
        /// https://docs.sui.io/sui-api-ref#suix_getdynamicfieldobject
        /// </summary>
        /// <param name="parent_object_id">The ID of the queried parent object.</param>
        /// <param name="name">The Name of the dynamic field.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `ObjectDataResponse` object.</returns>
        public Task<RpcResult<ObjectDataResponse>> GetDynamicFieldObjectAsync(AccountAddress parent_object_id, DynamicFieldNameInput name);

        #endregion

        #region suix_getDynamicFields

        /// <summary>
        /// Return the list of dynamic field objects owned by an object.
        /// https://docs.sui.io/sui-api-ref#suix_getdynamicfields
        /// </summary>
        /// <param name="parent_object_id">The ID of the parent object.</param>
        /// <param name="filter">The query object used for filtering by object options and filters, cursor, and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `DynamicFieldPage` object.</returns>
        public Task<RpcResult<PaginatedDynamicFieldInfo>> GetDynamicFieldsAsync(AccountAddress parent_object_id, ObjectQuery filter = null);

        #endregion

        #region suix_getOwnedObjects

        /// <summary>
        /// Return the list of objects owned by an address. Note that if the address owns more than QUERY_MAX_RESULT_LIMIT objects, the
        /// pagination is not accurate, because previous page may have been updated when the next page is fetched. Please use suix_queryObjects if
        /// this is a concern.
        /// </summary>
        /// <param name="owner">The owner's Sui address</param>
        /// <param name="filter">The query object used for filtering by object options and filters, cursor, and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `PaginatedObjectsResponse` object.</returns>
        public Task<RpcResult<PaginatedObjectDataResponse>> GetOwnedObjectsAsync(Account owner, ObjectQuery filter = null);

        /// <summary>
        /// Return the list of objects owned by an address. Note that if the address owns more than QUERY_MAX_RESULT_LIMIT objects, the
        /// pagination is not accurate, because previous page may have been updated when the next page is fetched. Please use suix_queryObjects if
        /// this is a concern.
        /// </summary>
        /// <param name="owner">The owner's Sui address</param>
        /// <param name="filter">The query object used for filtering by object options and filters, cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `PaginatedObjectsResponse` object.</returns>
        public Task<RpcResult<PaginatedObjectDataResponse>> GetOwnedObjectsAsync(SuiPublicKeyBase owner, ObjectQuery filter = null);

        /// <summary>
        /// Return the list of objects owned by an address. Note that if the address owns more than QUERY_MAX_RESULT_LIMIT objects, the
        /// pagination is not accurate, because previous page may have been updated when the next page is fetched. Please use suix_queryObjects if
        /// this is a concern.
        /// </summary>
        /// <param name="owner">The owner's Sui address</param>
        /// <param name="filter">The query object used for filtering by object options and filters, cursor, and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `PaginatedObjectsResponse` object.</returns>
        public Task<RpcResult<PaginatedObjectDataResponse>> GetOwnedObjectsAsync(AccountAddress owner, ObjectQuery filter = null);

        #endregion

        #region suix_queryEvents

        /// <summary>
        /// Return list of events for a specified query criteria.
        /// https://docs.sui.io/sui-api-ref#suix_queryevents
        /// </summary>
        /// <param name="query">The query object used for filtering by event filters, cursor, and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `EventPage` object.</returns>
        public Task<RpcResult<PaginatedEvent>> QueryEventsAsync(EventQuery query = null);

        #endregion

        #region suix_queryTransactionBlocks

        /// <summary>
        /// Return list of transactions for a specified query criteria.
        /// </summary>
        /// <param name="query">
        /// The query object used for filtering by transaction filters
        /// and options, cursor, order, and limit.
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of the `TransactionBlockResponsePage` object.</returns>
        public Task<RpcResult<TransactionBlockResponsePage>> QueryTransactionBlocksAsync(TransactionBlockResponseQueryInput query = null);

        #endregion

        #region suix_resolveNameServiceAddress

        /// <summary>
        /// Return the resolved address given resolver and name.
        /// https://docs.sui.io/sui-api-ref#suix_resolvenameserviceaddress
        /// </summary>
        /// <param name="name">The name to resolve, for example: "example.sui"</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `AccountAddress` object.</returns>
        public Task<RpcResult<AccountAddress>> ResolveNameServiceAddressAsync(string name);

        #endregion

        #region suix_resolveNameServiceNames

        /// <summary>
        /// Return the resolved names given address, if multiple names are resolved, the first one is the primary name.
        /// https://docs.sui.io/sui-api-ref#suix_resolvenameservicenames
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `AccountAddress` object.</returns>
        public Task<RpcResult<NameServicePage>> ResolveNameServiceNamesAsync(Account address, SuiRpcFilter filter = null);

        /// <summary>
        /// Return the resolved names given address, if multiple names are resolved, the first one is the primary name.
        /// https://docs.sui.io/sui-api-ref#suix_resolvenameservicenames
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `AccountAddress` object.</returns>
        public Task<RpcResult<NameServicePage>> ResolveNameServiceNamesAsync(SuiPublicKeyBase address, SuiRpcFilter filter = null);

        /// <summary>
        /// Return the resolved names given address, if multiple names are resolved, the first one is the primary name.
        /// https://docs.sui.io/sui-api-ref#suix_resolvenameservicenames
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `AccountAddress` object.</returns>
        public Task<RpcResult<NameServicePage>> ResolveNameServiceNamesAsync(AccountAddress address, SuiRpcFilter filter = null);

        #endregion

        #region suix_subscribeEvent

        // TODO: Implement Subscribe Event endpoint.
        // https://docs.sui.io/sui-api-ref#suix_subscribeevent

        #endregion

        #region suix_subscribeTransaction

        // TODO: Implement Subscribe Transaction endpoint.
        // https://docs.sui.io/sui-api-ref#suix_subscribetransaction

        #endregion
    }
}