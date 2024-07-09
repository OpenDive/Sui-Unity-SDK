using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Transactions.Types.Arguments;
using System.Collections.Generic;
using OpenDive.BCS;
using System.Linq;
using Newtonsoft.Json;

namespace Sui.Tests
{
    public class EntryPointStringTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("entry-point-types");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;
        }

        private IEnumerator CallWithString
        (
            string str,
            int len,
            string func_name
        )
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> obj = tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"{this.PackageID}::entry_point_types::{func_name}"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                new SuiTransactionArgument[]
                {
                        new SuiTransactionArgument(tx_block.AddPure(new BString(str))),
                        new SuiTransactionArgument(tx_block.AddPure(new U64((ulong)len)))
                }
            );
            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            Task<RpcResult<TransactionBlockResponse>> call_tx_block_task = this.Toolbox.Client.SignAndExecuteTransactionBlock(tx_block, this.Toolbox.Account, options);
            yield return new WaitUntil(() => call_tx_block_task.IsCompleted);

            Task tx_task = this.Toolbox.Client.WaitForTransaction(call_tx_block_task.Result.Result.Digest);
            yield return new WaitUntil(() => tx_task.IsCompleted);

            if (call_tx_block_task.Result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                Assert.Fail("Transaction Failed");
        }

        private IEnumerator CallWithString
        (
            string[] str,
            int len,
            string func_name
        )
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> obj = tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"{this.PackageID}::entry_point_types::{func_name}"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                new SuiTransactionArgument[]
                {
                        new SuiTransactionArgument(tx_block.AddPure(new Sequence(str.Select((val) => new BString(val)).ToArray()))),
                        new SuiTransactionArgument(tx_block.AddPure(new U64((ulong)len)))
                }
            );
            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            Task<RpcResult<TransactionBlockResponse>> call_tx_block_task = this.Toolbox.Client.SignAndExecuteTransactionBlock(tx_block, this.Toolbox.Account, options);
            yield return new WaitUntil(() => call_tx_block_task.IsCompleted);

            Task tx_task = this.Toolbox.Client.WaitForTransaction(call_tx_block_task.Result.Result.Digest);
            yield return new WaitUntil(() => tx_task.IsCompleted);

            if (call_tx_block_task.Result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                Assert.Fail("Transaction Failed");
        }

        [UnityTest]
        public IEnumerator AsciiStringTest()
        {
            string s = "SomeString";
            yield return this.CallWithString(s, s.Length, "ascii_arg");
        }

        [UnityTest]
        public IEnumerator Utf8StringTest()
        {
            string s = "çå∞≠¢õß∂ƒ∫";
            int byteLength = 24;
            yield return this.CallWithString(s, byteLength, "utf8_arg");
        }

        [UnityTest]
        public IEnumerator StringVectorTest()
        {
            string s1 = "çå∞≠¢";
            string s2 = "õß∂ƒ∫";
            int byte_length = 24;
            yield return this.CallWithString(new string[] { s1, s2 }, byte_length, "utf8_vec_arg");
        }
    }
}
