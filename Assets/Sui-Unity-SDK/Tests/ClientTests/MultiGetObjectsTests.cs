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
	public class MultiGetObjectTest
	{
        string path = Application.dataPath + "/Sui-Unity-SDK/Tests/ClientTests/Responses";

        [Test]
        public void Success1()
        {
            string success1Path = $"{path}/MultiGetObjects/Success1.json";
            StreamReader reader = new StreamReader(success1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<SuiObjectResponse[]> jsonObject = JsonConvert.DeserializeObject<RpcResult<SuiObjectResponse[]>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual(2, jsonObject.Result.Length);

            Assert.AreEqual("0xd0f544b2bbe3f65c194d3cf30bd3771439f89786fd06002127c16becd497815b", jsonObject.Result[0].Data.ObjectId);
            Assert.AreEqual(BigInteger.Parse("7217843"), jsonObject.Result[0].Data.Version);
            Assert.AreEqual("4RHWpwbUuZbmSdTebHVmpQPKTYd5V75zD9U9bY4DwhMr", jsonObject.Result[0].Data.Digest);

            Assert.AreEqual("0x1b98b1f1efdd0361d8fead88072b32d9385d94b64124827740d42c38e2fa55ad", jsonObject.Result[1].Data.ObjectId);
            Assert.AreEqual(BigInteger.Parse("7217843"), jsonObject.Result[1].Data.Version);
            Assert.AreEqual("J7hPuVZ4rUCPs4rKhCTdq5fXmA952P5d6qyKc5Y2KZ5p", jsonObject.Result[1].Data.Digest);
        }

        [Test]
        public void Success2()
        {
            string success2Path = $"{path}/MultiGetObjects/Success2.json";
            StreamReader reader = new StreamReader(success2Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<SuiObjectResponse[]> jsonObject = JsonConvert.DeserializeObject<RpcResult<SuiObjectResponse[]>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual(4, jsonObject.Result.Length);

            Assert.AreEqual("package", jsonObject.Result[0].Data.Type);
            Assert.AreEqual(SuiOwnerType.Immutable, jsonObject.Result[0].Data.Owner.Type);
            Assert.AreEqual("package", jsonObject.Result[0].Data.Content.DataType);
            Assert.AreEqual("// Move bytecode v6\nmodule 2.event {\n\n\nnative public emit<Ty0: copy + drop>(Arg0: Ty0)\n}", (string)((PackageData)jsonObject.Result[0].Data.Content).Disassembled["event"]);
            Assert.AreEqual("package", jsonObject.Result[0].Data.Bcs.DataType);
            Assert.AreEqual("0x0000000000000000000000000000000000000000000000000000000000000002", ((PackageRawData)jsonObject.Result[0].Data.Bcs).Id.ToHex());
            Assert.AreEqual("oRzrCwYAAAAGAQACAwIGBQgEBwwLCBcgDDcEAAEAAAABAQMBCQAABGVtaXQFZXZlbnQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgABAgAA", ((PackageRawData)jsonObject.Result[0].Data.Bcs).ModuleMap["event"]);
            Assert.AreEqual(45, ((PackageRawData)jsonObject.Result[0].Data.Bcs).ModuleMap.Count);
            Assert.AreEqual("tx_context", ((PackageRawData)jsonObject.Result[0].Data.Bcs).TypeOriginTable[0].ModuleName);
            Assert.AreEqual("TxContext", ((PackageRawData)jsonObject.Result[0].Data.Bcs).TypeOriginTable[0].StructName);
            Assert.AreEqual("0x0000000000000000000000000000000000000000000000000000000000000002", ((PackageRawData)jsonObject.Result[0].Data.Bcs).TypeOriginTable[0].Package.ToHex());
            Assert.AreEqual(67, ((PackageRawData)jsonObject.Result[0].Data.Bcs).TypeOriginTable.Count);
            Assert.AreEqual("0x0000000000000000000000000000000000000000000000000000000000000001", ((PackageRawData)jsonObject.Result[0].Data.Bcs).LinkageTable["0x0000000000000000000000000000000000000000000000000000000000000001"].UpgradedId.ToHex());
            Assert.AreEqual(BigInteger.Parse("0"), ((PackageRawData)jsonObject.Result[0].Data.Bcs).LinkageTable["0x0000000000000000000000000000000000000000000000000000000000000001"].UpgradedVersion);

            Assert.AreEqual("0x57138e18b82cc8ea6e92c3d5737d6078b1304b655f59cf5ae9668cc44aad4ead::profile::Profile", jsonObject.Result[1].Data.Type);
            Assert.AreEqual(SuiOwnerType.AddressOwner, jsonObject.Result[1].Data.Owner.Type);
            Assert.AreEqual("0x00698416819daaf2b676f65f4d726150c0e1db3f890a9cfe8419bd8c16681996", jsonObject.Result[1].Data.Owner.Address);
            Assert.NotNull(jsonObject.Result[1].Data.Display.Data);
            Assert.AreEqual("https://polymedia.app", jsonObject.Result[1].Data.Display.Data["creator"]);
            Assert.AreEqual("Fren", jsonObject.Result[1].Data.Display.Data["description"]);
            Assert.AreEqual("https://api-mainnet.suifrens.sui.io/suifrens/0xe48b2b60846db20cac7af3cb3accf7b9d2bd7f5e1a5b573faab1722cd8e214f3/svg", jsonObject.Result[1].Data.Display.Data["image_url"]);

            Assert.AreEqual("0x7c423c0f1ab19c99155c24e98fdb971453b699c90ab579b23b38103060ea26db::journey::Quest", jsonObject.Result[2].Data.Type);
            Assert.AreEqual(SuiOwnerType.ObjectOwner, jsonObject.Result[2].Data.Owner.Type);
            Assert.AreEqual("0x36e929a9b61863a280b1fb383dfbaf3800e3734d675f7847ad7c7e626de331c0", jsonObject.Result[2].Data.Owner.Address);
            Assert.NotNull(jsonObject.Result[2].Data.Display.Data);
            Assert.AreEqual("https://polymedia.app", jsonObject.Result[2].Data.Display.Data["creator"]);
            Assert.AreEqual("The door to the invisible must be visible", jsonObject.Result[2].Data.Display.Data["description"]);
            Assert.AreEqual("https://journey.polymedia.app/img/card_explorer.webp", jsonObject.Result[2].Data.Display.Data["image_url"]);

            Assert.AreEqual("0x2::dynamic_field::Field<0x2::dynamic_object_field::Wrapper<0x1::string::String>, 0x2::object::ID>", jsonObject.Result[3].Data.Type);
            Assert.AreEqual(SuiOwnerType.ObjectOwner, jsonObject.Result[3].Data.Owner.Type);
            Assert.AreEqual("0xd0f544b2bbe3f65c194d3cf30bd3771439f89786fd06002127c16becd497815b", jsonObject.Result[3].Data.Owner.Address);
            Assert.Null(jsonObject.Result[3].Data.Display.Data);
        }

        [Test]
        public void Failure1()
        {
            string failure1Path = $"{path}/MultiGetObjects/Failure1.json";
            StreamReader reader = new StreamReader(failure1Path);
            string rawRpcResponse = reader.ReadToEnd();

            RpcResult<SuiObjectResponse[]> jsonObject = JsonConvert.DeserializeObject<RpcResult<SuiObjectResponse[]>>(rawRpcResponse);

            Assert.NotNull(jsonObject);
            Assert.AreEqual("notExists", jsonObject.Result[0].Error.Code);
            Assert.AreEqual("0x0000000000000012300000000000000000000000000000000000000000000002", ((NotExistsError)jsonObject.Result[0].Error).ObjectId.ToHex());
        }
    }
}