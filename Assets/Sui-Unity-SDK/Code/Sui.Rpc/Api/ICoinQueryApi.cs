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
        /// https://docs.sui.io/sui-jsonrpc#suix_getCoinMetadata
        /// </summary>
        /// <param name="coin_type"></param>
        /// <returns></returns>
        Task<RpcResult<CoinMetadata>> GetCoinMetadata(SuiStructTag coin_type);
    }
}