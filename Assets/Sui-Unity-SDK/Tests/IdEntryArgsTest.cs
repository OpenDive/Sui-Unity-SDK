﻿using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;
using Sui.Accounts;

namespace Sui.Tests
{
    public class IdEntryArgsTest
    {
        TestToolbox Toolbox;
        string PackageID;
        string DefaultAddress = "0x000000000000000000000000c2b5625c221264078310a084df0a3137956d20ee";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("id-entry-args");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;
        }

        private async Task ExecuteArg(SuiClient client, Account account, string target)
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr(target), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                new SuiTransactionArgument[]
                {
                        new SuiTransactionArgument(tx_block.AddPure(AccountAddress.FromHex(this.DefaultAddress)))
                }
            );
            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            RpcResult<TransactionBlockResponse> call_tx_block_task = await client.SignAndExecuteTransactionBlock(tx_block, account, options);

            await client.WaitForTransaction(call_tx_block_task.Result.Digest);

            if (call_tx_block_task.Result.Effects.Status.Status == ExecutionStatus.Failure)
                Assert.Fail("Transaction Failed");
        }

        [UnityTest]
        public IEnumerator MutableIdEntryArgsTest()
        {
            Task tx_task = ExecuteArg(this.Toolbox.Client, this.Toolbox.Account, $"{this.PackageID}::test::test_id");
            yield return new WaitUntil(() => tx_task.IsCompleted);
        }

        [UnityTest]
        public IEnumerator NonMutableIdEntryArgsTest()
        {
            Task tx_task = ExecuteArg(this.Toolbox.Client, this.Toolbox.Account, $"{this.PackageID}::test::test_id_non_mut");
            yield return new WaitUntil(() => tx_task.IsCompleted);
        }
    }
}