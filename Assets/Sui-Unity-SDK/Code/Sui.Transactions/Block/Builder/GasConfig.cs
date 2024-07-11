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
            if (Payment != null)
            {
                Sequence paymentSeq = new(Payment);
                serializer.Serialize(paymentSeq);
            }

            if (Owner != null)
                serializer.Serialize(Owner);

            if (Price != null)
                serializer.SerializeU64((ulong)Price);

            if (Budget != null)
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
