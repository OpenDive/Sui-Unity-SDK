using NUnit.Framework;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using System.Numerics;

namespace Sui.Tests.Client
{
    public class GetNormalizedMoveFunctionTests
    {
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetNormalizedMoveFunction/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<NormalizedMoveFunctionResponse> jsonObject = JsonConvert.DeserializeObject<RpcResult<NormalizedMoveFunctionResponse>>(rawRpcResponse);

            Assert.NotNull(jsonObject);

            Assert.AreEqual("Public", jsonObject.Result.Visibility);
            Assert.AreEqual(false, jsonObject.Result.IsEntry);
            Assert.AreEqual(0, jsonObject.Result.TypeParameters.Count);
            Assert.AreEqual(8, jsonObject.Result.Parameters.Count);
            Assert.AreEqual(1, jsonObject.Result.Return.Count);
        }

        [Test]
        public void Success2()
        {
            string success2Path = $"{path}/GetNormalizedMoveFunction/Success2.json";
            StreamReader reader = new StreamReader(success2Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<NormalizedMoveFunctionResponse> jsonObject = JsonConvert.DeserializeObject<RpcResult<NormalizedMoveFunctionResponse>>(rawRpcResponse);

            Assert.NotNull(jsonObject);

            Assert.AreEqual("Public", jsonObject.Result.Visibility);
            Assert.AreEqual(false, jsonObject.Result.IsEntry);
            Assert.AreEqual(1, jsonObject.Result.TypeParameters.Count);
            Assert.AreEqual(1, jsonObject.Result.Parameters.Count);
            Assert.AreEqual(1, jsonObject.Result.Return.Count);
        }

        [Test]
        public void Failure1()
        {
            string failure1Path = $"{path}/GetNormalizedMoveFunction/Failure1.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcValidResponse<string> jsonObject = JsonConvert.DeserializeObject<RpcValidResponse<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("Package object does not exist with ID 0x0000000000000000000000000000000000000000000000000000000000000012", jsonObject.Error.Message);
        }

        [Test]
        public void Failure2()
        {
            string failure1Path = $"{path}/GetNormalizedMoveFunction/Failure2.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcValidResponse<string> jsonObject = JsonConvert.DeserializeObject<RpcValidResponse<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("No module found with module name not_a_module", jsonObject.Error.Message);
        }

        [Test]
        public void Failure3()
        {
            string failure1Path = $"{path}/GetNormalizedMoveFunction/Failure3.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcValidResponse<string> jsonObject = JsonConvert.DeserializeObject<RpcValidResponse<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("No function was found with function name not_a_function", jsonObject.Error.Message);
        }
    }
}