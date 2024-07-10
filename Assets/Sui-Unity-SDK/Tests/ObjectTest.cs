using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Linq;

namespace Sui.Tests
{
    public class ObjectTest
    {
        TestToolbox Toolbox;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator OwnedObjectFetchTest()
        {
            Task<RpcResult<PaginatedObjectsResponse>> gas_object_task = this.Toolbox.Client.GetOwnedObjects(this.Toolbox.Address());
            yield return new WaitUntil(() => gas_object_task.IsCompleted);
            Assert.Greater(gas_object_task.Result.Result.Data.Length, 0);
        }

        [UnityTest]
        public IEnumerator GetObjectFetchTest()
        {
            Task<RpcResult<CoinPage>> gas_object_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => gas_object_task.IsCompleted);
            Assert.Greater(gas_object_task.Result.Result.Data.Count, 0);

            foreach (CoinDetails gas_coin in gas_object_task.Result.Result.Data)
            {
                ObjectData details = gas_coin.ToSuiObjectData();

                Task<RpcResult<ObjectDataResponse>> coin_object_task = this.Toolbox.Client.GetObject
                (
                    details.ObjectId,
                    new ObjectDataOptions(show_type: true)
                );
                yield return new WaitUntil(() => coin_object_task.IsCompleted);

                Assert.IsTrue(coin_object_task.Result.Result.Data.Type == "0x2::coin::Coin<0x2::sui::SUI>");
            }
        }

        [UnityTest]
        public IEnumerator GetObjectsFetchTest()
        {
            Task<RpcResult<CoinPage>> gas_object_task = this.Toolbox.GetCoins();
            yield return new WaitUntil(() => gas_object_task.IsCompleted);
            Assert.Greater(gas_object_task.Result.Result.Data.Count, 0);

            string[] gas_object_ids = gas_object_task.Result.Result.Data.Select((obj) => obj.CoinObjectId).ToArray();
            Task<RpcResult<System.Collections.Generic.IEnumerable<ObjectDataResponse>>> object_infos_task = this.Toolbox.Client.MultiGetObjects
            (
                gas_object_ids,
                new ObjectDataOptions(show_type: true)
            );
            yield return new WaitUntil(() => object_infos_task.IsCompleted);

            Assert.IsTrue
            (
                gas_object_task.Result.Result.Data.Count ==
                object_infos_task.Result.Result.Count()
            );

            foreach (ObjectDataResponse obj in object_infos_task.Result.Result)
                Assert.IsTrue(obj.Data.Type == "0x2::coin::Coin<0x2::sui::SUI>");
        }

        [UnityTest]
        public IEnumerator ObjectNotExistingHandleTest()
        {
            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObject(NormalizedTypeConverter.NormalizeSuiAddress("0x9999"), 0);
            yield return new WaitUntil(() => past_object_task.IsCompleted);
            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.ObjectNotExists);
        }

        [UnityTest]
        public IEnumerator OldObjectHandleTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoins(this.Toolbox.Address(), "0x2::sui::SUI");
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObject
            (
                coin_data_task.Result.Result.Data[0].CoinObjectId,
                int.Parse(coin_data_task.Result.Result.Data[0].Version ??= "0")
            );
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.VersionFound);
        }

        [UnityTest]
        public IEnumerator VersionTooHighHandleTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoins(this.Toolbox.Address(), "0x2::sui::SUI");
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObject
            (
                coin_data_task.Result.Result.Data[0].CoinObjectId,
                int.Parse(coin_data_task.Result.Result.Data[0].Version ??= "0") + 1
            );
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.VersionTooHigh);
        }

        [UnityTest]
        public IEnumerator VersionNotFoundHandleTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoins(this.Toolbox.Address(), "0x2::sui::SUI");
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            // NOTE: This works because we know that this is a fresh coin that hasn't been modified:
            Task<RpcResult<ObjectRead>> past_object_task = this.Toolbox.Client.TryGetPastObject
            (
                coin_data_task.Result.Result.Data[0].CoinObjectId,
                int.Parse(coin_data_task.Result.Result.Data[0].Version ??= "0") - 1
            );
            yield return new WaitUntil(() => past_object_task.IsCompleted);

            Assert.IsTrue(past_object_task.Result.Result.Type == ObjectReadType.VersionNotFound);
        }

        [UnityTest]
        public IEnumerator OldVersionFindTest()
        {
            Task<RpcResult<CoinPage>> coin_data_task = this.Toolbox.Client.GetCoins(this.Toolbox.Address(), "0x2::sui::SUI");
            yield return new WaitUntil(() => coin_data_task.IsCompleted);

            Transactions.TransactionBlock tx_block = new Transactions.TransactionBlock();
            tx_block.AddTransferObjectsTx
            (
                new Transactions.Types.Arguments.SuiTransactionArgument[] { tx_block.gas },
                NormalizedTypeConverter.NormalizeSuiAddress("0x2")
            );

            Task<RpcResult<TransactionBlockResponse>> tx_block_sign_task = this.Toolbox.Client.SignAndExecuteTransactionBlock(tx_block, this.Toolbox.Account);
            yield return new WaitUntil(() => tx_block_sign_task.IsCompleted);

            // NOTE: This works because we know that this is a fresh coin that hasn't been modified:
            Task<RpcResult<ObjectRead>> result_task = this.Toolbox.Client.TryGetPastObject
            (
                coin_data_task.Result.Result.Data[0].CoinObjectId,
                int.Parse(coin_data_task.Result.Result.Data[0].Version ??= "0")
            );
            yield return new WaitUntil(() => result_task.IsCompleted);

            Assert.IsTrue(result_task.Result.Result.Type == ObjectReadType.VersionFound);
        }
    }
}
