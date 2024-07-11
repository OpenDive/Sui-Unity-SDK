using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;

namespace Sui.Tests
{
    public class ProtocolConfigTest
    {
        TestToolbox Toolbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator ProtocolConfigFetchTest()
        {
            Task<RpcResult<ProtocolConfig>> config_task = this.Toolbox.Client.GetProtocolConfigAsync();
            yield return new WaitUntil(() => config_task.IsCompleted);

            Assert.IsTrue(config_task.Result.Result.ProtocolVersion != "");
        }
    }
}
