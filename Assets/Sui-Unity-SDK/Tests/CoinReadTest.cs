//
//  CoinReadTest.cs
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
using System.Linq;
using System.Numerics;
using Sui.Types;

namespace Sui.Tests
{
    public class CoinReadTest
    {
        TestToolbox Toolbox;
        TestToolbox PublishToolbox;
        string PackageID;
        SuiStructTag TestType;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            this.PublishToolbox = new TestToolbox();

            yield return this.Toolbox.Setup();
            yield return this.PublishToolbox.Setup();            

            yield return this.PublishToolbox.PublishPackage("coin-metadata", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
                this.TestType = new SuiStructTag($"{this.PackageID}::test::TEST");
            });
        }

        [UnityTest]
        public IEnumerator NoTypeFunctionTest()
        {
            Task<RpcResult<CoinPage>> sui_coin_task = this.Toolbox.Client.GetCoinsAsync(this.Toolbox.Account);
            yield return new WaitUntil(() => sui_coin_task.IsCompleted);
            CoinPage sui_coin = sui_coin_task.Result.Result;

            Assert.IsTrue(sui_coin.Data.Length == 5);

            Task<RpcResult<CoinPage>> test_coin_task = this.PublishToolbox.Client.GetCoinsAsync
            (
                this.PublishToolbox.Account,
                this.TestType
            );
            yield return new WaitUntil(() => test_coin_task.IsCompleted);
            CoinPage test_coin = test_coin_task.Result.Result;

            Assert.IsTrue(test_coin.Data.Length == 2);

            Task<RpcResult<CoinPage>> all_coins_task = this.Toolbox.Client.GetAllCoinsAsync(this.Toolbox.Account);
            yield return new WaitUntil(() => all_coins_task.IsCompleted);
            CoinPage all_coins = all_coins_task.Result.Result;

            Assert.IsTrue(all_coins.Data.Length == 5);
            Assert.IsFalse(all_coins.HasNextPage);

            Task<RpcResult<CoinPage>> publisher_all_coins_task = this.Toolbox.Client.GetAllCoinsAsync(this.PublishToolbox.Account);
            yield return new WaitUntil(() => publisher_all_coins_task.IsCompleted);
            CoinPage publisher_all_coins = publisher_all_coins_task.Result.Result;

            Assert.IsTrue(publisher_all_coins.Data.Length == 3);
            Assert.IsFalse(all_coins.HasNextPage);

            Task<RpcResult<CoinPage>> some_sui_coin_task = this.Toolbox.Client.GetCoinsAsync(this.Toolbox.Account, filter: new SuiRpcFilter(limit: 3));
            yield return new WaitUntil(() => some_sui_coin_task.IsCompleted);
            CoinPage some_sui_coin = some_sui_coin_task.Result.Result;

            Assert.IsTrue(some_sui_coin.Data.Length == 3);
            Assert.IsTrue(some_sui_coin.HasNextPage);
        }

        [UnityTest]
        public IEnumerator BalanceWithTypeTest()
        {
            Task<RpcResult<Balance>> sui_balance_task = this.Toolbox.Client.GetBalanceAsync(this.Toolbox.Account.PublicKey);
            yield return new WaitUntil(() => sui_balance_task.IsCompleted);
            Balance sui_balance = sui_balance_task.Result.Result;

            Assert.IsTrue(sui_balance.CoinType.Equals(new SuiStructTag("0x2::sui::SUI")));
            Assert.IsTrue(sui_balance.CoinObjectCount == 5);
            Assert.IsTrue(sui_balance.TotalBalance != BigInteger.Zero);

            Task<RpcResult<Balance>> test_balance_task = this.PublishToolbox.Client.GetBalanceAsync
            (
                this.PublishToolbox.Account,
                this.TestType
            );
            yield return new WaitUntil(() => test_balance_task.IsCompleted);
            Balance test_balance = test_balance_task.Result.Result;

            Assert.IsTrue(test_balance.CoinType.Equals(this.TestType));
            Assert.IsTrue(test_balance.CoinObjectCount == 2);
            Assert.IsTrue((int)test_balance.TotalBalance == 11);

            Task<RpcResult<IEnumerable<Balance>>> all_balance_task = this.PublishToolbox.Client.GetAllBalancesAsync
            (
                this.PublishToolbox.Account
            );
            yield return new WaitUntil(() => all_balance_task.IsCompleted);
            IEnumerable<Balance> all_balances = all_balance_task.Result.Result;

            Assert.IsTrue(all_balances.ToList().Count() == 2);
        }

        [UnityTest]
        public IEnumerator TotalSupplyTest()
        {
            Task<RpcResult<TotalSupply>> test_supply_task = this.PublishToolbox.Client.GetTotalSupplyAsync(this.TestType);
            yield return new WaitUntil(() => test_supply_task.IsCompleted);
            TotalSupply test_supply = test_supply_task.Result.Result;

            Assert.IsTrue((int)test_supply.Value == 11);
        }
    }
}
