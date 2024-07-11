using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;

namespace Sui.Tests
{
    public class ReadEventTest
    {
        TestToolbox Toolbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator EventFetchTest()
        {
            Task<RpcResult<EventPage>> event_task = this.Toolbox.Client.QueryEvents();
            yield return new WaitUntil(() => event_task.IsCompleted);

            Assert.Greater(event_task.Result.Result.Data.Length, 0);
        }

        [UnityTest]
        public IEnumerator EventPageFetchTest()
        {
            Task<RpcResult<EventPage>> page_1_task = this.Toolbox.Client.QueryEvents(limit: 2);
            yield return new WaitUntil(() => page_1_task.IsCompleted);

            Assert.IsTrue(page_1_task.Result.Result.NextCursor.EventSequence != "");
            Assert.IsTrue(page_1_task.Result.Result.NextCursor.TransactionDigest != "");
        }

        [UnityTest]
        public IEnumerator EventSenderPaginationFetchTest()
        {
            Task<RpcResult<EventPage>> query_1_task = this.Toolbox.Client.QueryEvents
            (
                query: new SenderEventFilter(this.Toolbox.Address()),
                limit: 2
            );
            yield return new WaitUntil(() => query_1_task.IsCompleted);

            Assert.IsTrue(query_1_task.Result.Result.Data.Length == 0);
        }
    }
}
