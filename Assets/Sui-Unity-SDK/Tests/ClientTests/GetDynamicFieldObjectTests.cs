using NUnit.Framework;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Sui.Rpc;
using System.Numerics;

namespace Sui.Tests.Client
{
    public class GetDynamicFieldObjectTests
    {
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetDynamicFieldObject/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<ObjectDataResponse> jsonObject = JsonConvert.DeserializeObject<RpcResult<ObjectDataResponse>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            ObjectData objData = jsonObject.Result.Data;

            Assert.AreEqual("0x230a2f7c34369bcba0d075f75a20e3857decc0d7539bb6ab101f8453725ee67f", objData.ObjectId);
            Assert.AreEqual(BigInteger.Parse("32603500"), objData.Version);
            Assert.AreEqual("EqEhCYAov7yDW3xVa6C2r1zx7Zc9ucvWtMbpY8idjGKp", objData.Digest);
            Assert.AreEqual("0x2::dynamic_field::Field<0x2::kiosk::Listing, u64>", objData.Type);
            Assert.AreEqual(SuiOwnerType.ObjectOwner, objData.Owner.Type);
            Assert.AreEqual("0xb57fba584a700a5bcb40991e1b2e6bf68b0f3896d767a0da92e69de73de226ac", objData.Owner.Address);
            Assert.AreEqual("798my1pu54zkaip2SZHtpgx7P9rN6YWGVRhqRtYnVcNP", objData.PreviousTransaction);
            Assert.AreEqual(BigInteger.Parse("2014000"), objData.StorageRebate);
            Assert.AreEqual("moveObject", objData.Content.DataType);
            Assert.AreEqual("0x2::dynamic_field::Field<0x2::kiosk::Listing, u64>", ((MoveObjectData)objData.Content).Type);
            Assert.AreEqual(false, ((MoveObjectData)objData.Content).HasPublicTransfer);
            Assert.AreEqual("0x230a2f7c34369bcba0d075f75a20e3857decc0d7539bb6ab101f8453725ee67f", ((ObjectIDMoveValue)((AdditionalPropertiesMoveStruct)((MoveObjectData)objData.Content).Fields).AdditionalProperties["id"]).Id);
        }
    }
}