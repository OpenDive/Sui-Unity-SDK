//
//  ICoinQueryApi.cs
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
using System.Threading.Tasks;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Cryptography;
using Sui.Rpc.Models;

namespace Sui.Rpc.Api
{
    public interface ICoinQueryApi
    {
        #region suix_getAllBalances

        /// <summary>
        /// Return the total coin balance for all coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getAllBalances
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Balance` object.</returns>
        public Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(Account owner);

        /// <summary>
        /// Return the total coin balance for all coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getAllBalances
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Balance` object.</returns>
        public Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(SuiPublicKeyBase owner);

        /// <summary>
        /// Return the total coin balance for all coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getAllBalances
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Balance` object.</returns>
        public Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(AccountAddress owner);

        #endregion

        #region suix_getAllCoins

        /// <summary>
        /// Return all Coin objects owned by an address.
        /// https://docs.sui.io/sui-api-ref#suix_getallcoins
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinPage` object.</returns>
        public Task<RpcResult<CoinPage>> GetAllCoinsAsync(Account owner, SuiRpcFilter filter = null);

        /// <summary>
        /// Return all Coin objects owned by an address.
        /// https://docs.sui.io/sui-api-ref#suix_getallcoins
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinPage` object.</returns>
        public Task<RpcResult<CoinPage>> GetAllCoinsAsync(SuiPublicKeyBase owner, SuiRpcFilter filter = null);

        /// <summary>
        /// Return all Coin objects owned by an address.
        /// https://docs.sui.io/sui-api-ref#suix_getallcoins
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinPage` object.</returns>
        public Task<RpcResult<CoinPage>> GetAllCoinsAsync(AccountAddress owner, SuiRpcFilter filter = null);

        #endregion

        #region suix_getBalance

        /// <summary>
        /// Return the total coin balance for one coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getBalance
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <param name="coin_type">
        /// Optional type names for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC),
        /// default to 0x2::sui::SUI if not specified.
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Balance` object.</returns>
        public Task<RpcResult<Balance>> GetBalanceAsync(Account owner, SuiStructTag coin_type = null);

        /// <summary>
        /// Return the total coin balance for one coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getBalance
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <param name="coin_type">
        /// Optional type names for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC),
        /// default to 0x2::sui::SUI if not specified.
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Balance` object.</returns>
        public Task<RpcResult<Balance>> GetBalanceAsync(SuiPublicKeyBase owner, SuiStructTag coin_type = null);

        /// <summary>
        /// Return the total coin balance for one coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getBalance
        /// </summary>
        /// <param name="owner">The owner's Sui address.</param>
        /// <param name="coin_type">
        /// Optional type names for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC),
        /// default to 0x2::sui::SUI if not specified.
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Balance` object.</returns>
        public Task<RpcResult<Balance>> GetBalanceAsync(AccountAddress owner, SuiStructTag coin_type = null);

        #endregion

        #region suix_getCoinMetadata

        /// <summary>
        /// Return metadata(e.g., symbol, decimals) for a coin.
        /// https://docs.sui.io/sui-jsonrpc#suix_getCoinMetadata
        /// </summary>
        /// <param name="coin_type">
        /// Type name for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC)
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinMetadata` object.</returns>
        public Task<RpcResult<CoinMetadata>> GetCoinMetadataAsync(SuiStructTag coin_type);

        #endregion

        #region suix_getCoins

        /// <summary>
        /// Return all Coin<coin_type> objects owned by an address.
        /// https://docs.sui.io/sui-api-ref#suix_getcoins
        /// </summary>
        /// <param name="owner">The owner's Sui address</param>
        /// <param name="coin_type">
        /// Optional type name for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC),
        /// default to 0x2::sui::SUI if not specified.
        /// </param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinPage` object.</returns>
        public Task<RpcResult<CoinPage>> GetCoinsAsync(Account owner, SuiStructTag coin_type = null, SuiRpcFilter filter = null);

        /// <summary>
        /// Return all Coin<coin_type> objects owned by an address.
        /// https://docs.sui.io/sui-api-ref#suix_getcoins
        /// </summary>
        /// <param name="owner">The owner's Sui address</param>
        /// <param name="coin_type">
        /// Optional type name for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC),
        /// default to 0x2::sui::SUI if not specified.
        /// </param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinPage` object.</returns>
        public Task<RpcResult<CoinPage>> GetCoinsAsync(SuiPublicKeyBase owner, SuiStructTag coin_type = null, SuiRpcFilter filter = null);

        /// <summary>
        /// Return all Coin<coin_type> objects owned by an address.
        /// https://docs.sui.io/sui-api-ref#suix_getcoins
        /// </summary>
        /// <param name="owner">The owner's Sui address</param>
        /// <param name="coin_type">
        /// Optional type name for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC),
        /// default to 0x2::sui::SUI if not specified.
        /// </param>
        /// <param name="filter">The query object used for filtering by cursor and limit.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `CoinPage` object.</returns>
        public Task<RpcResult<CoinPage>> GetCoinsAsync(AccountAddress owner, SuiStructTag coin_type = null, SuiRpcFilter filter = null);

        #endregion

        #region suix_getTotalSupply

        /// <summary>
        /// Return total supply for a coin.
        /// https://docs.sui.io/sui-jsonrpc#suix_getTotalSupply
        /// </summary>
        /// <param name="coin_type">
        /// Type name for the coin (e.g.,
        /// 0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC)
        /// </param>
        /// <returns>An asynchronous task object containing the wrapped result of the `TotalSupply` object.</returns>
        public Task<RpcResult<TotalSupply>> GetTotalSupplyAsync(SuiStructTag coin_type);

        #endregion
    }
}