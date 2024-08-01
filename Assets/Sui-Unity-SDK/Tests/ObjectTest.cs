//
//  ObjectTest.cs
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
using System.Linq;
using Sui.Accounts;
using System.Collections.Generic;
using Sui.Types;
using Sui.Transactions;

namespace Sui.Tests
{
    public class ObjectTest
    {
        TestToolbox Toolbox;
        SuiStructTag CoinStruct;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            this.CoinStruct = new SuiStructTag("0x2::sui::SUI");
        }

        [UnityTest]
        public IEnumerator OwnedObjectFetchTest()
        {
            Task<RpcResult<PaginatedObjectDataResponse>> gas_object_task = this.Toolbox.Client.GetOwnedObjectsAsync(this.Toolbox.Account);
            yield return new WaitUntil(() => gas_object_task.IsCompleted);
            Assert.Greater(gas_object_task.Result.Result.Data.Length, 0);
        }

        [UnityTest]
        public IEnumerator GetObjectFetchTest()
        {
            Task<RpcResult<CoinPage>> gas_object_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => gas_object_task.IsCompleted);
            Assert.Greater(gas_object_task.Result.Result.Data.Length, 0);

            foreach (CoinDetails gas_coin in gas_object_task.Result.Result.Data)
            {
                ObjectData details = gas_coin.ToSuiObjectData();

                Task<RpcResult<ObjectDataResponse>> coin_object_task = this.Toolbox.Client.GetObjectAsync
                (
                    details.ObjectID,
                    new ObjectDataOptions(show_type: true)
                );
                yield return new WaitUntil(() => coin_object_task.IsCompleted);

                Assert.IsTrue(coin_object_task.Result.Result.Data.Type.Equals(new SuiStructTag("0x2::coin::Coin<0x2::sui::SUI>")));
            }
        }

        [UnityTest]
        public IEnumerator GetObjectsFetchTest()
        {
            Task<RpcResult<CoinPage>> gas_object_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => gas_object_task.IsCompleted);
            Assert.Greater(gas_object_task.Result.Result.Data.Length, 0);

            List<AccountAddress> gas_object_ids = gas_object_task.Result.Result.Data.Select((obj) => obj.CoinObjectID).ToList();
            Task<RpcResult<System.Collections.Generic.IEnumerable<ObjectDataResponse>>> object_infos_task = this.Toolbox.Client.MultiGetObjectsAsync
            (
                gas_object_ids,
                new ObjectDataOptions(show_type: true)
            );
            yield return new WaitUntil(() => object_infos_task.IsCompleted);

            Assert.IsTrue
            (
                gas_object_task.Result.Result.Data.Length ==
                object_infos_task.Result.Result.Count()
            );

            foreach (ObjectDataResponse obj in object_infos_task.Result.Result)
                Assert.IsTrue(obj.Data.Type.Equals(new SuiStructTag("0x2::coin::Coin<0x2::sui::SUI>")));
        }

        [UnityTest]
        public IEnumerator ObjectNotExistingHandleTest()
        {
            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObjectAsync(AccountAddress.FromHex("0x9999"), 0);
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.ObjectNotExists);
        }

        [UnityTest]
        public IEnumerator OldObjectHandleTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoinsAsync(this.Toolbox.Account, this.CoinStruct);
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObjectAsync
            (
                coin_data_task.Result.Result.Data[0].CoinObjectID,
                coin_data_task.Result.Result.Data[0].Version
            );
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.VersionFound);
        }

        [UnityTest]
        public IEnumerator VersionTooHighHandleTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoinsAsync(this.Toolbox.Account, this.CoinStruct);
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObjectAsync
            (
                coin_data_task.Result.Result.Data[0].CoinObjectID,
                coin_data_task.Result.Result.Data[0].Version + 1
            );
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.VersionTooHigh);
        }

        [UnityTest]
        public IEnumerator VersionNotFoundHandleTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoinsAsync(this.Toolbox.Account, this.CoinStruct);
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            // NOTE: This works because we know that this is a fresh coin that hasn't been modified:
            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObjectAsync
            (
                coin_data_task.Result.Result.Data[0].CoinObjectID,
                coin_data_task.Result.Result.Data[0].Version - 1
            );
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.VersionNotFound);
        }

        [UnityTest]
        public IEnumerator OldVersionFindTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoinsAsync(this.Toolbox.Account, this.CoinStruct);
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            tx_block.AddTransferObjectsTx
            (
                new TransactionArgument[] { tx_block.gas },
                AccountAddress.FromHex("0x2")
            );

            Task<RpcResult<TransactionBlockResponse>> tx_block_sign_task = this.Toolbox.Client.SignAndExecuteTransactionBlockAsync(tx_block, this.Toolbox.Account);
            yield return new WaitUntil(() => tx_block_sign_task.IsCompleted);

            // NOTE: This works because we know that this is a fresh coin that hasn't been modified:
            Task<RpcResult<ObjectRead>> result_task = this.Toolbox.Client.TryGetPastObjectAsync
            (
                coin_data_task.Result.Result.Data[0].CoinObjectID,
                coin_data_task.Result.Result.Data[0].Version
            );
            yield return new WaitUntil(() => result_task.IsCompleted);

            Assert.IsTrue(result_task.Result.Result.Type == ObjectReadType.VersionFound);
        }
    }
}
