using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using UnityEditor.VersionControl;

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

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("display-test");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;
        }

        [UnityTest]
        public IEnumerator DisplayFieldErrorTest()
        {
            Task<RpcResult<PaginatedObjectsResponse>> resp_task = this.Toolbox.Client.GetOwnedObjects
            (
                this.Toolbox.Address(),
                new ObjectDataFilterStructType($"{this.PackageID}::boars::Boar"),
                new ObjectDataOptions(show_display: true, show_type: true)
            );
            yield return new WaitUntil(() => resp_task.IsCompleted);

            if (resp_task.Result.Result.Data.Length == 0 || resp_task.Result.Result.Data[0].Data == null)
                Assert.Fail("Failed to get data from response.");

            ObjectData data = resp_task.Result.Result.Data[0].Data;

            string boar_id = data.ObjectId;

            Task<RpcResult<ObjectDataResponse>> display_full_task = this.Toolbox.Client.GetObject(boar_id, new ObjectDataOptions(show_display: true));
            yield return new WaitUntil(() => display_full_task.IsCompleted);

            DisplayFieldsResponse display_full = display_full_task.Result.Result.Data.Display;
            System.Collections.Generic.Dictionary<string, string> display = display_full.Data;

            Assert.IsTrue(display["age"] == "10");
            Assert.IsTrue(display["buyer"] == this.Toolbox.Address());
            Assert.IsTrue(display["creator"] == "Chris");
            Assert.IsTrue(display["description"] == $"Unique Boar from the Boars collection with First Boar and {boar_id}");
            Assert.IsTrue(display["img_url"] == "https://get-a-boar.com/first.png");
            Assert.IsTrue(display["name"] == "First Boar");
            Assert.IsTrue(display["price"] == "");
            Assert.IsTrue(display["project_url"] == "https://get-a-boar.com/");
            Assert.IsTrue(display["full_url"] == "https://get-a-boar.fullurl.com/");
            Assert.IsTrue(display["escape_syntax"] == "{name}");

            string error = "Field value idd cannot be found in struct; Field value namee cannot be found in struct";
            Assert.IsTrue(display_full.Error.Equals(new DisplayError(error)));
        }

        [UnityTest]
        public IEnumerator NoDisplayObjectFetchTest()
        {
            Task<RpcResult<CoinPage>> coin_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coin_task.IsCompleted);

            string coin_id = coin_task.Result.Result.Data[0].CoinObjectId;

            Task<RpcResult<ObjectDataResponse>> display_full_task = this.Toolbox.Client.GetObject(coin_id, new ObjectDataOptions(show_display: true));
            yield return new WaitUntil(() => display_full_task.IsCompleted);

            Assert.IsNull(display_full_task.Result.Result.Data.Display.Data);
        }
    }
}