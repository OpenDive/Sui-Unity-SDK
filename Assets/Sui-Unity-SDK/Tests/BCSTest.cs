using NUnit.Framework;
using OpenDive.BCS;
using Sui.Accounts;
using UnityEngine;
using Sui.Utilities;
using Sui.Types;
using System.Linq;
using System.Numerics;
using Sui.Transactions;

namespace Sui.Tests
{
    public class BCSTest
    {
        [Test]
        public void BoolTrueSerAndDerTest()
        {
            bool input = true;
            Serialization ser = new Serialization();
            ser.SerializeBool(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            bool output = der.DeserializeBool();

            Assert.IsTrue(input == output);
        }

        [Test]
        public void BoolFalseSerAndDerTest()
        {
            bool input = false;
            Serialization ser = new Serialization();
            ser.SerializeBool(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            bool output = der.DeserializeBool();

            Assert.IsTrue(input == output);
        }

        [Test]
        public void BoolErrorSerAndDerTest()
        {
            byte input = 32;
            Serialization ser = new Serialization();
            ser.SerializeU8(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            der.DeserializeBool();

            Assert.NotNull(der.Error);
        }

        [Test]
        public void ByteArraySerAndDerTest()
        {
            byte[] input = System.Text.Encoding.UTF8.GetBytes("1234567890");
            Serialization ser = new Serialization();
            ser.SerializeBytes(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            byte[] output = der.ToBytes();

            Assert.IsTrue(input.SequenceEqual(output));
        }

        [Test]
        public void SequenceSerAndDerTest()
        {
            Sequence input = new Sequence(new string[] { "a", "abc", "def", "ghi" }.ToList().Select(str => new BString(str)).ToArray());
            Serialization ser = new Serialization();
            input.Serialize(ser);

            Deserialization der = new Deserialization(ser.GetBytes());
            Sequence output = der.DeserializeSequence(typeof(BString));

            Assert.IsTrue(input.Equals(output));
        }

        [Test]
        public void StringSerAndDerTest()
        {
            string input = "1234567890";
            Serialization ser = new Serialization();
            ser.SerializeString(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            BString output = der.DeserializeString();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void ByteSerAndDerTest()
        {
            byte input = 15;
            Serialization ser = new Serialization();
            ser.SerializeU8(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            U8 output = der.DeserializeU8();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void UShortSerAndDerTest()
        {
            ushort input = 111_15;
            Serialization ser = new Serialization();
            ser.SerializeU16(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            U16 output = der.DeserializeU16();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void UIntSerAndDerTest()
        {
            uint input = 1_111_111_115;
            Serialization ser = new Serialization();
            ser.SerializeU32(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            U32 output = der.DeserializeU32();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void ULongSerAndDerTest()
        {
            ulong input = 1_111_111_111_111_111_115;
            Serialization ser = new Serialization();
            ser.SerializeU64(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            U64 output = der.DeserializeU64();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void UInt128BigIntegerSerAndDerTest()
        {
            BigInteger input = BigInteger.Parse("1111111111111111111111111111111111115");
            Serialization ser = new Serialization();
            ser.SerializeU128(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            U128 output = der.DeserializeU128();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void UInt256BigIntegerSerAndDerTest()
        {
            BigInteger input = BigInteger.Parse("111111111111111111111111111111111111111111111111111111111111111111111111111115");
            Serialization ser = new Serialization();
            ser.SerializeU256(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            U256 output = der.DeserializeU256();

            Assert.IsTrue(output.Equals(input));
        }

        [Test]
        public void ULeb128SerAndDerTest()
        {
            uint input = 1_111_111_115;
            Serialization ser = new Serialization();
            ser.SerializeU32AsUleb128(input);

            Deserialization der = new Deserialization(ser.GetBytes());
            int output = der.DeserializeUleb128();

            Assert.IsTrue(input == output);
        }

        // Oooh-weeee we nailed it!
        [Test]
        public void SimpleProgrammingTransactionsTest()
        {
            string sui = "0x0000000000000000000000000000000000000000000000000000000000000002";
            AccountAddress suiAddress = AccountAddress.FromHex(sui);

            string capy = "0x0000000000000000000000000000000000000000000000000000000000000006";
            AccountAddress capyAddress = AccountAddress.FromHex(capy);

            MoveCall moveCallTransaction = new MoveCall(
                new SuiMoveNormalizedStructType(suiAddress, "display", "new", new System.Collections.Generic.List<Rpc.Models.SuiMoveNormalizedType>()), // TODO: THIS IS A NORMALIZED STRUCT
                new SerializableTypeTag[] { new SerializableTypeTag(new SuiStructTag(capyAddress, "capy", "Capy", new System.Collections.Generic.List<SerializableTypeTag>())) },
                new TransactionArgument[]
                {
                    new TransactionArgument(TransactionArgumentKind.GasCoin, null),
                    new TransactionArgument(TransactionArgumentKind.NestedResult, new NestedResult(0, 1)),
                    new TransactionArgument(TransactionArgumentKind.Input, new TransactionBlockInput(3)),
                    new TransactionArgument(TransactionArgumentKind.Result, new Result(1))
                }
            );

            Serialization serializer = new Serialization();
            moveCallTransaction.Serialize(serializer);
            byte[] actual = serializer.GetBytes();
            byte[] expected = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 7, 100, 105, 115, 112, 108, 97, 121, 3, 110, 101, 119, 1, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 4, 99, 97, 112, 121, 4, 67, 97, 112, 121, 0, 4, 0, 3, 0, 0, 1, 0, 1, 3, 0, 2, 1, 0 };

            Assert.AreEqual(expected, actual,
                "ACTUAL LENGHT: " + actual.Length + "\n"
                + "EXPECTED LENGTH: " + expected.Length + "\n" + actual.ToReadableString());
        }
    }
}