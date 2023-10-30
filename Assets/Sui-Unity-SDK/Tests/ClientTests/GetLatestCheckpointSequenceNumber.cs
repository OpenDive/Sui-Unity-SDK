using NUnit.Framework;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;

namespace Sui.Tests.Client
{
    public class GetLatestCheckpointSequenceNumber
    {
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetLatestCheckpointSequenceNumber/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<string> jsonObject = JsonConvert.DeserializeObject<RpcResult<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("1978", jsonObject.Result);
        }
    }
}