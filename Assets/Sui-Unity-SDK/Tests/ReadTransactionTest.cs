using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Collections.Generic;
using Sui.Rpc.Client;
using System.Linq;
using Sui.Transactions.Types.Arguments;
using OpenDive.BCS;
using System.Numerics;
using Newtonsoft.Json;
using System;

namespace Sui.Tests
{
    public class ReadTransactionTest
    {
        TestToolbox Toolbox;
        List<TransactionBlockResponse> Transactions;

        int NumTransactions = 2;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        private async Task<RpcError> InitializePaySui()
        {
            RpcResult<IEnumerable<TransactionBlockResponse>> txns = await this.Toolbox.ExecutePaySuiNTimes(this.NumTransactions);

            if (txns.Error != null)
                return (RpcError)txns.Error;

            this.Transactions = txns.Result.ToList();

            return null;
        }

        private async Task<RpcResult<TransactionBlockResponse>> SetupTransaction(TestToolbox toolbox)
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                tx_block.gas,
                new SuiTransactionArgument[]
                {
                    tx_block.AddPure(new U64(1))
                }
            );
            tx_block.AddTransferObjectsTx(coin.ToArray(), toolbox.Address());
            return await toolbox.Client.SignAndExecuteTransactionBlock
            (
                tx_block,
                toolbox.Account,
                request_type: RequestType.WaitForEffectsCert
            );
        }

        private IEnumerator PaySuiWithCheck()
        {
            Task<RpcError> pay_sui_error_task = this.InitializePaySui();
            yield return new WaitUntil(() => pay_sui_error_task.IsCompleted);

            if (pay_sui_error_task.Result != null)
                Assert.Fail($"Transaction Failed with message: {pay_sui_error_task.Result.Message}");
        }

        [UnityTest]
        public IEnumerator TotalTransactionFetchTest()
        {
            Task<RpcResult<BigInteger>> total_transaction_blocks_task = this.Toolbox.Client.GetTotalTransactionBlocksAsync();
            yield return new WaitUntil(() => total_transaction_blocks_task.IsCompleted);

            Assert.Greater(total_transaction_blocks_task.Result.Result, BigInteger.Parse("0"));
        }

        [UnityTest]
        public IEnumerator TransactionBlockWaitingTest()
        {
            Task<RpcResult<TransactionBlockResponse>> response_task = this.SetupTransaction(this.Toolbox);
            yield return new WaitUntil(() => response_task.IsCompleted);

            Task<RpcResult<TransactionBlockResponse>> wait_task = this.Toolbox.Client.WaitForTransaction
            (
                response_task.Result.Result.Digest
            );
            yield return new WaitUntil(() => wait_task.IsCompleted);

            Assert.NotNull(wait_task.Result.Result.TimestampMs);
        }

        [UnityTest]
        public IEnumerator TransactionFetchTest()
        {
            yield return this.PaySuiWithCheck();

            string digest = this.Transactions[0].Digest;

            Task<RpcResult<TransactionBlockResponse>> tx_block_task = this.Toolbox.Client.GetTransactionBlock(digest);
            yield return new WaitUntil(() => tx_block_task.IsCompleted);

            Assert.IsTrue(tx_block_task.Result.Result.Digest == digest);
        }

        [UnityTest]
        public IEnumerator MultiGetPayTransactionTest()
        {
            yield return this.PaySuiWithCheck();

            string[] digests = this.Transactions.Select((txn) => txn.Digest).ToArray();

            Task<RpcResult<IEnumerable<TransactionBlockResponse>>> txns_task = this.Toolbox.Client.GetTransactionBlocks
            (
                digests,
                new TransactionBlockResponseOptions(showBalanceChanges: true)
            );
            yield return new WaitUntil(() => txns_task.IsCompleted);

            foreach(Tuple<int, TransactionBlockResponse> tx in txns_task.Result.Result.Select((tx, i) => new Tuple<int, TransactionBlockResponse>(i, tx)))
            {
                Assert.IsTrue(tx.Item2.Digest == digests[tx.Item1]);
                Assert.IsTrue(tx.Item2.BalanceChanges.Count() == 2);
            }
        }

        [UnityTest]
        public IEnumerator QueryTransactionOptionsTest()
        {
            yield return this.PaySuiWithCheck();

            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions
            (
                showInput: true,
                showEffects: true,
                showEvents: true,
                showObjectChanges: true,
                showBalanceChanges: true
            );

            Task<RpcResult<TransactionBlockResponsePage>> response_1_task = this.Toolbox.Client.QueryTransactionBlocks(limit: 1, options: options);
            yield return new WaitUntil(() => response_1_task.IsCompleted);

            string digest = response_1_task.Result.Result.Data[0].Digest;

            Task<RpcResult<TransactionBlockResponse>> response_2_task = this.Toolbox.Client.GetTransactionBlock(digest, options);
            yield return new WaitUntil(() => response_2_task.IsCompleted);

            Assert.IsTrue(digest == response_2_task.Result.Result.Digest);
        }

        [UnityTest]
        public IEnumerator AllTransactionFetchTest()
        {
            yield return this.PaySuiWithCheck();

            Task<RpcResult<TransactionBlockResponsePage>> all_transactions_task = this.Toolbox.Client.QueryTransactionBlocks(limit: 10);
            yield return new WaitUntil(() => all_transactions_task.IsCompleted);

            Assert.Greater(all_transactions_task.Result.Result.Data.Count, 0);
        }

        [UnityTest]
        public IEnumerator GenesisTransactionFetchTest()
        {
            yield return this.PaySuiWithCheck();

            Task<RpcResult<TransactionBlockResponsePage>> all_transactions_task = this.Toolbox.Client.QueryTransactionBlocks(limit: 1, order: SortOrder.Ascending);
            yield return new WaitUntil(() => all_transactions_task.IsCompleted);

            Task<RpcResult<TransactionBlockResponse>> transaction_block_task = this.Toolbox.Client.GetTransactionBlock
            (
                all_transactions_task.Result.Result.Data[0].Digest,
                new TransactionBlockResponseOptions(showInput: true)
            );
            yield return new WaitUntil(() => transaction_block_task.IsCompleted);

            Transactions.Kinds.TransactionBlockKind tx_kind = transaction_block_task.Result.Result.Transaction.Data.Transaction;
            Assert.IsTrue(tx_kind.Type == Sui.Transactions.Kinds.SuiTransactionKindType.Genesis);
        }
    }
}