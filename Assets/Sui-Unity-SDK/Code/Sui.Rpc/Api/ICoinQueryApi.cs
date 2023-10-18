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
    }
}