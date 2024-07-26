using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Collections.Generic;

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
        public IEnumerator InvalidDigestTest()
        {
            // Empty Digest
            Task<RpcResult<TransactionBlockResponse>> empty_digest_task = this.Toolbox.Client.GetTransactionBlockAsync("");
            yield return new WaitUntil(() => empty_digest_task.IsCompleted);

            Assert.IsNull(empty_digest_task.Result.Result);
            Assert.IsTrue(empty_digest_task.Result.Error.Message == "Invalid digest.");

            // Wrong Batch Request
            List<string> digests = new List<string> { "AQ7FA8JTGs368CvMkXj2iFz2WUWwzP6AAWgsLpPLxUmr", "wrong" };
            Task<RpcResult<System.Collections.Generic.IEnumerable<TransactionBlockResponse>>> wrong_digest_batch_task = this.Toolbox.Client.MultiGetTransactionBlocksAsync(digests);
            yield return new WaitUntil(() => wrong_digest_batch_task.IsCompleted);

            Assert.IsNull(wrong_digest_batch_task.Result.Result);
            Assert.IsTrue(wrong_digest_batch_task.Result.Error.Message == "Invalid digests.");
        }
    }
}
