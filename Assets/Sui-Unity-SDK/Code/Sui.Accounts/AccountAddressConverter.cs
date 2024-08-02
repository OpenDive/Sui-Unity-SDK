using System;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    public class AccountAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type object_type) => object_type == typeof(AccountAddress);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => new AccountAddress(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(((AccountAddress)value).ToHex());
        }
    }
}