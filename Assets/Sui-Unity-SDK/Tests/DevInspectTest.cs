using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Transactions.Types.Arguments;
using System.Collections.Generic;
using OpenDive.BCS;
using Newtonsoft.Json;

namespace Sui.Tests
{
    public class DevInspectTest
    {
        TestToolbox Toolbox;
        string PackageID;

        private IEnumerator ValidateDevInspectTransaction
        (
            SuiClient client,
            Account signer,
            Transactions.TransactionBlock transaction_block,
            ExecutionStatus status
        )
        {
            Task<RpcResult<DevInspectResponse>> result_task = client.DevInspectTransactionBlock(signer, transaction_block);
            yield return new WaitUntil(() => result_task.IsCompleted);

            if (status != result_task.Result.Result.Effects.Status.Status)
                Assert.Fail("Status does not match");
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("serializer");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;
        }

        [UnityTest]
        public IEnumerator DevInspectSplitAndTransferTest()
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                new SuiTransactionArgument(new GasCoin()),
                new SuiTransactionArgument[] { new SuiTransactionArgument(tx_block.AddPure(new U64(10))) }
            );
            tx_block.AddTransferObjectsTx(coin.ToArray(), this.Toolbox.Account.SuiAddress());
            yield return ValidateDevInspectTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block,
                ExecutionStatus.Success
            );
        }

        [UnityTest]
        public IEnumerator DevInspectMoveCallTest()
        {
            Task<RpcResult<CoinPage>> coins_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coins_task.IsCompleted);
            CoinPage coins = coins_task.Result.Result;

            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            CoinDetails coin_0 = coins.Data[0];
            List<SuiTransactionArgument> obj = tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"{this.PackageID}::serializer_tests::return_struct"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { new SerializableTypeTag(SuiStructTag.FromStr("0x2::coin::Coin<0x2::sui::SUI>")) },
                new SuiTransactionArgument[]
                {
                        new SuiTransactionArgument(tx_block.AddPure(new BString(coin_0.CoinObjectId)))
                }
            );
            tx_block.AddTransferObjectsTx(obj.ToArray(), this.Toolbox.Account.SuiAddress());

            yield return ValidateDevInspectTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block,
                ExecutionStatus.Success
            );
        }

        [UnityTest]
        public IEnumerator MoveCallFailedDevInspectTest()
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"{this.PackageID}::serializer_tests::test_abort"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                new SuiTransactionArgument[] { }
            );

            yield return ValidateDevInspectTransaction
            (
                this.Toolbox.Client,
                this.Toolbox.Account,
                tx_block,
                ExecutionStatus.Failure
            );
        }
    }
}
