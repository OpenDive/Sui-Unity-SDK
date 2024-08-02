//
//  EntryPointStringTest.cs
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

using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Collections.Generic;
using OpenDive.BCS;
using System.Linq;
using Sui.Types;
using Sui.Transactions;

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

            yield return this.Toolbox.PublishPackage("entry-point-types", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });
        }

        private IEnumerator CallWithString
        (
            string str,
            int len,
            string func_name
        )
        {
            TransactionBlock tx_block = new TransactionBlock();
            List<TransactionArgument> obj = tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::entry_point_types::{func_name}"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddPure(new BString(str)),
                        tx_block.AddPure(new U64((ulong)len))
                }
            );
            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            Task<RpcResult<TransactionBlockResponse>> call_tx_block_task = this.Toolbox.Client.SignAndExecuteTransactionBlockAsync(tx_block, this.Toolbox.Account, options);
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
            TransactionBlock tx_block = new TransactionBlock();
            List<TransactionArgument> obj = tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::entry_point_types::{func_name}"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddPure(new Sequence(str.Select((val) => new BString(val)).ToArray())),
                        tx_block.AddPure(new U64((ulong)len))
                }
            );
            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            Task<RpcResult<TransactionBlockResponse>> call_tx_block_task = this.Toolbox.Client.SignAndExecuteTransactionBlockAsync(tx_block, this.Toolbox.Account, options);
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
