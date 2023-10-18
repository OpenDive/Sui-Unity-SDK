using System;
using Newtonsoft.Json;
using OpenDive.BCS;

namespace Sui.Rpc.Models
{
    public class SuiStructTagConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SuiStructTag);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            return SuiStructTag.FromStr(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SuiStructTag suiStructTag = (SuiStructTag)value;
            writer.WriteValue(suiStructTag.ToString());
        }
    }
}