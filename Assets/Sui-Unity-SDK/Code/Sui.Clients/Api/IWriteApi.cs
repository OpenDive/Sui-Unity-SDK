//
//  IWriteApi.cs
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
using System.Numerics;
using System.Threading.Tasks;
using Sui.Accounts;
using Sui.Rpc.Models;
using Sui.Rpc;

namespace Sui.Clients.Api
{
    public interface IWriteApi
    {
        #region sui_devInspectTransactionBlock

        /// <summary>
        /// Runs the transaction in dev-inspect mode. Which allows for nearly any transaction (or Move call) with any arguments.
        /// Detailed results are provided, including both the transaction effects and any return values.
        /// </summary>
        /// <param name="sender">The sender's Sui Address to sign the transaction.</param>
        /// <param name="transaction_block">BCS encoded TransactionKind(as opposed to TransactionData, which include gasBudget and gasPrice).</param>
        /// <param name="gas_price">Gas is not charged, but gas usage is still calculated. Default to use reference gas price.</param>
        /// <param name="epoch">The epoch to perform the call. Will be set from the system state object if not provided.</param>
        /// <param name="additional_args">Additional arguments including gas_budget, gas_objects, gas_sponsor and skip_checks.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `DevInspectResponse` object.</returns>
        public Task<RpcResult<DevInspectResponse>> DevInspectTransactionBlockAsync
        (
            Account sender,
            Transactions.TransactionBlock transaction_block,
            BigInteger? gas_price = null,
            BigInteger? epoch = null,
            DevInspectArgs additional_args = null
        );

        #endregion

        #region sui_dryRunTransactionBlock

        /// <summary>
        /// Return transaction execution effects including the gas cost summary, while the effects are not committed to the chain.
        /// </summary>
        /// <param name="tx_bytes">BCS encoded TransactionData.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `TransactionBlockResponse` object.</returns>
        public Task<RpcResult<TransactionBlockResponse>> DryRunTransactionBlockAsync(string tx_bytes);

        #endregion

        #region sui_executeTransactionBlock

        /// <summary>
        /// Execute the transaction and wait for results if desired. Request types:
        /// <para>1. WaitForEffectsCert: waits for TransactionEffectsCert and then return to client.
        /// This mode is a proxy for transaction finality.</para>
        /// <para>2. WaitForLocalExecution: waits for TransactionEffectsCert and make sure the node executed the
        /// transaction locally before returning the client. The local execution makes sure this node is aware
        /// of this transaction when client fires subsequent queries. However if the node fails to execute the
        /// transaction locally in a timely manner, a bool type in the response is set to false to indicated the
        /// case. request_type is default to be WaitForEffectsCert unless options.show_events or options.show_effects is true.</para>
        /// </summary>
        /// <param name="tx_bytes">BCS serialized transaction data bytes without its type tag, as base-64 encoded string.</param>
        /// <param name="signature">
        /// A list of signatures (flag || signature || pubkey bytes, as base-64 encoded string).
        /// Signature is committed to the intent message of the transaction data, as base-64 encoded string.
        /// </param>
        /// <param name="options">Options for specifying the content to be returned.</param>
        /// <param name="request_type">The request type, derived from SuiTransactionBlockResponseOptions if None.</param>
        /// <returns>An asynchronous task object containing the wrapped result of a `TransactionBlockResponse` object.</returns>
        public Task<RpcResult<TransactionBlockResponse>> ExecuteTransactionBlockAsync
        (
            byte[] tx_bytes,
            List<string> signature,
            TransactionBlockResponseOptions options = null,
            RequestType? request_type = null
        );

        #endregion
    }
}