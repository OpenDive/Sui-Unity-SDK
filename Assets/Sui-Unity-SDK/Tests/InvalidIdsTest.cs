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
using Newtonsoft.Json;

namespace Sui.Tests
{
    public class InvalidIdsTest
    {
        TestToolbox Toolbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator WrongAddressTest()
        {
            // Empty ID
            Task<RpcResult<PaginatedObjectsResponse>> owned_objects_empty_id_task = this.Toolbox.Client.GetOwnedObjects("");
            yield return new WaitUntil(() => owned_objects_empty_id_task.IsCompleted);

            Assert.IsNull(owned_objects_empty_id_task.Result.Result);
            Assert.IsTrue(owned_objects_empty_id_task.Result.Error.Message == "Unable to validate the address.");

            // More than 20 Bytes
            Task<RpcResult<DynamicFieldPage>> dynaic_field_incorrect_byte_length_task = this.Toolbox.Client.GetDynamicFields("0x0000000000000000000000004ce52ee7b659b610d59a1ced129291b3d0d4216322");
            yield return new WaitUntil(() => dynaic_field_incorrect_byte_length_task.IsCompleted);

            Assert.IsNull(dynaic_field_incorrect_byte_length_task.Result.Result);
            Assert.IsTrue(dynaic_field_incorrect_byte_length_task.Result.Error.Message == "Unable to validate the address.");

            // Wrong Batch Request (0xWRONG)
            string[] object_ids = new string[] { "0xBABE", "0xCAFE", "0xWRONG", "0xFACE" };
            Task<RpcResult<System.Collections.Generic.IEnumerable<ObjectDataResponse>>> wrong_batch_request_multi_object_task = this.Toolbox.Client.MultiGetObjects(object_ids);
            yield return new WaitUntil(() => wrong_batch_request_multi_object_task.IsCompleted);

            Assert.IsNull(wrong_batch_request_multi_object_task.Result.Result);
            Assert.IsTrue(wrong_batch_request_multi_object_task.Result.Error.Message == "Unable to validate the address.");
        }

        [UnityTest]
        public IEnumerator InvalidDigestTest()
        {
            // Empty Digest
            Task<RpcResult<TransactionBlockResponse>> empty_digest_task = this.Toolbox.Client.GetTransactionBlock("");
            yield return new WaitUntil(() => empty_digest_task.IsCompleted);

            Assert.IsNull(empty_digest_task.Result.Result);
            Assert.IsTrue(empty_digest_task.Result.Error.Message == "Invalid digest.");

            // Wrong Batch Request
            string[] digests = new string[] { "AQ7FA8JTGs368CvMkXj2iFz2WUWwzP6AAWgsLpPLxUmr", "wrong" };
            Task<RpcResult<System.Collections.Generic.IEnumerable<TransactionBlockResponse>>> wrong_digest_batch_task = this.Toolbox.Client.GetTransactionBlocks(digests);
            yield return new WaitUntil(() => wrong_digest_batch_task.IsCompleted);

            Assert.IsNull(wrong_digest_batch_task.Result.Result);
            Assert.IsTrue(wrong_digest_batch_task.Result.Error.Message == "Invalid digest.");
        }
    }
}
