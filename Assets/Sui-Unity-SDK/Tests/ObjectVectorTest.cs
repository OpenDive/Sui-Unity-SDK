using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using NBitcoin;
using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;
using UnityEditor.VersionControl;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Newtonsoft.Json;
using UnityEditor.UIElements;

namespace Sui.Tests
{
    public class ObjectVectorTest
    {
        TestToolbox Toolbox;
        string PackageID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            Task<PublishedPackage> task = this.Toolbox.PublishPackage("entry-point-vector");
            yield return new WaitUntil(() => task.IsCompleted);

            this.PackageID = task.Result.PackageID;

            yield return this.Toolbox.Setup();
        }

        private void CheckBlockResponse(RpcResult<TransactionBlockResponse> tx_sign)
        {
            if (tx_sign.Error != null || tx_sign.Result == null || (tx_sign.Result != null && tx_sign.Result.Effects.Status.Status == ExecutionStatus.Failure))
                Assert.Fail($"Transaction Failed with message - {tx_sign.Error.Message ??= tx_sign.Result.Effects.Status.Error}");
        }

        private async Task<string> MintObject(int val, TestToolbox toolbox)
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"{this.PackageID}::entry_point_vector::mint"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                new SuiTransactionArgument[]
                {
                    new SuiTransactionArgument(tx_block.AddPure(new U64((ulong)val)))
                }
            );

            RpcResult<TransactionBlockResponse> tx_sign = await toolbox.Client.SignAndExecuteTransactionBlock
            (
                tx_block,
                toolbox.Account,
                new TransactionBlockResponseOptions(showEffects: true)
            );

            this.CheckBlockResponse(tx_sign);

            return tx_sign.Result.Effects.Created[0].Reference.ObjectId;
        }

        private IEnumerator DestroyObject(string[] objects, TestToolbox toolbox, bool with_type = false)
        {
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> vec = tx_block.AddMakeMoveVecTx
            (
                objects.Select((obj) => new SuiTransactionArgument(tx_block.AddObjectInput(obj))).ToArray(),
                with_type ? SuiStructTag.FromStr($"{this.PackageID}::entry_point_vector::Obj") : null
            );
            tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr($"{this.PackageID}::entry_point_vector::two_obj_vec_destroy"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { },
                vec.ToArray()
            );

            Task<RpcResult<TransactionBlockResponse>> tx_sign_task = toolbox.Client.SignAndExecuteTransactionBlock
            (
                tx_block,
                toolbox.Account,
                new TransactionBlockResponseOptions(showEffects: true)
            );
            yield return new WaitUntil(() => tx_sign_task.IsCompleted);

            this.CheckBlockResponse(tx_sign_task.Result);
        }
        
        [UnityTest]
        public IEnumerator VectorObjectsInitializationTest()
        {
            Task<string> res_1 = this.MintObject(7, this.Toolbox);
            yield return new WaitUntil(() => res_1.IsCompleted);

            Task<string> res_2 = this.MintObject(42, this.Toolbox);
            yield return new WaitUntil(() => res_2.IsCompleted);

            yield return this.DestroyObject(new string[] { res_1.Result, res_2.Result }, this.Toolbox);
        }

        // TODO: Figure out and implement a solution for this flaky test.
        [UnityTest]
        [Retry(10)]
        public IEnumerator TypeHintTest()
        {
            Task<string> res_1 = this.MintObject(7, this.Toolbox);
            yield return new WaitUntil(() => res_1.IsCompleted);

            Task<string> res_2 = this.MintObject(42, this.Toolbox);
            yield return new WaitUntil(() => res_2.IsCompleted);

            yield return this.DestroyObject(new string[] { res_1.Result, res_2.Result }, this.Toolbox, true);
        }

        [UnityTest]
        public IEnumerator MixedArgumentTest()
        {
            Task<RpcResult<CoinPage>> coin_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => coin_task.IsCompleted);

            CoinDetails coin = coin_task.Result.Result.Data[3];
            string[] coin_ids = coin_task.Result.Result.Data.Select((coin) => coin.CoinObjectId).ToArray();
            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            List<SuiTransactionArgument> vec = tx_block.AddMakeMoveVecTx
            (
                new SuiTransactionArgument[]
                {
                    new SuiTransactionArgument(tx_block.AddObjectInput(coin_ids[1])),
                    new SuiTransactionArgument(tx_block.AddObjectInput(coin_ids[2]))
                }
            );
            tx_block.AddMoveCallTx
            (
                new SuiMoveNormalizedStructType(SuiStructTag.FromStr("0x2::pay::join_vec"), new SuiMoveNormalizedType[] { }),
                new SerializableTypeTag[] { new SerializableTypeTag(SuiStructTag.FromStr("0x2::sui::SUI")) },
                new SuiTransactionArgument[]
                {
                    new SuiTransactionArgument(tx_block.AddObjectInput(coin_ids[0])),
                    vec[0]
                }
            );
            tx_block.SetGasPayment(new Types.SuiObjectRef[] { coin.ToSuiObjectRef() });

            Task<RpcResult<TransactionBlockResponse>> tx_sign_task = this.Toolbox.Client.SignAndExecuteTransactionBlock
            (
                tx_block,
                this.Toolbox.Account,
                new TransactionBlockResponseOptions(showEffects: true)
            );
            yield return new WaitUntil(() => tx_sign_task.IsCompleted);

            this.CheckBlockResponse(tx_sign_task.Result);
        }
    }
}