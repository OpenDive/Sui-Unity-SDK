using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Accounts;
using Sui.Rpc.Client;
using System.Collections.Generic;
using OpenDive.BCS;
using System.Linq;
using Sui.Utilities;
using Sui.Types;
using Sui.Transactions;

namespace Sui.Tests
{
    public class GovernanceTest
    {
        TestToolbox Toolbox;
        int DefaultStakeAmount = 1_000_000_000;
        string StateObjectID = Utils.NormalizeSuiAddress("0x5");

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        private async Task<RpcResult<TransactionBlockResponse>> AddStake(SuiClient client, Account account)
        {
            RpcResult<CoinPage> coins_task = await client.GetCoinsAsync(account, new SuiStructTag("0x2::sui::SUI"));

            RpcResult<SuiSystemSummary> info_task = await client.GetLatestSuiSystemStateAsync();

            AccountAddress active_validator = info_task.Result.ActiveValidators[0].SuiAddress;
            TransactionBlock tx_block = new TransactionBlock();
            List<TransactionArgument> coins_tx = tx_block.AddSplitCoinsTx(tx_block.gas, new TransactionArgument[]
            {
                tx_block.AddPure(new U64((ulong)this.DefaultStakeAmount))
            });

            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"0x3::sui_system::request_add_stake"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.StateObjectID),
                        coins_tx[0],
                        tx_block.AddPure(active_validator)
                }
            );

            RpcResult<IEnumerable<ObjectDataResponse>> coin_objects_task = await client.MultiGetObjectsAsync
            (
                coins_task.Result.Data.Select((coin) => coin.CoinObjectID).ToList(),
                new ObjectDataOptions(show_owner: true)
            );

            tx_block.SetGasPayment(coin_objects_task.Result.Select((obj) => obj.GetObjectReference()).ToArray());

            return await client.SignAndExecuteTransactionBlockAsync(tx_block, account, new TransactionBlockResponseOptions(showEffects: true));
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

        [UnityTest]
        public IEnumerator DelegatedStakesTest()
        {
            yield return this.Toolbox.Setup();

            Task<RpcResult<TransactionBlockResponse>> stake_task = this.AddStake(this.Toolbox.Client, this.Toolbox.Account);
            yield return new WaitUntil(() => stake_task.IsCompleted);

            if (stake_task.Result.Error != null)
                Assert.Fail(stake_task.Result.Error.Message);

            Task<RpcResult<IEnumerable<DelegatedStake>>> stake_fetch_task = this.Toolbox.Client.GetStakesAsync(this.Toolbox.Account);
            yield return new WaitUntil(() => stake_fetch_task.IsCompleted);

            Task<RpcResult<IEnumerable<DelegatedStake>>> stakes_by_id_task = this.Toolbox.Client.GetStakesByIdsAsync
            (
                new List<AccountAddress>()
                {
                    stake_fetch_task.Result.Result.ToList()[0].Stakes[0].Stake.StakedSuiID
                }
            );
            yield return new WaitUntil(() => stakes_by_id_task.IsCompleted);

            Assert.Greater(stake_fetch_task.Result.Result.ToList().Count, 0);
            Assert.IsTrue(stakes_by_id_task.Result.Result.ToList()[0].Stakes[0].Equals(stake_fetch_task.Result.Result.ToList()[0].Stakes[0]));
        }

        [UnityTest]
        public IEnumerator ValidatorFunctionFetchTest()
        {
            Task<RpcResult<CommitteeInfo>> committee_info_task = this.Toolbox.Client.GetCommitteeInfoAsync(0);
            yield return new WaitUntil(() => committee_info_task.IsCompleted);

            Assert.Greater(committee_info_task.Result.Result.Validators.Count(), 0);
        }
    }
}
