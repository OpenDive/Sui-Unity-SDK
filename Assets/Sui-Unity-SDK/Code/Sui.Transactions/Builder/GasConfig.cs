using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;

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
            serializer.Serialize(owner);
            serializer.SerializeU64((ulong)price);
            serializer.SerializeU64((ulong)budget);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }
}
