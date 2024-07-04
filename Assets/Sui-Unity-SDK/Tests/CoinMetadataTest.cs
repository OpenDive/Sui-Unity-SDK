using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Rpc;
using Sui.Clients;
using Sui.Rpc.Models;

namespace Sui.Tests
{
    public class CoinMetadataTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("coin-metadata");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;
        }

        [UnityTest]
        public IEnumerator CoinMetadataAccessTest()
        {
            Task<RpcResult<CoinMetadata>> coin_metadata_task = this.Toolbox.Client.GetCoinMetadata($"{this.PackageID}::test::TEST");
            yield return new WaitUntil(() => coin_metadata_task.IsCompleted);

            CoinMetadata coin_metadata = coin_metadata_task.Result.Result;
            Assert.IsTrue(coin_metadata.Decimals == 2);
            Assert.IsTrue(coin_metadata.Name == "Test Coin");
            Assert.IsTrue(coin_metadata.Description == "Test coin metadata");
            Assert.NotNull(coin_metadata.IconUrl);
            Assert.IsTrue(coin_metadata.IconUrl == "http://sui.io");
        }
    }
}
