using System;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    public class SuiAddressJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type object_type) => object_type == typeof(AccountAddress);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            return AccountAddress.FromHex(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AccountAddress address = (AccountAddress)value;
            writer.WriteValue(address.ToHex());
        }
    }
}