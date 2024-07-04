using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using OpenDive.BCS;
using Sui.Accounts;
using System;

namespace Sui.Tests
{
    public class CoinTest
    {
        TestToolbox Toolbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator CoinUtilityTest()
        {
            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);
            CoinPage coins = coins_task.Result.Result;

            Assert.Greater(coins.Data.Count, 0);
        }

        [UnityTest]
        public IEnumerator CoinStructTagTest()
        {
            SuiStructTag sui_struct_tag = new SuiStructTag(AccountAddress.FromHex("0x2"), "sui", "SUI", Array.Empty<ISerializableTag>());
            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);
            CoinPage coins = coins_task.Result.Result;

            Assert.IsTrue(new SuiStructTag(coins.Data[0].CoinType).Equals(sui_struct_tag));
        }
    }
}
