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
    public class GetCheckpointsTests
    {
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetCheckpoints/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<Checkpoints> jsonObject = JsonConvert.DeserializeObject<RpcResult<Checkpoints>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual(5, jsonObject.Result.Data.Length);

            Checkpoint checkpoint = jsonObject.Result.Data[0];

            Assert.AreEqual("C5k3cUvNL1ejYNyy6Xgri78dSZCPENHJnY6wzketTccv", checkpoint.Digest);
            Assert.AreEqual(BigInteger.Parse("0"), checkpoint.SequenceNumber);
            Assert.AreEqual(BigInteger.Parse("0"), checkpoint.Epoch);
            Assert.AreEqual(BigInteger.Parse("1698349906298"), checkpoint.TimestampMs);
            Assert.AreEqual(BigInteger.Parse("1"), checkpoint.NetworkTotalTransactions);
            Assert.AreEqual("p5+pmMfn7ZmkBMSlptWFUpDijCI8+0zXuanBooLCSzBh5RjJTpQtHNnckfpBuIBa", checkpoint.ValidatorSignature);
            Assert.AreEqual(1, checkpoint.Transactions.Length);
            Assert.AreEqual("3P54PRYUMLfriLZ5i8Kmd13VLf58XwYywZKW8YfRn5jY", checkpoint.Transactions[0]);
        }

        [Test]
        public void Failure1()
        {
            string failure1Path = $"{path}/GetCheckpoints/Failure1.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcValidResponse<string> jsonObject = JsonConvert.DeserializeObject<RpcValidResponse<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("invalid value: integer `-1`, expected usize at line 1 column 2", jsonObject.Error.Message);
        }
    }
}
