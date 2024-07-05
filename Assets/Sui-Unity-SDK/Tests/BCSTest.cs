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

namespace Sui.Tests
{
    public class BCSTest : MonoBehaviour
    {
        // Oooh-weeee we nailed it!
        [Test]
        public void SimpleProgrammingTransactionsTest()
        {
            string sui = "0x0000000000000000000000000000000000000000000000000000000000000002";
            AccountAddress suiAddress = AccountAddress.FromHex(sui);

            string capy = "0x0000000000000000000000000000000000000000000000000000000000000006";
            AccountAddress capyAddress = AccountAddress.FromHex(capy);

            MoveCall moveCallTransaction = new MoveCall(
                new SuiMoveNormalizedStructType(new SuiStructTag(suiAddress, "display", "new", new SerializableTypeTag[0]), new Rpc.Models.SuiMoveNormalizedType[] { }), // TODO: THIS IS A NORMALIZED STRUCT
                new SerializableTypeTag[] { new SerializableTypeTag(new SuiStructTag(capyAddress, "capy", "Capy", new SerializableTypeTag[0])) },
                new SuiTransactionArgument[]
                {
                    new SuiTransactionArgument(new GasCoin()),
                    new SuiTransactionArgument(new NestedResult(0, 1)),
                    new SuiTransactionArgument(new TransactionBlockInput(3)),
                    new SuiTransactionArgument(new Result(1))
                }
            );

            Serialization serializer = new Serialization();
            moveCallTransaction.Serialize(serializer);
            byte[] actual = serializer.GetBytes();
            byte[] expected = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 4, 0, 3, 0, 0, 1, 0, 1, 3, 0, 2, 1, 0 };

            Assert.AreEqual(expected, actual,
                "ACTUAL LENGHT: " + actual.Length + "\n"
                + "EXPECTED LENGTH: " + expected.Length + "\n" + actual.ByteArrayToString());
        }
    }
}