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
	public class GetCheckpointTests
	{
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetCheckpoint/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<Checkpoint> jsonObject = JsonConvert.DeserializeObject<RpcResult<Checkpoint>>(rawRpcResponse);

            Assert.NotNull(jsonObject);

            Assert.AreEqual("C5k3cUvNL1ejYNyy6Xgri78dSZCPENHJnY6wzketTccv", jsonObject.Result.Digest);
            Assert.AreEqual(BigInteger.Parse("0"), jsonObject.Result.SequenceNumber);
            Assert.AreEqual(BigInteger.Parse("0"), jsonObject.Result.Epoch);
            Assert.AreEqual(BigInteger.Parse("1698349906298"), jsonObject.Result.TimestampMs);
            Assert.AreEqual(BigInteger.Parse("1"), jsonObject.Result.NetworkTotalTransactions);
            Assert.AreEqual("p5+pmMfn7ZmkBMSlptWFUpDijCI8+0zXuanBooLCSzBh5RjJTpQtHNnckfpBuIBa", jsonObject.Result.ValidatorSignature);
            Assert.AreEqual(1, jsonObject.Result.Transactions.Length);
            Assert.AreEqual("3P54PRYUMLfriLZ5i8Kmd13VLf58XwYywZKW8YfRn5jY", jsonObject.Result.Transactions[0]);
        }

        [Test]
        public void Success2()
        {
            string success2Path = $"{path}/GetCheckpoint/Success2.json";
            StreamReader reader = new StreamReader(success2Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<Checkpoint> jsonObject = JsonConvert.DeserializeObject<RpcResult<Checkpoint>>(rawRpcResponse);

            Assert.NotNull(jsonObject);

            Assert.AreEqual("86g1MiGo7dUjRJmeoDvY6JAuUF5MUb8mTAKQK9TU8mAF", jsonObject.Result.Digest);
            Assert.AreEqual(BigInteger.Parse("1"), jsonObject.Result.SequenceNumber);
            Assert.AreEqual(BigInteger.Parse("0"), jsonObject.Result.Epoch);
            Assert.AreEqual(BigInteger.Parse("1698349910861"), jsonObject.Result.TimestampMs);
            Assert.AreEqual(BigInteger.Parse("2"), jsonObject.Result.NetworkTotalTransactions);
            Assert.AreEqual("pPNWzln3ic0iVhjrTlSi1g0qILuRMbl4N6ugywMMkApDyUKlqcpgutB+j4DkcGDb", jsonObject.Result.ValidatorSignature);
            Assert.AreEqual(1, jsonObject.Result.Transactions.Length);
            Assert.AreEqual("GPSthahweMQjCxQHkSa8T2XREF4bxbQC8xR3rKAVbBog", jsonObject.Result.Transactions[0]);
            Assert.AreEqual("C5k3cUvNL1ejYNyy6Xgri78dSZCPENHJnY6wzketTccv", jsonObject.Result.PreviousDigest);
        }

        [Test]
        public void Failure1()
        {
            string failure1Path = $"{path}/GetCheckpoint/Failure1.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcValidResponse<string> jsonObject = JsonConvert.DeserializeObject<RpcValidResponse<string>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("Verified checkpoint not found for sequence number: 1234567890", jsonObject.Error.Message);
        }
    }
}

