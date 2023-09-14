using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types;
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
        public long? Budget { get; set; }    // BigInt
        public long? Price { get; set; }     // BigInt
        public SuiObjectRef[] Payment { get; set; }
        public AccountAddress Owner { get; set; }

        public GasConfig(string budget = null, string price = null, SuiObjectRef[] payment = null, AccountAddress owner = null)
        {
            this.Budget = budget != null ? long.Parse(budget) : null;
            this.Price = price != null ? long.Parse(price) : null;
            this.Payment = payment;
            this.Owner = owner;
        }

        public void Serialize(Serialization serializer)
        {
            Sequence paymentSeq = new Sequence(Payment);
            serializer.Serialize(paymentSeq);
            serializer.Serialize(Owner);
            serializer.SerializeU64((ulong)Price);
            serializer.SerializeU64((ulong)Budget);

            Serialization ser = new Serialization();
            Sequence paymentSeq2 = new Sequence(Payment);
            ser.Serialize(paymentSeq2);
            ser.Serialize(Owner);
            ser.SerializeU64((ulong)Price);
            ser.SerializeU64((ulong)Budget);

            Debug.Log(" === GasConfig ::: ");
            Debug.Log(ser.GetBytes().ByteArrayToString());
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }
}
