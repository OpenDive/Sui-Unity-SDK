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
using UnityEditor.VersionControl;

namespace Sui.Tests
{
    public class TXBuilderTest: MonoBehaviour
    {
        TestToolbox Toolbox;
        string PackageID;
        string SharedObjectID;
        string SuiClockObjectID = NormalizedTypeConverter.NormalizeSuiAddress("0x6");

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("serializer");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;

            List<SuiOwnedObjectRef> created_objects = task.Result.PublishedTX.Effects.Created;
            List<SuiOwnedObjectRef> shared_object = created_objects.Where(obj => obj.Owner.Type == SuiOwnerType.Shared).ToList();

            this.SharedObjectID = shared_object[0].Reference.ObjectId;
        }

        private IEnumerator ValidateTransaction
        (
            SuiClient client,
            Account account,
            Transactions.TransactionBlock tx_block
        )
        {
            tx_block.SetSenderIfNotSet(account.SuiAddress());

            Task<RpcResult<string>> local_digest_task = tx_block.GetDigest(new BuildOptions(client));
            yield return new WaitUntil(() => local_digest_task.IsCompleted);

            TransactionBlockResponseOptions options = new TransactionBlockResponseOptions(showEffects: true);

            Task<RpcResult<TransactionBlockResponse>> result_task = client.SignAndExecuteTransactionBlock
            (
                tx_block,
                account,
                options
            );
            yield return new WaitUntil(() => result_task.IsCompleted);

            Assert.IsTrue(local_digest_task.Result.Result == result_task.Result.Result.Digest);

            if (result_task.Result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                Assert.Fail("Transaction Failed");
        }

        [UnityTest]
        public IEnumerator SplitCoinAndTransferTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);

            CoinDetails coin_0 = coins_task.Result.Result.Data[0];
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();

            List<SuiTransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                tx_block.AddObjectInput(coin_0.CoinObjectId),
                new SuiTransactionArgument[] { tx_block.AddPure(new U64((ulong)(this.Toolbox.DefaultGasBudget * 2))) }
            );
            tx_block.AddTransferObjectsTx(coin.ToArray(), this.Toolbox.Address());

            yield return this.ValidateTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block
            );
        }
    }
}
