//
//  IMoveUtils.cs
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

using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sui.Rpc.Api
{
    public interface IMoveUtils
    {
        #region sui_getMoveFunctionArgTypes

        /// <summary>
        /// Return the argument types of a Move function, based on normalized Type.
        /// https://docs.sui.io/sui-jsonrpc#sui_getMoveFunctionArgTypes
        /// </summary>
        /// <param name="struct_tag">The `SuiStructTag` identifier of the function.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `MoveFunctionArgTypes` object.</returns>
        public Task<RpcResult<IEnumerable<MoveFunctionArgType>>> GetMoveFunctionArgTypesAsync(SuiStructTag struct_tag);

        #endregion

        #region sui_getNormalizedMoveFunction

        /// <summary>
        /// Return a structured representation of Move function
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveFunction
        /// </summary>
        /// <param name="struct_tag">The `SuiStructTag` identifier of the function.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `NormalizedMoveFunctionResponse` object.</returns>
        public Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunctionAsync(SuiStructTag struct_tag);

        /// <summary>
        /// Return a structured representation of Move function
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveFunction
        /// </summary>
        /// <param name="struct_tag">The `SuiStructTag` identifier of the function.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `NormalizedMoveFunctionResponse` object.</returns>
        public Task<RpcResult<NormalizedMoveFunctionResponse>> GetNormalizedMoveFunctionAsync(SuiMoveNormalizedStructType struct_tag);

        #endregion

        #region sui_getNormalizedMoveModule

        /// <summary>
        /// Return a structured representation of Move module
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveModule
        /// </summary>
        /// <param name="package">The `AccountAddress` identifier of the package containing the module and function.</param>
        /// <param name="module_name">The `string` identifier of the module containing the function.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `SuiMoveNormalizedModule` object.</returns>
        public Task<RpcResult<SuiMoveNormalizedModule>> GetNormalizedMoveModuleAsync(AccountAddress package, string module_name);

        #endregion

        #region sui_getNormalizedMoveModulesByPackage

        /// <summary>
        /// Return structured representations of all modules in the given package
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveModulesByPackage
        /// </summary>
        /// <param name="package">The `AccountAddress` identifier of the package containing the modules.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `Dictionary<string, SuiMoveNormalizedModule>` object.</returns>
        public Task<RpcResult<Dictionary<string, SuiMoveNormalizedModule>>> GetNormalizedMoveModulesByPackageAsync(AccountAddress package);

        #endregion

        #region sui_getNormalizedMoveStruct

        /// <summary>
        /// Return a structured representation of Move struct
        /// https://docs.sui.io/sui-jsonrpc#sui_getNormalizedMoveStruct
        /// </summary>
        /// <param name="struct_tag">The `SuiStructTag` identifier of the struct.</param>
        /// <returns>An asynchronous task object containing the wrapped result of the `SuiMoveNormalizedStruct` object.</returns>
        Task<RpcResult<SuiMoveNormalizedStruct>> GetNormalizedMoveStructAsync(SuiStructTag struct_tag);

        #endregion
    }
}