using System;
using System.Linq;
using System.Numerics;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    public class GasConfig : ISerializable
    {
        public BigInteger? Budget { get; set; }    // BigInt
        public BigInteger? Price { get; set; }     // BigInt
        public SuiObjectRef[] Payment { get; set; }
        public AccountAddress Owner { get; set; }

        public GasConfig(
            string budget = null,
            string price = null,
            SuiObjectRef[] payment = null,
            AccountAddress owner = null
        )
        {
            this.Budget = budget != null ? BigInteger.Parse(budget) : null;
            this.Price = price != null ? BigInteger.Parse(price) : null;
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
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new GasConfig(
                deserializer.DeserializeU64().ToString(),
                deserializer.DeserializeU64().ToString(),
                deserializer.DeserializeSequence(typeof(SuiObjectRef)).Cast<SuiObjectRef>().ToArray(),
                AccountAddress.Deserialize(deserializer)
            );
        }
    }
}
