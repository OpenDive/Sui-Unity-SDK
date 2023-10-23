using System.Collections.Generic;
using System.Threading.Tasks;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;

namespace Sui.Rpc.Api
{
    public interface IExtendedApi
    {
        /// <summary>
        /// Return the resolved address given resolver and name
        /// </summary>
        /// <param name="name"> The name to resolve, for example: "example.sui"</param>
        /// <returns></returns>
        Task<RpcResult<AccountAddress>> ResolveNameServiceAddress(string name);
    }
}