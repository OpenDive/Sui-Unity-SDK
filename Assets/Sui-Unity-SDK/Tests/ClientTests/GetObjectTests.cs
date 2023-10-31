using NUnit.Framework;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Client;
using System.Collections.Generic;
using System.Numerics;

namespace Sui.Tests.Client
{
    public class GetObjectTests
    {
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/GetObject/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<ObjectDataResponse> jsonObject = JsonConvert.DeserializeObject<RpcResult<ObjectDataResponse>>(rawRpcResponse);

            Assert.NotNull(jsonObject);

            Assert.AreEqual("package", jsonObject.Result.Data.Type);
            Assert.AreEqual(SuiOwnerType.Immutable, jsonObject.Result.Data.Owner.Type);
            Assert.AreEqual("package", jsonObject.Result.Data.Content.DataType);
            Assert.AreEqual("// Move bytecode v6\nmodule 2.event {\n\n\nnative public emit<Ty0: copy + drop>(Arg0: Ty0)\n}", (string)((PackageData)jsonObject.Result.Data.Content).Disassembled["event"]);
            Assert.AreEqual("package", jsonObject.Result.Data.Bcs.DataType);
            Assert.AreEqual("0x0000000000000000000000000000000000000000000000000000000000000002", ((PackageRawData)jsonObject.Result.Data.Bcs).Id.ToHex());
            Assert.AreEqual("oRzrCwYAAAAGAQACAwIGBQgEBwwLCBcgDDcEAAEAAAABAQMBCQAABGVtaXQFZXZlbnQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgABAgAA", ((PackageRawData)jsonObject.Result.Data.Bcs).ModuleMap["event"]);
            Assert.AreEqual(43, ((PackageRawData)jsonObject.Result.Data.Bcs).ModuleMap.Count);
            Assert.AreEqual("tx_context", ((PackageRawData)jsonObject.Result.Data.Bcs).TypeOriginTable[0].ModuleName);
            Assert.AreEqual("TxContext", ((PackageRawData)jsonObject.Result.Data.Bcs).TypeOriginTable[0].StructName);
            Assert.AreEqual("0x0000000000000000000000000000000000000000000000000000000000000002", ((PackageRawData)jsonObject.Result.Data.Bcs).TypeOriginTable[0].Package.ToHex());
            Assert.AreEqual(64, ((PackageRawData)jsonObject.Result.Data.Bcs).TypeOriginTable.Count);
            Assert.AreEqual("0x0000000000000000000000000000000000000000000000000000000000000001", ((PackageRawData)jsonObject.Result.Data.Bcs).LinkageTable["0x0000000000000000000000000000000000000000000000000000000000000001"].UpgradedId.ToHex());
            Assert.AreEqual(BigInteger.Parse("1"), ((PackageRawData)jsonObject.Result.Data.Bcs).LinkageTable["0x0000000000000000000000000000000000000000000000000000000000000001"].UpgradedVersion);
        }
    }
}