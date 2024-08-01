//
//  DevInspectTest.cs
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
using Sui.Accounts;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Rpc.Client;
using Sui.Types;
using Sui.Transactions;

namespace Sui.Tests
{
    public class DevInspectTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("serializer", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });
        }

        private IEnumerator ValidateDevInspectTransaction
        (
            SuiClient client,
            Account signer,
            TransactionBlock transaction_block,
            ExecutionStatus status
        )
        {
            Task<RpcResult<DevInspectResponse>> result_task = client.DevInspectTransactionBlockAsync(signer, transaction_block);
            yield return new WaitUntil(() => result_task.IsCompleted);

            if (result_task.Result.Result == null || status != result_task.Result.Result.Effects.Status.Status)
                Assert.Fail("Status does not match");
        }

        [UnityTest]
        public IEnumerator DevInspectSplitAndTransferTest()
        {
            TransactionBlock tx_block = new TransactionBlock();
            List<TransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                tx_block.gas,
                new TransactionArgument[] { tx_block.AddPure(new U64(10)) }
            );
            tx_block.AddTransferObjectsTx(coin.ToArray(), this.Toolbox.Account.SuiAddress());
            yield return ValidateDevInspectTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block,
                ExecutionStatus.Success
            );
        }

        [UnityTest]
        public IEnumerator DevInspectMoveCallTest()
        {
            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);
            CoinPage coins = coins_task.Result.Result;

            TransactionBlock tx_block = new TransactionBlock();
            CoinDetails coin_0 = coins.Data[0];
            List<TransactionArgument> obj = tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::return_struct"),
                new SerializableTypeTag[] { new SerializableTypeTag(SuiStructTag.FromStr("0x2::coin::Coin<0x2::sui::SUI>")) },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(coin_0.CoinObjectID.KeyHex)
                }
            );
            tx_block.AddTransferObjectsTx(obj.ToArray(), this.Toolbox.Account.SuiAddress());

            yield return ValidateDevInspectTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block,
                ExecutionStatus.Success
            );
        }

        [UnityTest]
        public IEnumerator MoveCallFailedDevInspectTest()
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::test_abort"),
                new SerializableTypeTag[] { },
                new TransactionArgument[] { }
            );

            yield return ValidateDevInspectTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block,
                ExecutionStatus.Failure
            );
        }
    }
}
