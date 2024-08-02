//
//  TXBuilderTest.cs
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

using NUnit.Framework;
using OpenDive.BCS;
using Sui.Accounts;
using UnityEngine;
using Sui.Transactions;
using Sui.Rpc;
using Sui.Rpc.Client;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TestTools;
using Sui.Rpc.Models;
using Sui.Utilities;
using Sui.Types;

namespace Sui.Tests
{
    public class TXBuilderTest
    {
        TestToolbox Toolbox;
        string PackageID;
        string SharedObjectID;
        string SuiClockObjectID = Utils.NormalizeSuiAddress("0x6");

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("serializer", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;

                List<SuiOwnedObjectRef> created_objects = package_result.Result.PublishedTX.Effects.Created.ToList();
                List<SuiOwnedObjectRef> shared_object = created_objects.Where(obj => obj.Owner.Type == SuiOwnerType.Shared).ToList();

                this.SharedObjectID = shared_object[0].Reference.ObjectID.KeyHex;
            });
        }

        private IEnumerator ValidateTransaction
        (
            SuiClient client,
            Account account,
            TransactionBlock tx_block
        )
        {
            tx_block.SetSenderIfNotSet(account.SuiAddress());

            Task<string> local_digest_task = tx_block.GetDigest(new BuildOptions(client));
            yield return new WaitUntil(() => local_digest_task.IsCompleted);

            if (tx_block.Error != null)
                Assert.Fail(tx_block.Error.Message);

            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            Task<RpcResult<TransactionBlockResponse>> result_task = client.SignAndExecuteTransactionBlockAsync
            (
                tx_block,
                account,
                options
            );
            yield return new WaitUntil(() => result_task.IsCompleted);

            Task<RpcResult<TransactionBlockResponse>> wait_task = client.WaitForTransaction(local_digest_task.Result, options);
            yield return new WaitUntil(() => wait_task.IsCompleted);

            Assert.IsTrue(local_digest_task.Result == wait_task.Result.Result.Digest);

            if (wait_task.Result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                Assert.Fail("Transaction Failed");
        }

        [UnityTest]
        public IEnumerator SplitCoinAndTransferTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);

            CoinDetails coin_0 = coins_task.Result.Result.Data[0];
            TransactionBlock tx_block = new TransactionBlock();

            List<TransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                tx_block.AddObjectInput(coin_0.CoinObjectID.KeyHex),
                new TransactionArgument[] { tx_block.AddPure(new U64((ulong)(this.Toolbox.DefaultGasBudget * 2))) }
            );
            tx_block.AddTransferObjectsTx(coin.ToArray(), this.Toolbox.Address());

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator MergeCoinTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);

            CoinDetails coin_0 = coins_task.Result.Result.Data[0];
            CoinDetails coin_1 = coins_task.Result.Result.Data[1];

            TransactionBlock tx_block = new TransactionBlock();

            tx_block.AddMergeCoinsTx
            (
                tx_block.AddObjectInput(coin_0.CoinObjectID.KeyHex),
                new TransactionArgument[] { tx_block.AddObjectInput(coin_1.CoinObjectID.KeyHex) }
            );

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator MoveCallTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);

            CoinDetails coin_0 = coins_task.Result.Result.Data[0];

            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr("0x2::pay::split"),
                new SerializableTypeTag[] { new SerializableTypeTag("0x2::sui::SUI") },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(coin_0.CoinObjectID.KeyHex),
                        tx_block.AddPure(new U64((ulong)(this.Toolbox.DefaultGasBudget * 2)))
                }
            );

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator SplitCoinAndTransferObjectWithGasCoinTest()
        {
            yield return this.Toolbox.Setup();

            TransactionBlock tx_block = new TransactionBlock();

            List<TransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                tx_block.gas,
                new TransactionArgument[] { tx_block.AddPure(new U64(1)) }
            );
            tx_block.AddTransferObjectsTx(coin.ToArray(), this.Toolbox.DefaultRecipient);

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator TransferObjectGasCoinTest()
        {
            yield return this.Toolbox.Setup();

            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddTransferObjectsTx
            (
                new TransactionArgument[] { tx_block.gas },
                this.Toolbox.DefaultRecipient
            );

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator TransferObjectsTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);

            CoinDetails coin_0 = coins_task.Result.Result.Data[0];

            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddTransferObjectsTx
            (
                new TransactionArgument[] { tx_block.AddObjectInput(coin_0.CoinObjectID.KeyHex) },
                this.Toolbox.DefaultRecipient
            );

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator MoveSharedMutableImmutableReferencesTest()
        {
            yield return this.Toolbox.Setup();

            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::value"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.SharedObjectID)
                }
            );
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::set_value"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.SharedObjectID)
                }
            );

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }

        [UnityTest]
        public IEnumerator ImmutableClockFunctionTest()
        {
            yield return this.Toolbox.Setup();

            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::use_clock"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.SuiClockObjectID)
                }
            );

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }
    }
}
