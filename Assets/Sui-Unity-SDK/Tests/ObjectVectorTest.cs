//
//  ObjectVectorTest.cs
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
using OpenDive.BCS;
using System.Linq;
using System.Collections.Generic;
using Sui.Accounts;
using Sui.Types;
using Sui.Utilities;
using Sui.Transactions;

namespace Sui.Tests
{
    public class ObjectVectorTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("entry-point-vector", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });

            yield return this.Toolbox.Setup();
        }

        private void CheckBlockResponse(RpcResult<TransactionBlockResponse> tx_sign)
        {
            if (tx_sign.Error != null || tx_sign.Result == null || (tx_sign.Result != null && tx_sign.Result.Effects.Status.Status == ExecutionStatus.Failure))
                Assert.Fail($"Transaction Failed with message - {tx_sign.Error.Message ?? tx_sign.Result.Effects.Status.Error}");
        }

        private async Task<AccountAddress> MintObject(int val, TestToolbox toolbox)
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::entry_point_vector::mint"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                    tx_block.AddPure(new U64((ulong)val))
                }
            );

            RpcResult<TransactionBlockResponse> tx_sign = await toolbox.Client.SignAndExecuteTransactionBlockAsync
            (
                tx_block,
                toolbox.Account,
                new TransactionBlockResponseOptions(showEffects: true)
            );

            this.CheckBlockResponse(tx_sign);

            return tx_sign.Result.Effects.Created[0].Reference.ObjectID;
        }

        private IEnumerator DestroyObject(string[] objects, TestToolbox toolbox, bool with_type = false)
        {
            TransactionBlock tx_block = new TransactionBlock();
            List<TransactionArgument> vec = tx_block.AddMakeMoveVecTx
            (
                objects.Select((obj) => tx_block.AddObjectInput(obj)).ToArray(),
                with_type ? SuiStructTag.FromStr($"{this.PackageID}::entry_point_vector::Obj") : null
            );
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::entry_point_vector::two_obj_vec_destroy"),
                new SerializableTypeTag[] { },
                vec.ToArray()
            );

            Task<RpcResult<TransactionBlockResponse>> tx_sign_task = toolbox.Client.SignAndExecuteTransactionBlockAsync
            (
                tx_block,
                toolbox.Account,
                new TransactionBlockResponseOptions(showEffects: true)
            );
            yield return new WaitUntil(() => tx_sign_task.IsCompleted);

            this.CheckBlockResponse(tx_sign_task.Result);
        }
        
        [UnityTest]
        public IEnumerator VectorObjectsInitializationTest()
        {
            Task<AccountAddress> res_1 = this.MintObject(7, this.Toolbox);
            yield return new WaitUntil(() => res_1.IsCompleted);

            Task<AccountAddress> res_2 = this.MintObject(42, this.Toolbox);
            yield return new WaitUntil(() => res_2.IsCompleted);

            yield return this.DestroyObject(new string[] { res_1.Result.KeyHex, res_2.Result.KeyHex }, this.Toolbox);
        }

        // TODO: Figure out and implement a solution for this flaky test.
        [UnityTest, Category("ExcludeFromCI")]
        [Retry(10)]
        public IEnumerator TypeHintTest()
        {
            Task<AccountAddress> res_1 = this.MintObject(7, this.Toolbox);
            yield return new WaitUntil(() => res_1.IsCompleted);

            Task<AccountAddress> res_2 = this.MintObject(42, this.Toolbox);
            yield return new WaitUntil(() => res_2.IsCompleted);

            yield return this.DestroyObject(new string[] { res_1.Result.KeyHex, res_2.Result.KeyHex }, this.Toolbox, true);
        }

        [UnityTest]
        public IEnumerator MixedArgumentTest()
        {
            Task<RpcResult<CoinPage>> coin_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coin_task.IsCompleted);

            CoinDetails coin = coin_task.Result.Result.Data[3];
            string[] coin_ids = coin_task.Result.Result.Data.Select((coin) => coin.CoinObjectID.KeyHex).ToArray();
            TransactionBlock tx_block = new TransactionBlock();
            List<TransactionArgument> vec = tx_block.AddMakeMoveVecTx
            (
                new TransactionArgument[]
                {
                    tx_block.AddObjectInput(coin_ids[1]),
                    tx_block.AddObjectInput(coin_ids[2])
                }
            );
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr("0x2::pay::join_vec"),
                new SerializableTypeTag[] { new SerializableTypeTag(Utils.SuiCoinStruct) },
                new TransactionArgument[]
                {
                    tx_block.AddObjectInput(coin_ids[0]),
                    vec[0]
                }
            );
            tx_block.SetGasPayment(new SuiObjectRef[] { coin.ToSuiObjectRef() });

            Task<RpcResult<TransactionBlockResponse>> tx_sign_task = this.Toolbox.Client.SignAndExecuteTransactionBlockAsync
            (
                tx_block,
                this.Toolbox.Account,
                new TransactionBlockResponseOptions(showEffects: true)
            );
            yield return new WaitUntil(() => tx_sign_task.IsCompleted);

            this.CheckBlockResponse(tx_sign_task.Result);
        }
    }
}