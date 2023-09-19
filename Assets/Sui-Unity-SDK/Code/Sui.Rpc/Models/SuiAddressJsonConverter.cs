using System;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    public class SuiAddressJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AccountAddress);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            return AccountAddress.FromHex(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var address = (AccountAddress)value;
            writer.WriteValue(address.ToHex());
        }
    }
}