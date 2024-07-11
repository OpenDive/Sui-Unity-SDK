using NUnit.Framework;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Builder;
using Sui.Transactions.Kinds;
using UnityEngine;
using Sui.Utilities;
using Sui.Transactions.Types;
using Sui.Transactions.Types.Arguments;
using Sui.Types;
using Sui.Transactions;
using Sui.Rpc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sui.Clients;
using UnityEngine.TestTools;
using Sui.Rpc.Models;
using Newtonsoft.Json;

namespace Sui.Tests
{
    public class TXBuilderTest: MonoBehaviour
    {
        string suiAddressHex = "0x0000000000000000000000000000000000000000000000000000000000000002";

        //[UnityTest]
        //public IEnumerator SimpleTransactionMoveCallBuildTest()
        //{
        //    AccountAddress sui_address = AccountAddress.FromHex(suiAddressHex);

        //    TestToolbox toolbox = new TestToolbox();
        //    yield return toolbox.Setup();

        //    Task<PublishedPackage> publish_task = toolbox.PublishPackage("serializer");
        //    yield return new WaitUntil(() => publish_task.IsCompleted);

        //    PublishedPackage package_id = publish_task.Result;

        //    yield return toolbox.Setup();

        //    Task<RpcResult<CoinPage>> coin_task = toolbox.GetCoins();
        //    yield return new WaitUntil(() => coin_task.IsCompleted);

        //    CoinPage coins = coin_task.Result.Result;
        //    CoinDetails coin_0 = coins.Data[0];

        //    var tx = new Sui.Transactions.TransactionBlock();

        //    tx.AddMoveCallTx(
        //        new SuiMoveNormalizedStructType(new SuiStructTag(sui_address, "pay", "split", new SerializableTypeTag[0]), new SuiMoveNormalizedType[] { }), // TODO: THIS IS A NORMALIZED STRUCT
        //        new SerializableTypeTag[] { new SerializableTypeTag(new StructTag(sui_address, "sui", "SUI", new ISerializableTag[0])) },
        //        new SuiTransactionArgument[]
        //        {
        //                new SuiTransactionArgument(tx.AddObjectInput(coin_0.CoinObjectId)),
        //                new SuiTransactionArgument(tx.AddInput(Types.Type.Pure, new U64((ulong)(toolbox.DefaultGasBudget * 2))))
        //        }
        //    );

        //    tx.SetSenderIfNotSet(toolbox.Account.AccountAddress);
        //    Task<string> digest_task = tx.GetDigest(new BuildOptions(toolbox.Client));
        //    yield return new WaitUntil(() => digest_task.IsCompleted);

        //    string local_digest = digest_task.Result;
        //    TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);
        //    Task<RpcResult<TransactionBlockResponse>> result_task = toolbox.Client.SignAndExecuteTransactionBlock(tx, toolbox.Account, options);
        //    yield return new WaitUntil(() => result_task.IsCompleted);

        //    //TransactionBlockResponse result = result_task.Result.Result;

        //    //Task wait_task = toolbox.Client.WaitForTransaction(local_digest);
        //    //yield return new WaitUntil(() => wait_task.IsCompleted);

        //    //Assert.AreEqual(local_digest, result.Digest);
        //    //Assert.IsTrue(result.Effects.Status.Status == ExecutionStatus.Success);
        //}

        [UnityTest]
        public IEnumerator SimplePublishBuildTest()
        {
            TestToolbox toolbox = new TestToolbox();
            yield return toolbox.Setup();

            Task task = toolbox.PublishPackage("serializer");
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }
}
