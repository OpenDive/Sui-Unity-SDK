//
//  CheckpointTest.cs
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

namespace Sui.Tests
{
    public class CheckpointTest
    {
        TestToolbox Toolbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator CheckpointUpdateFetchTest()
        {
            Task<RpcResult<string>> checkpoint_sequence_number = this.Toolbox.Client.GetLatestCheckpointSequenceNumberAsync();
            yield return new WaitUntil(() => checkpoint_sequence_number.IsCompleted);

            Assert.Greater(int.Parse(checkpoint_sequence_number.Result.Result), -1);
        }

        [UnityTest]
        public IEnumerator CheckpointIDTest()
        {
            Task<RpcResult<Checkpoint>> resp = this.Toolbox.Client.GetCheckpointAsync("0");
            yield return new WaitUntil(() => resp.IsCompleted);
            Checkpoint checkpoint = resp.Result.Result;

            Assert.Greater(checkpoint.Digest.Length, 0);
            Assert.Greater(checkpoint.Transactions.Length, 0);
            Assert.NotNull(checkpoint.Epoch);
            Assert.NotNull(checkpoint.SequenceNumber);
            Assert.NotNull(checkpoint.NetworkTotalTransactions);
            Assert.NotNull(checkpoint.EpochRollingGasCostSummary);
            Assert.NotNull(checkpoint.TimestampMs);
        }
         
        [UnityTest]
        public IEnumerator CheckpointDigestTest()
        {
            Task<RpcResult<Checkpoint>> resp_1 = this.Toolbox.Client.GetCheckpointAsync("0");
            yield return new WaitUntil(() => resp_1.IsCompleted);
            Checkpoint checkpoint_1 = resp_1.Result.Result;

            Task<RpcResult<Checkpoint>> resp_2 = this.Toolbox.Client.GetCheckpointAsync(checkpoint_1.Digest);
            yield return new WaitUntil(() => resp_2.IsCompleted);
            Checkpoint checkpoint_2 = resp_2.Result.Result;

            Assert.True(checkpoint_1.Digest == checkpoint_2.Digest);
        }

        [UnityTest]
        public IEnumerator BulkCheckpointFetchTest()
        {
            Task<RpcResult<PaginatedCheckpoint>> resp = this.Toolbox.Client.GetCheckpointsAsync(new SuiRpcFilter(limit: 1, order: SortOrder.Ascending));
            yield return new WaitUntil(() => resp.IsCompleted);
            PaginatedCheckpoint checkpoints = resp.Result.Result;

            Assert.True(checkpoints.NextCursor == "0");
            Assert.True(checkpoints.Data.Length == 1);
            Assert.True(checkpoints.HasNextPage);

            Task<RpcResult<PaginatedCheckpoint>> resp_1 = this.Toolbox.Client.GetCheckpointsAsync(new SuiRpcFilter(limit: 1, cursor: checkpoints.NextCursor, order: SortOrder.Ascending));
            yield return new WaitUntil(() => resp_1.IsCompleted);
            PaginatedCheckpoint checkpoints_1 = resp_1.Result.Result;

            Assert.True(checkpoints_1.NextCursor == "1");
            Assert.True(checkpoints_1.Data.Length == 1);
            Assert.True(checkpoints_1.HasNextPage);
        }
    }
}
