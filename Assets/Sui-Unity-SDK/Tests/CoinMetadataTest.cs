using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using OpenDive.BCS;

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

            yield return this.Toolbox.PublishPackage("coin-metadata", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });
        }

        [UnityTest]
        public IEnumerator CoinMetadataAccessTest()
        {
            Task<RpcResult<CoinMetadata>> coin_metadata_task = this.Toolbox.Client.GetCoinMetadataAsync(new SuiStructTag($"{this.PackageID}::test::TEST"));
            yield return new WaitUntil(() => coin_metadata_task.IsCompleted);

            CoinMetadata coin_metadata = coin_metadata_task.Result.Result;
            Assert.IsTrue(coin_metadata.Decimals == 2);
            Assert.IsTrue(coin_metadata.Name == "Test Coin");
            Assert.IsTrue(coin_metadata.Description == "Test coin metadata");
            Assert.NotNull(coin_metadata.IconURL);
            Assert.IsTrue(coin_metadata.IconURL == "http://sui.io");
        }
    }
}
