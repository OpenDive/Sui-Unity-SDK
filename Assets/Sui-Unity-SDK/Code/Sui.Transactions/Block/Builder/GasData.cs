using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    public class GasDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GasData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject gas_data = JObject.Load(reader);

            string owner = gas_data["owner"].Value<string>();
            string price = gas_data["price"].Value<string>();
            string budget = gas_data["budget"].Value<string>();

            List<Sui.Types.SuiObjectRef> objects = new List<Sui.Types.SuiObjectRef>();
            foreach (JObject payment_object in (JArray)gas_data["payment"])
                objects.Add
                (
                    new Sui.Types.SuiObjectRef
                    (
                        payment_object["objectId"].Value<string>(),
                        payment_object["version"].Value<int>(),
                        payment_object["digest"].Value<string>()
                    )
                );

            return new GasData
            (
                budget,
                price,
                objects.ToArray(),
                AccountAddress.FromHex(owner)
            );
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();
                GasData gas_data = (GasData)value;

                writer.WritePropertyName("owner");
                writer.WriteValue(gas_data.Owner.ToHex());

                writer.WritePropertyName("price");
                writer.WriteValue(gas_data.Price);

                writer.WritePropertyName("budget");
                writer.WriteValue(gas_data.Budget);

                writer.WritePropertyName("payment");
                writer.WriteStartArray();

                foreach (Sui.Types.SuiObjectRef payment in gas_data.Payment)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("objectId");
                    writer.WriteValue(payment.ObjectIDString);

                    writer.WritePropertyName("version");
                    writer.WriteValue(payment.Version);

                    writer.WritePropertyName("digest");
                    writer.WriteValue(payment.Digest);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }

    [JsonConverter(typeof(GasDataConverter))]
    public class GasData : ISerializable
    {
        [JsonProperty("budget")]
        public BigInteger? Budget { get; set; }

        [JsonProperty("price")]
        public BigInteger? Price { get; set; }

        [JsonProperty("payment")]
        public Sui.Types.SuiObjectRef[] Payment { get; set; }

        [JsonProperty("owner")]
        public AccountAddress Owner { get; set; }

        public GasData(
            string budget = null,
            string price = null,
            Sui.Types.SuiObjectRef[] payment = null,
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
            Sui.Types.SuiObjectRef[] payment = deserializer.DeserializeSequence(typeof(Sui.Types.SuiObjectRef)).Cast<Sui.Types.SuiObjectRef>().ToArray();
            AccountAddress owner = (AccountAddress)AccountAddress.Deserialize(deserializer);
            U64 price = (U64)deserializer.DeserializeOptional(typeof(U64));
            U64 budget = (U64)deserializer.DeserializeOptional(typeof(U64));

            return new GasData(
                budget != null ? $"{budget.value}" : null,
                price != null ? $"{price.value}" : null,
                payment,
                owner
            );
        }
    }
}
