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
using System.Collections.Generic;

namespace Sui.Tests
{
    public class TransactionsTest : MonoBehaviour
    {
        string test             = "0x0000000000000000000000000000000000000000000000000000000000000BAD";
        string objectId         = "0x1000000000000000000000000000000000000000000000000000000000000000";
        int version             = int.Parse("10000");
        string digest           = "1Bhh3pU9gLXZhoVxkr5wyg9sX6";
        string suiAddressHex    = "0x0000000000000000000000000000000000000000000000000000000000000002";

        [Test]
        public void TransactionDataSerializationSingleInput()
        {
            AccountAddress suiAddress = AccountAddress.FromHex(suiAddressHex);

            // ////////////////////////////////////////
            // Programmable Transaction Block -- Inputs
            SuiObjectRef paymentRef = new SuiObjectRef(AccountAddress.FromHex(objectId), version, digest);
            ICallArg[] inputs = new ICallArg[] { new ObjectCallArg(new ObjectArg(ObjectRefType.ImmOrOwned, paymentRef)) };

            MoveCall moveCallTransaction = new MoveCall(
                new SuiMoveNormalizedStructType(new SuiStructTag(suiAddress, "display", "new", new ISerializableTag[0]), new Rpc.Models.SuiMoveNormalizedType[] { }), // TODO: THIS IS A NORMALIZED STRUCT
                new ISerializableTag[] { new StructTag(suiAddress, "capy", "Capy", new ISerializableTag[0]) },
                new SuiTransactionArgument[] { new SuiTransactionArgument(new TransactionBlockInput(0)) } // TODO: We should not use this abstract, this should be a "pure" or an "object.
            );

            List<Sui.Transactions.Types.SuiTransaction> transactions = new List<Sui.Transactions.Types.SuiTransaction> { new SuiTransaction(moveCallTransaction) };

            ////////////////////////////////////////
            //Programmable Transaction Block--  Transactions
            // This is createdi in "build"
            ////////////////////////////////////////
            Transactions.Builder.TransactionDataV1 transactionData = new(
                AccountAddress.FromHex(test),
                new TransactionExpirationNone(),
                new GasConfig(
                    "1000000",
                    "1",
                    new SuiObjectRef[] { paymentRef },
                    suiAddress
                ),
                new ProgrammableTransaction(
                    inputs,
                    transactions
                )
            );

            Serialization serializer = new Serialization();
            transactionData.Serialize(serializer);
            byte[] actual = serializer.GetBytes();
            byte[] expected = new byte[] { 0, 0, 1, 1, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 173, 1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 0 };

            Assert.AreEqual(expected, actual,
                "ACTUAL LENGHT: " + actual.Length + "\n"
                + "EXPECTED LENGTH: " + expected.Length + "\n" + actual.ByteArrayToString());
        }

        [Test]
        public void TransactionDataSerialization()
        {
            string test = "0x0000000000000000000000000000000000000000000000000000000000000BAD";

            AccountAddress sender = AccountAddress.FromHex(test);
            ITransactionExpiration expiration = new TransactionExpirationNone();

            SuiObjectRef paymentRef = new SuiObjectRef(
                AccountAddress.FromHex("0x1000000000000000000000000000000000000000000000000000000000000000"),
                int.Parse("10000"),
                "1Bhh3pU9gLXZhoVxkr5wyg9sX6"
            );

            string sui = "0x0000000000000000000000000000000000000000000000000000000000000002";
            AccountAddress suiAddress = AccountAddress.FromHex(sui);

            GasConfig gasData = new GasConfig(
                "1000000",
                "1",
                new SuiObjectRef[] { paymentRef },
                suiAddress
            );

            ICallArg[] inputs = new ICallArg[] { new ObjectCallArg(new ObjectArg(ObjectRefType.ImmOrOwned, paymentRef)) };

            MoveCall moveCallTransaction = new MoveCall(
                new SuiMoveNormalizedStructType(new SuiStructTag(suiAddress, "display", "new", new ISerializableTag[0]), new Rpc.Models.SuiMoveNormalizedType[] { }), // TODO: THIS IS A NORMALIZED STRUCT
                new ISerializableTag[] { new StructTag(suiAddress, "capy", "Capy", new ISerializableTag[0]) },
                new SuiTransactionArgument[]
                {
                    new SuiTransactionArgument(new TransactionBlockInput(0)),
                    new SuiTransactionArgument(new TransactionBlockInput(1)),
                    new SuiTransactionArgument(new Result(2))
                }
            );

            List<Sui.Transactions.Types.SuiTransaction> transactions = new List<Sui.Transactions.Types.SuiTransaction> { new SuiTransaction(moveCallTransaction) };

            ProgrammableTransaction programmableTransaction
                = new(inputs, transactions);

            Transactions.Builder.TransactionDataV1 transactionData = new(
                sender,
                expiration,
                gasData,
                programmableTransaction
            );

            Serialization serializer = new Serialization();
            transactionData.Serialize(serializer);

            byte[] actual = serializer.GetBytes();
            byte[] expected = new byte[] { 0, 0, 1, 1, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 3, 1, 0, 0, 1, 1, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 173, 1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 0 };

            Assert.AreEqual(expected, actual,
                "ACTUAL LENGHT: " + actual.Length + "\n"
                + "EXPECTED LENGTH: " + expected.Length + "\n" + actual.ByteArrayToString());
        }
    }
}
