using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types;

namespace Sui.Transactions.Builder
{
    [JsonObject]
    public class GasData : ISerializable
    {
        [JsonProperty("budget")]
        public BigInteger? Budget { get; set; }

        [JsonProperty("price")]
        public BigInteger? Price { get; set; }

        [JsonProperty("payment")]
        public SuiObjectRef[] Payment { get; set; }

        [JsonProperty("owner")]
        public AccountAddress Owner { get; set; }

        public GasData(
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
            return new GasData(
                deserializer.DeserializeU64().ToString(),
                deserializer.DeserializeU64().ToString(),
                deserializer.DeserializeSequence(typeof(SuiObjectRef)).Cast<SuiObjectRef>().ToArray(),
                AccountAddress.Deserialize(deserializer)
            );
        }
    }
}
