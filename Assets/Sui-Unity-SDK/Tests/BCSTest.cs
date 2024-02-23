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
        public void SimpleProgrammingTransactionBlock()
        {
            var target = new SuiMoveNormalizedStructType("0x2::display::new");
            ISerializableTag[] typeArguments = { new SuiStructTag("0x6::capy::Capy") };
            SuiTransactionArgument[] arguments = {
                new SuiTransactionArgument(new GasCoin()),
                new SuiTransactionArgument(new NestedResult(0, 1)),
                new SuiTransactionArgument(new TransactionBlockInput(3, null)),
                new SuiTransactionArgument(new TransactionResult(1))
            };

            MoveCall moveCall = new MoveCall(
                target,
                typeArguments,
                arguments
            );

            Serialization ser = new Serialization();
            byte[] moveCallBytes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 4, 0, 3, 0, 0, 1, 0, 1, 3, 0, 2, 1, 0 };
            moveCall.Serialize(ser);

            Assert.AreEqual(moveCallBytes, ser.GetBytes());
        }
    }
}