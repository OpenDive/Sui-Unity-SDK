using System;
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.ZKLogin.Utils
{
    public class BigIntegerJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BigInteger) || objectType == typeof(BigInteger?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType == typeof(BigInteger?))
                    return null;
                return BigInteger.Zero;
            }

            if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer)
            {
                string value = reader.Value.ToString();
                if (BigInteger.TryParse(value, out BigInteger result))
                    return result;
            }

            throw new JsonSerializationException($"Cannot convert {reader.Value} to BigInteger");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            BigInteger bigIntValue = (BigInteger)value;
            writer.WriteValue(bigIntValue.ToString());
        }
    }
}
