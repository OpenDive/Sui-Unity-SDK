//
//  TransactionsTest.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using NUnit.Framework;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Data;
using Sui.Utilities;
using Sui.Types;
using System.Collections.Generic;
using Sui.Transactions;

namespace Sui.Tests
{
    public class TransactionsTest
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

            SuiObjectRef paymentRef = new SuiObjectRef(AccountAddress.FromHex(objectId), version, digest);
            CallArg[] inputs = new CallArg[] { new CallArg(CallArgumentType.Object, new ObjectCallArg(new ObjectArg(ObjectRefType.ImmOrOwned, paymentRef))) };

            MoveCall moveCallTransaction = new MoveCall(
                new SuiMoveNormalizedStructType(suiAddress, "display", "new", new List<Rpc.Models.SuiMoveNormalizedType>()),
                new SerializableTypeTag[] { new SerializableTypeTag(new SuiStructTag(suiAddress, "capy", "Capy", new List<SerializableTypeTag>())) },
                new TransactionArgument[] { new TransactionArgument(TransactionArgumentKind.Input, new TransactionBlockInput(0)) }
            );

            List<Command> transactions = new List<Command> { new Command(CommandKind.MoveCall, moveCallTransaction) };

            TransactionData transactionData = new TransactionData
            (
                TransactionType.V1,
                new TransactionDataV1
                (
                    AccountAddress.FromHex(test),
                    new TransactionExpiration(),
                    new GasData(
                        "1000000",
                        "1",
                        new SuiObjectRef[] { paymentRef },
                        suiAddress
                    ),
                    new TransactionKind
                    (
                        TransactionKindType.ProgrammableTransaction, new ProgrammableTransaction
                        (
                            inputs,
                            transactions.ToArray()
                        )
                    )
                )
            );

            Serialization serializer = new Serialization();
            transactionData.Serialize(serializer);
            byte[] actual = serializer.GetBytes();
            byte[] expected = new byte[] { 0, 0, 1, 1, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 173, 1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 0 };

            Assert.AreEqual(expected, actual,
                "ACTUAL LENGHT: " + actual.Length + "\n"
                + "EXPECTED LENGTH: " + expected.Length + "\n" + actual.ToReadableString());
        }

        [Test]
        public void TransactionDataSerialization()
        {
            string test = "0x0000000000000000000000000000000000000000000000000000000000000BAD";

            AccountAddress sender = AccountAddress.FromHex(test);
            TransactionExpiration expiration = new TransactionExpiration();

            SuiObjectRef paymentRef = new SuiObjectRef(
                AccountAddress.FromHex("0x1000000000000000000000000000000000000000000000000000000000000000"),
                int.Parse("10000"),
                "1Bhh3pU9gLXZhoVxkr5wyg9sX6"
            );

            string sui = "0x0000000000000000000000000000000000000000000000000000000000000002";
            AccountAddress suiAddress = AccountAddress.FromHex(sui);

            GasData gasData = new GasData(
                "1000000",
                "1",
                new SuiObjectRef[] { paymentRef },
                suiAddress
            );

            CallArg[] inputs = new CallArg[] { new CallArg(CallArgumentType.Object, new ObjectCallArg(new ObjectArg(ObjectRefType.ImmOrOwned, paymentRef))) };

            MoveCall moveCallTransaction = new MoveCall(
                new SuiMoveNormalizedStructType(suiAddress, "display", "new", new List<Rpc.Models.SuiMoveNormalizedType>()), // TODO: THIS IS A NORMALIZED STRUCT
                new SerializableTypeTag[] { new SerializableTypeTag(new SuiStructTag(suiAddress, "capy", "Capy", new List<SerializableTypeTag>())) },
                new TransactionArgument[]
                {
                    new TransactionArgument(TransactionArgumentKind.Input, new TransactionBlockInput(0)),
                    new TransactionArgument(TransactionArgumentKind.Input, new TransactionBlockInput(1)),
                    new TransactionArgument(TransactionArgumentKind.Result, new Result(2))
                }
            );

            List<Command> transactions = new List<Command> { new Command(CommandKind.MoveCall, moveCallTransaction) };

            TransactionDataV1 transactionDataV1 = new TransactionDataV1(
                sender,
                expiration,
                gasData,
                new TransactionKind(TransactionKindType.ProgrammableTransaction, new ProgrammableTransaction(
                    inputs,
                    transactions.ToArray()
                ))
            );

            TransactionData transactionData = new TransactionData(TransactionType.V1, transactionDataV1);

            Serialization serializer = new Serialization();
            transactionData.Serialize(serializer);

            byte[] actual = serializer.GetBytes();
            byte[] expected = new byte[] { 0, 0, 1, 1, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 3, 1, 0, 0, 1, 1, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 173, 1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 39, 0, 0, 0, 0, 0, 0, 20, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 0 };

            Assert.AreEqual(expected, actual,
                "ACTUAL LENGHT: " + actual.Length + "\n"
                + "EXPECTED LENGTH: " + expected.Length + "\n" + actual.ToReadableString());
        }
    }
}
