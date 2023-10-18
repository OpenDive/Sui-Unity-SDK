using System.Collections.Generic;
using System.Threading.Tasks;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;

namespace Sui.Rpc.Api
{
    public interface ICoinQueryApi
    {
        /// <summary>
        /// https://docs.sui.io/sui-jsonrpc#suix_getBalance
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<Balance>> GetBalanceAsync(AccountAddress owner, SuiStructTag coinType = null);

        /// <summary>
        /// Return the total coin balance for all coin type, owned by the address owner.
        /// https://docs.sui.io/sui-jsonrpc#suix_getAllBalances
        /// </summary>
        /// <returns></returns>
        Task<RpcResult<IEnumerable<Balance>>> GetAllBalancesAsync(AccountAddress owner);

        /// <summary>
        /// https://docs.sui.io/sui-jsonrpc#suix_getCoinMetadata
        /// </summary>
        /// <param name="coin_type"></param>
        /// <returns></returns>
        Task<RpcResult<CoinMetadata>> GetCoinMetadata(SuiStructTag coin_type);
    }
}