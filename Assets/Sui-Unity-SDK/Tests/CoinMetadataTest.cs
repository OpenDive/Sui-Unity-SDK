//
//  CoinMetadataTest.cs
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
using Sui.Types;

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
