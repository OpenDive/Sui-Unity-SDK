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
	public class GetChainIdentifierTests
	{
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetChainIdentifier/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<string> jsonObject = JsonConvert.DeserializeObject<RpcResult<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("4c78adac", jsonObject.Result);
        }

        [Test]
        public void Success2()
        {
            string success2Path = $"{path}/GetChainIdentifier/Success2.json";
            StreamReader reader = new StreamReader(success2Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<string> jsonObject = JsonConvert.DeserializeObject<RpcResult<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("a4a78cf3", jsonObject.Result);
        }

        [Test]
        public void Failure1()
        {
            string failure1Path = $"{path}/GetChainIdentifier/Failure1.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcValidResponse<string> jsonObject = JsonConvert.DeserializeObject<RpcValidResponse<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("Network error: Could not connect to the server.", jsonObject.Error.Message);
        }

        [Test]
        public void Failure2()
        {
            string failure2Path = $"{path}/GetChainIdentifier/Failure2.json";
            StreamReader reader = new StreamReader(failure2Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<string> jsonObject = JsonConvert.DeserializeObject<RpcResult<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("invalid_response", jsonObject.Result);
        }
    }
}

