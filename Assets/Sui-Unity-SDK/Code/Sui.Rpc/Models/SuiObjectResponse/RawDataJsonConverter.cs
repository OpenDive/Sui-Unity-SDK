using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    public class RawDataJsonConverter : JsonConverter<RawData>
    {
        public override RawData ReadJson(JsonReader reader, Type objectType, RawData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            string dataType = jsonObject["dataType"].Value<string>();

            switch (dataType)
            {
                case "moveObject":
                    return jsonObject.ToObject<MoveObjectRawData>(serializer);
                case "package":
                    return jsonObject.ToObject<PackageRawData>(serializer);
                default:
                    throw new JsonSerializationException($"Unknown dataType: {dataType}");
            }
        }

        public override void WriteJson(JsonWriter writer, RawData value, JsonSerializer serializer)
        {
            if (value is MoveObjectRawData moveObjectRawData)
            {
                serializer.Serialize(writer, moveObjectRawData);
            }
            else if (value is PackageRawData packageRawData)
            {
                serializer.Serialize(writer, packageRawData);
            }
            else
            {
                throw new JsonSerializationException($"Unsupported type: {value.GetType().FullName}");
            }
        }
    }
}