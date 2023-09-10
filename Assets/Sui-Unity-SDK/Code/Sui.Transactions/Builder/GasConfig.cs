using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    //const GasConfig = object({
    //	budget: optional(StringEncodedBigint),
    //	price: optional(StringEncodedBigint),
    //	payment: optional(array(SuiObjectRef)),
    //	owner: optional(string()),
    //});
    public class GasConfig : ISerializable
    {
        public long? budget;    // BigInt
        public long? price;     // BigInt
        public SuiObjectRef[] payment;
        public AccountAddress owner;

        public GasConfig(string budget = null, string price = null, SuiObjectRef[] payment = null, AccountAddress owner = null)
        {
            this.budget = budget != null ? long.Parse(budget) : null;
            this.price = price != null ? long.Parse(price) : null;
            this.payment = payment;
            this.owner = owner;
        }

        public void Serialize(Serialization serializer)
        {
            Sequence paymentSeq = new Sequence(payment);
            serializer.Serialize(paymentSeq);
            //paymentSeq.Serialize(serializer);
            serializer.Serialize(owner);
            serializer.SerializeU64((ulong)price);
            serializer.SerializeU64((ulong)budget);

            Serialization ser = new Serialization();
            Sequence paymentSeq2 = new Sequence(payment);
            ser.Serialize(paymentSeq2);
            //paymentSeq.Serialize(serializer);
            ser.Serialize(owner);
            ser.SerializeU64((ulong)price);
            ser.SerializeU64((ulong)budget);

            Debug.Log(" === GasConfig ::: ");
            Debug.Log(ser.GetBytes().ByteArrayToString());
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }
}
