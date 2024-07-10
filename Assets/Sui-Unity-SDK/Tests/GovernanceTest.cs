﻿using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Transactions.Types.Arguments;
using Sui.Transactions;
using System.Collections.Generic;
using OpenDive.BCS;
using System.Linq;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;

namespace Sui.Tests
{
    public class GovernanceTest
    {
        TestToolbox Toolbox;
        int DefaultStakeAmount = 1_000_000_000;
        string StateObjectID = NormalizedTypeConverter.NormalizeSuiAddress("0x5");

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        private async Task<RpcResult<TransactionBlockResponse>> AddStake(SuiClient client, Account account)
        {
            RpcResult<CoinPage> coins_task = await client.GetCoins(account.SuiAddress(), "0x2::sui::SUI");

            RpcResult<SuiSystemSummary> info_task = await client.GetLatestSuiSystemState();

            string active_validator = info_task.Result.ActiveValidators[0].SuiAddress;
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> coins_tx = tx_block.AddSplitCoinsTx(tx_block.gas, new SuiTransactionArgument[]
            {
                new SuiTransactionArgument(tx_block.AddPure(new U64((ulong)this.DefaultStakeAmount)))
            });

            var ser = new Serialization();
            coins_tx[0].Serialize(ser);
            Debug.Log($"MARCUS::: COIN TX SER - {string.Join(", ", ser.GetBytes())}");

            tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"0x3::sui_system::request_add_stake"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                new SuiTransactionArgument[]
                {
                        new SuiTransactionArgument(tx_block.AddObjectInput(this.StateObjectID)),
                        coins_tx[0],
                        new SuiTransactionArgument(tx_block.AddPure(AccountAddress.FromHex(active_validator)))
                }
            );

            RpcResult<IEnumerable<ObjectDataResponse>> coin_objects_task = await client.MultiGetObjects
            (
                coins_task.Result.Data.Select((coin) => AccountAddress.FromHex(coin.CoinObjectId)).ToArray(),
                new ObjectDataOptions(show_owner: true)
            );

            tx_block.SetGasPayment(coin_objects_task.Result.Select((obj) => obj.GetObjectReference()).ToArray());

            return await client.SignAndExecuteTransactionBlock(tx_block, account, new TransactionBlockResponseOptions(showEffects: true));
        }

        [UnityTest]
        public IEnumerator RequestAddStakeTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<TransactionBlockResponse>> stake_task = this.AddStake(this.Toolbox.Client, this.Toolbox.Account);
            yield return new WaitUntil(() => stake_task.IsCompleted);

            if (stake_task.Result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                Assert.Fail("Transaction Failed");
        }
    }
}
