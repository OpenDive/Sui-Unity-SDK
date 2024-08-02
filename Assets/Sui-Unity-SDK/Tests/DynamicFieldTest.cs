//
//  DynamicFieldsTest.cs
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
    public class DynamicFieldsTest
    {
        TestToolbox Toolbox;
        string PackageID;
        AccountAddress ParentObjectID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("dynamic-fields", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });

            Task<RpcResult<PaginatedObjectDataResponse>> owned_objects_task = this.Toolbox.Client.GetOwnedObjectsAsync
            (
                this.Toolbox.Account,
                new ObjectQuery
                (
                    object_data_filter: new ObjectDataFilterStructType($"{this.PackageID}::dynamic_fields_test::Test"),
                    object_data_options: new ObjectDataOptions(show_type: true)
                )
            );
            yield return new WaitUntil(() => owned_objects_task.IsCompleted);

            this.ParentObjectID = owned_objects_task.Result.Result.Data[0].Data.ObjectID;
        }

        [UnityTest]
        public IEnumerator DynamicFieldsFetchTest()
        {
            Task<RpcResult<PaginatedDynamicFieldInfo>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFieldsAsync(this.ParentObjectID);
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            Assert.IsTrue(dynamic_fields_task.Result.Result.Data.Length == 2);
        }

        [UnityTest]
        public IEnumerator DynamicFieldsLimitedFetchTest()
        {
            Task<RpcResult<PaginatedDynamicFieldInfo>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFieldsAsync(this.ParentObjectID, new ObjectQuery(limit: 1));
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            Assert.IsTrue(dynamic_fields_task.Result.Result.Data.Length == 1);
            Assert.NotNull(dynamic_fields_task.Result.Result.NextCursor);
        }

        [UnityTest]
        public IEnumerator DynamicFieldsNextPageFetchTest()
        {
            Task<RpcResult<PaginatedDynamicFieldInfo>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFieldsAsync(this.ParentObjectID, new ObjectQuery(limit: 1));
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            Assert.NotNull(dynamic_fields_task.Result.Result.NextCursor);

            Task<RpcResult<PaginatedDynamicFieldInfo>> dynamic_fields_cursor_task = this.Toolbox.Client.GetDynamicFieldsAsync(
                this.ParentObjectID,
                new ObjectQuery(cursor: dynamic_fields_task.Result.Result.NextCursor)
            );
            yield return new WaitUntil(() => dynamic_fields_cursor_task.IsCompleted);

            Assert.Greater(dynamic_fields_cursor_task.Result.Result.Data.Length, 0);
        }

        [UnityTest]
        public IEnumerator DynamicObjectFieldFetchTest()
        {
            Task<RpcResult<PaginatedDynamicFieldInfo>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFieldsAsync(this.ParentObjectID);
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            foreach (DynamicFieldInfo field in dynamic_fields_task.Result.Result.Data)
            {
                DynamicFieldName object_name = field.Name;
                Task<RpcResult<ObjectDataResponse>> object_task = this.Toolbox.Client.GetDynamicFieldObjectAsync(this.ParentObjectID, object_name.ToInput());
                yield return new WaitUntil(() => object_task.IsCompleted);

                Assert.IsTrue(object_task.Result.Result.Data.ObjectID == field.ObjectID);
            }
        }
    }
}