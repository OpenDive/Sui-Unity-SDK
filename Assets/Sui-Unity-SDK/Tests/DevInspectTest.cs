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
using Sui.Rpc.Client;
using Sui.Utilities;

namespace Sui.Tests
{
    public class DevInspectTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("serializer", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;
            });
        }

        private IEnumerator ValidateDevInspectTransaction
        (
            SuiClient client,
            Account signer,
            Transactions.TransactionBlock transaction_block,
            ExecutionStatus status
        )
        {
            Task<RpcResult<DevInspectResponse>> result_task = client.DevInspectTransactionBlockAsync(signer, transaction_block);
            yield return new WaitUntil(() => result_task.IsCompleted);

            if (result_task.Result.Result == null || status != result_task.Result.Result.Effects.Status.Status)
                Assert.Fail("Status does not match");
        }

        [UnityTest]
        public IEnumerator DevInspectSplitAndTransferTest()
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> coin = tx_block.AddSplitCoinsTx
            (
                tx_block.gas,
                new SuiTransactionArgument[] { tx_block.AddPure(new U64(10)) }
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
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::return_struct"),
                new SerializableTypeTag[] { new SerializableTypeTag(SuiStructTag.FromStr("0x2::coin::Coin<0x2::sui::SUI>")) },
                new SuiTransactionArgument[]
                {
                        tx_block.AddObjectInput(coin_0.CoinObjectID.KeyHex)
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
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::test_abort"),
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
