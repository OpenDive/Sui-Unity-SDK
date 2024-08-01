using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types;
using Sui.Transactions;

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

            yield return this.Toolbox.PublishPackage("id-entry-args", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });
        }

        private async Task ExecuteArg(SuiClient client, Account account, string target)
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr(target),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddPure(AccountAddress.FromHex(this.DefaultAddress))
                }
            );
            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            RpcResult<TransactionBlockResponse> call_tx_block_task = await client.SignAndExecuteTransactionBlockAsync(tx_block, account, options);

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