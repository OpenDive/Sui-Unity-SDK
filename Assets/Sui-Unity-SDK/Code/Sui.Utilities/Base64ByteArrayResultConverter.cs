using System;
using Chaos.NaCl;
using Newtonsoft.Json;

namespace Sui.Utilities
{
    /// <summary>
    /// Used for converting JSON (A Base 64 string) to a Result object containing a byte array.
    /// </summary>
    public class Base64ByteArrayResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SuiResult<byte[]>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return new SuiResult<byte[]>(CryptoBytes.FromBase64String(reader.Value.ToString()));

            return new SuiResult<byte[]>(null, new SuiError(0, "Unable to convert JSON to a byte array.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                SuiResult<byte[]> byte_array_result = (SuiResult<byte[]>)value;

                if (byte_array_result.Error != null)
                    writer.WriteValue(JsonConvert.SerializeObject(byte_array_result.Error));
                else
                    writer.WriteValue(CryptoBytes.ToBase64String(byte_array_result.Result));
            }
        }
    }
}