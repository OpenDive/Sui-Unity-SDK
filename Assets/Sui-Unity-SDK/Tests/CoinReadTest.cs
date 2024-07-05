﻿using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sui.Tests
{
    public class CoinReadTest
    {
        TestToolbox Toolbox;
        TestToolbox PublishToolbox;
        string PackageID;
        string TestType;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            this.PublishToolbox = new TestToolbox();

            yield return this.Toolbox.Setup();
            yield return this.PublishToolbox.Setup();

            Task<PublishedPackage> task = this.PublishToolbox.PublishPackage("coin-metadata");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;
            this.TestType = $"{this.PackageID}::test::TEST";
        }

        [UnityTest]
        public IEnumerator NoTypeFunctionTest()
        {
            Task<RpcResult<CoinPage>> sui_coin_task = this.Toolbox.Client.GetCoins(this.Toolbox.Account.SuiAddress());
            yield return new WaitUntil(() => sui_coin_task.IsCompleted);
            CoinPage sui_coin = sui_coin_task.Result.Result;

            Assert.IsTrue(sui_coin.Data.Count == 5);

            Task<RpcResult<CoinPage>> test_coin_task = this.PublishToolbox.Client.GetCoins
            (
                this.PublishToolbox.Account.SuiAddress(),
                this.TestType
            );
            yield return new WaitUntil(() => test_coin_task.IsCompleted);
            CoinPage test_coin = test_coin_task.Result.Result;

            Assert.IsTrue(test_coin.Data.Count == 2);

            Task<RpcResult<CoinPage>> all_coins_task = this.Toolbox.Client.GetAllCoins(this.Toolbox.Account.SuiAddress());
            yield return new WaitUntil(() => all_coins_task.IsCompleted);
            CoinPage all_coins = all_coins_task.Result.Result;

            Assert.IsTrue(all_coins.Data.Count == 5);
            Assert.IsFalse(all_coins.HasNextPage);

            Task<RpcResult<CoinPage>> publisher_all_coins_task = this.Toolbox.Client.GetAllCoins(this.PublishToolbox.Account.SuiAddress());
            yield return new WaitUntil(() => publisher_all_coins_task.IsCompleted);
            CoinPage publisher_all_coins = publisher_all_coins_task.Result.Result;

            Assert.IsTrue(publisher_all_coins.Data.Count == 3);
            Assert.IsFalse(all_coins.HasNextPage);

            Task<RpcResult<CoinPage>> some_sui_coin_task = this.Toolbox.Client.GetCoins(this.Toolbox.Account.SuiAddress(), null, null, 3);
            yield return new WaitUntil(() => some_sui_coin_task.IsCompleted);
            CoinPage some_sui_coin = some_sui_coin_task.Result.Result;

            Assert.IsTrue(some_sui_coin.Data.Count == 3);
            Assert.IsTrue(some_sui_coin.HasNextPage);
        }

        [UnityTest]
        public IEnumerator BalanceWithTypeTest()
        {
            Task<RpcResult<Balance>> sui_balance_task = this.Toolbox.Client.GetBalanceAsync(this.Toolbox.Account.SuiAddress());
            yield return new WaitUntil(() => sui_balance_task.IsCompleted);
            Balance sui_balance = sui_balance_task.Result.Result;

            Assert.IsTrue(sui_balance.cointType == "0x2::sui::SUI");
            Assert.IsTrue(sui_balance.CoinObjectCount == 5);
            Assert.IsTrue(sui_balance.TotalBalance != BigInteger.Zero);

            Task<RpcResult<Balance>> test_balance_task = this.PublishToolbox.Client.GetBalanceAsync
            (
                this.PublishToolbox.Account.SuiAddress(),
                this.TestType
            );
            yield return new WaitUntil(() => test_balance_task.IsCompleted);
            Balance test_balance = test_balance_task.Result.Result;

            Assert.IsTrue(test_balance.cointType == this.TestType);
            Assert.IsTrue(test_balance.CoinObjectCount == 2);
            Assert.IsTrue((int)test_balance.TotalBalance == 11);

            Task<RpcResult<IEnumerable<Balance>>> all_balance_task = this.PublishToolbox.Client.GetAllBalancesAsync
            (
                this.PublishToolbox.Account.SuiAddress()
            );
            yield return new WaitUntil(() => all_balance_task.IsCompleted);
            IEnumerable<Balance> all_balances = all_balance_task.Result.Result;

            Assert.IsTrue(all_balances.ToList().Count() == 2);
        }

        [UnityTest]
        public IEnumerator TotalSupplyTest()
        {
            Task<RpcResult<TotalSupply>> test_supply_task = this.PublishToolbox.Client.GetTotalSupply(this.TestType);
            yield return new WaitUntil(() => test_supply_task.IsCompleted);
            TotalSupply test_supply = test_supply_task.Result.Result;

            Assert.IsTrue((int)test_supply.Value == 11);
        }
    }
}