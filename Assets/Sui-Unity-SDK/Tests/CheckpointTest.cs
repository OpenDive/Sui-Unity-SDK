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
            Task<RpcResult<string>> checkpoint_sequence_number = this.Toolbox.Client.GetLatestCheckpointSequenceNumber();
            yield return new WaitUntil(() => checkpoint_sequence_number.IsCompleted);

            Assert.Greater(int.Parse(checkpoint_sequence_number.Result.Result), -1);
        }

        [UnityTest]
        public IEnumerator CheckpointIDTest()
        {
            Task<RpcResult<Checkpoint>> resp = this.Toolbox.Client.GetCheckpoint("0");
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
            Task<RpcResult<Checkpoint>> resp_1 = this.Toolbox.Client.GetCheckpoint("0");
            yield return new WaitUntil(() => resp_1.IsCompleted);
            Checkpoint checkpoint_1 = resp_1.Result.Result;

            Task<RpcResult<Checkpoint>> resp_2 = this.Toolbox.Client.GetCheckpoint(checkpoint_1.Digest);
            yield return new WaitUntil(() => resp_2.IsCompleted);
            Checkpoint checkpoint_2 = resp_2.Result.Result;

            Assert.True(checkpoint_1.Digest == checkpoint_2.Digest);
        }

        [UnityTest]
        public IEnumerator BulkCheckpointFetchTest()
        {
            Task<RpcResult<Checkpoints>> resp = this.Toolbox.Client.GetCheckpoints(null, 1, false);
            yield return new WaitUntil(() => resp.IsCompleted);
            Checkpoints checkpoints = resp.Result.Result;

            Assert.True(checkpoints.NextCursor == "0");
            Assert.True(checkpoints.Data.Length == 1);
            Assert.True(checkpoints.HasNextPage);

            Task<RpcResult<Checkpoints>> resp_1 = this.Toolbox.Client.GetCheckpoints(checkpoints.NextCursor, 1, false);
            yield return new WaitUntil(() => resp_1.IsCompleted);
            Checkpoints checkpoints_1 = resp_1.Result.Result;

            Assert.True(checkpoints_1.NextCursor == "1");
            Assert.True(checkpoints_1.Data.Length == 1);
            Assert.True(checkpoints_1.HasNextPage);
        }
    }
}
