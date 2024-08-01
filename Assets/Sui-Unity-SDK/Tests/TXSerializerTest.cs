using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc.Models;
using System.Collections.Generic;
using System.Linq;
using Sui.Types;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions;

namespace Sui.Tests
{
    public class TXSerializerTest
    {
        TestToolbox Toolbox;
        string PackageID;
        AccountAddress SharedObjectID;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            yield return this.Toolbox.PublishPackage("serializer", (package_result) => {
                if (package_result.Error != null)
                    Assert.Fail(package_result.Error.Message);

                this.PackageID = package_result.Result.PackageID;

                List<SuiOwnedObjectRef> created_objects = package_result.Result.PublishedTX.Effects.Created.ToList();
                List<SuiOwnedObjectRef> shared_object = created_objects.Where(obj => obj.Owner.Type == SuiOwnerType.Shared).ToList();

                this.SharedObjectID = shared_object[0].Reference.ObjectID;
            });
        }

        private IEnumerator SerializeAndDeserialize(TransactionBlock tx_block, List<bool> mutable)
        {
            tx_block.SetSender(this.Toolbox.Account);

            Task<byte[]> tx_build_task = tx_block.Build(new BuildOptions(this.Toolbox.Client));
            yield return new WaitUntil(() => tx_build_task.IsCompleted);

            if (tx_block.Error != null)
                Assert.Fail(tx_block.Error.Message);

            byte[] tx_build_bytes = tx_build_task.Result;
            TransactionBlockDataBuilderSerializer deserialized_txn_builder =
                new TransactionBlockDataBuilderSerializer(tx_build_bytes);

            List<bool> mutable_compare = deserialized_txn_builder.Builder.Inputs.Where
            (input => {
                if (input.Value.GetType() == typeof(CallArg))
                    return ((CallArg)input.Value).GetSharedObectInput() != null;
                else
                    return false;
            }).ToList().Select((shared_input) => {
                if (shared_input.Value.GetType() == typeof(CallArg))
                    return ((CallArg)shared_input.Value).IsMutableSharedObjectInput();
                else
                    return false;
            }).ToList();

            Assert.IsTrue(mutable_compare.SequenceEqual(mutable));

            TransactionBlock reserialized_tx_block = new TransactionBlock(deserialized_txn_builder);

            Task<byte[]> tx_rebuild_task = reserialized_tx_block.Build(new BuildOptions(this.Toolbox.Client));
            yield return new WaitUntil(() => tx_rebuild_task.IsCompleted);

            if (reserialized_tx_block.Error != null)
                Assert.Fail(reserialized_tx_block.Error.Message);

            Assert.IsTrue(tx_rebuild_task.Result.SequenceEqual(tx_build_bytes));
        }

        [UnityTest]
        public IEnumerator SerAndDerMoveSharedObjectWithImmutableReferenceTest()
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::value"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.SharedObjectID.KeyHex)
                }
            );
            yield return this.SerializeAndDeserialize(tx_block, new List<bool> { false });
        }

        [UnityTest]
        public IEnumerator SerAndDerMoveSharedObjectWithImmutableAndMutableReferencesTest()
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::value"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.SharedObjectID.KeyHex)
                }
            );
            tx_block.AddMoveCallTx
            (
                SuiMoveNormalizedStructType.FromStr($"{this.PackageID}::serializer_tests::set_value"),
                new SerializableTypeTag[] { },
                new TransactionArgument[]
                {
                        tx_block.AddObjectInput(this.SharedObjectID.KeyHex)
                }
            );
            yield return this.SerializeAndDeserialize(tx_block, new List<bool> { true });
        }

        [UnityTest]
        public IEnumerator TransactionExpirationSerAndDerTest()
        {
            TransactionBlock tx_block = new TransactionBlock();
            tx_block.SetExpiration(new TransactionExpiration(100));
            yield return this.SerializeAndDeserialize(tx_block, new List<bool> { });
        }
    }
}

