using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;

namespace Sui.Tests
{
    public class DynamicFieldsTest
    {
        TestToolbox Toolbox;
        string PackageID;
        string ParentObjectID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("dynamic-fields");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;

            Task<RpcResult<PaginatedObjectsResponse>> owned_objects_task = this.Toolbox.Client.GetOwnedObjects
            (
                this.Toolbox.Account.SuiAddress(),
                new ObjectDataFilterStructType($"{this.PackageID}::dynamic_fields_test::Test"),
                new ObjectDataOptions(show_type: true)
            );
            yield return new WaitUntil(() => owned_objects_task.IsCompleted);

            this.ParentObjectID = owned_objects_task.Result.Result.Data[0].Data.ObjectId;
        }

        [UnityTest]
        public IEnumerator DynamicFieldsFetchTest()
        {
            Task<RpcResult<DynamicFieldPage>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFields(this.ParentObjectID);
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            Assert.IsTrue(dynamic_fields_task.Result.Result.Data.Length == 2);
        }

        [UnityTest]
        public IEnumerator DynamicFieldsLimitedFetchTest()
        {
            Task<RpcResult<DynamicFieldPage>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFields(this.ParentObjectID, null, null, null, 1);
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            Assert.IsTrue(dynamic_fields_task.Result.Result.Data.Length == 1);
            Assert.NotNull(dynamic_fields_task.Result.Result.NextCursor);
        }

        [UnityTest]
        public IEnumerator DynamicFieldsNextPageFetchTest()
        {
            Task<RpcResult<DynamicFieldPage>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFields(this.ParentObjectID, null, null, null, 1);
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            Assert.NotNull(dynamic_fields_task.Result.Result.NextCursor);

            Task<RpcResult<DynamicFieldPage>> dynamic_fields_cursor_task = this.Toolbox.Client.GetDynamicFields(
                this.ParentObjectID,
                null,
                null,
                dynamic_fields_task.Result.Result.NextCursor
            );
            yield return new WaitUntil(() => dynamic_fields_cursor_task.IsCompleted);

            Assert.Greater(dynamic_fields_cursor_task.Result.Result.Data.Length, 0);
        }

        [UnityTest]
        public IEnumerator DynamicObjectFieldFetchTest()
        {
            Task<RpcResult<DynamicFieldPage>> dynamic_fields_task = this.Toolbox.Client.GetDynamicFields(this.ParentObjectID);
            yield return new WaitUntil(() => dynamic_fields_task.IsCompleted);

            foreach (DynamicFieldInfo field in dynamic_fields_task.Result.Result.Data)
            {
                DynamicFieldName object_name = field.Name;
                Task<RpcResult<ObjectDataResponse>> object_task = this.Toolbox.Client.GetDynamicFieldObject(this.ParentObjectID, object_name);
                yield return new WaitUntil(() => object_task.IsCompleted);

                Assert.IsTrue(object_task.Result.Result.Data.ObjectId == field.ObjectID);
            }
        }
    }
}