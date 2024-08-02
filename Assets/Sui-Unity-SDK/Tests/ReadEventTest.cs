//
//  ReadEventTest.cs
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
            Task<RpcResult<PaginatedEvent>> event_task = this.Toolbox.Client.QueryEventsAsync();
            yield return new WaitUntil(() => event_task.IsCompleted);

            Assert.Greater(event_task.Result.Result.Data.Length, 0);
        }

        [UnityTest]
        public IEnumerator EventPageFetchTest()
        {
            Task<RpcResult<PaginatedEvent>> page_1_task = this.Toolbox.Client.QueryEventsAsync(new EventQuery(limit: 2));
            yield return new WaitUntil(() => page_1_task.IsCompleted);

            Assert.IsTrue(page_1_task.Result.Result.NextCursor.EventSequence != "");
            Assert.IsTrue(page_1_task.Result.Result.NextCursor.TransactionDigest != "");
        }

        [UnityTest]
        public IEnumerator EventSenderPaginationFetchTest()
        {
            Task<RpcResult<PaginatedEvent>> query_1_task = this.Toolbox.Client.QueryEventsAsync
            (
                new EventQuery
                (
                    event_filter: new SenderEventFilter(this.Toolbox.Address().KeyHex),
                    limit: 2
                )
            );
            yield return new WaitUntil(() => query_1_task.IsCompleted);

            Assert.IsTrue(query_1_task.Result.Result.Data.Length == 0);
        }
    }
}
