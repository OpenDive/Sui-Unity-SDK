//
//  InvalidIdsTest.cs
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
            Task<RpcResult<IEnumerable<TransactionBlockResponse>>> wrong_digest_batch_task = this.Toolbox.Client.MultiGetTransactionBlocksAsync(digests);
            yield return new WaitUntil(() => wrong_digest_batch_task.IsCompleted);

            Assert.IsNull(wrong_digest_batch_task.Result.Result);
            Assert.IsTrue(wrong_digest_batch_task.Result.Error.Message == "Invalid digests.");
        }
    }
}