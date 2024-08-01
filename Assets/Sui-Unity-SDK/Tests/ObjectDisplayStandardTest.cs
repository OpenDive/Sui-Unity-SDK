//
//  ObjectDisplayStandardTest.cs
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

namespace Sui.Tests
{
    public class ObjectDisplayStandardTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("display-test", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });
        }

        [UnityTest]
        public IEnumerator DisplayFieldErrorTest()
        {
            Task<RpcResult<PaginatedObjectDataResponse>> resp_task = this.Toolbox.Client.GetOwnedObjectsAsync
            (
                this.Toolbox.Account,
                new ObjectQuery
                (
                    object_data_filter: new ObjectDataFilterStructType($"{this.PackageID}::boars::Boar"),
                    object_data_options: new ObjectDataOptions(show_display: true, show_type: true)
                )
            );
            yield return new WaitUntil(() => resp_task.IsCompleted);

            if (resp_task.Result.Result.Data.Length == 0 || resp_task.Result.Result.Data[0].Data == null)
                Assert.Fail("Failed to get data from response.");

            ObjectData data = resp_task.Result.Result.Data[0].Data;

            AccountAddress boar_id = data.ObjectID;

            Task<RpcResult<ObjectDataResponse>> display_full_task = this.Toolbox.Client.GetObjectAsync(boar_id, new ObjectDataOptions(show_display: true));
            yield return new WaitUntil(() => display_full_task.IsCompleted);

            DisplayFieldsResponse display_full = display_full_task.Result.Result.Data.Display;
            System.Collections.Generic.Dictionary<string, string> display = display_full.Data;

            Assert.IsTrue(display["age"] == "10");
            Assert.IsTrue(display["buyer"] == this.Toolbox.Address().KeyHex);
            Assert.IsTrue(display["creator"] == "Chris");
            Assert.IsTrue(display["description"] == $"Unique Boar from the Boars collection with First Boar and {boar_id}");
            Assert.IsTrue(display["img_url"] == "https://get-a-boar.com/first.png");
            Assert.IsTrue(display["name"] == "First Boar");
            Assert.IsTrue(display["price"] == "");
            Assert.IsTrue(display["project_url"] == "https://get-a-boar.com/");
            Assert.IsTrue(display["full_url"] == "https://get-a-boar.fullurl.com/");
            Assert.IsTrue(display["escape_syntax"] == "{name}");

            string error = "Field value idd cannot be found in struct; Field value namee cannot be found in struct";
            Assert.IsTrue(display_full.Error.Equals(new ObjectResponseError(new ObjectResponseErrorDisplayError(error), ObjectResponseErrorType.DisplayError)));
        }

        [UnityTest]
        public IEnumerator NoDisplayObjectFetchTest()
        {
            Task<RpcResult<CoinPage>> coin_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coin_task.IsCompleted);

            AccountAddress coin_id = coin_task.Result.Result.Data[0].CoinObjectID;

            Task<RpcResult<ObjectDataResponse>> display_full_task = this.Toolbox.Client.GetObjectAsync(coin_id, new ObjectDataOptions(show_display: true));
            yield return new WaitUntil(() => display_full_task.IsCompleted);

            Assert.IsNull(display_full_task.Result.Result.Data.Display.Data);
        }
    }
}