using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using UnityEngine;

namespace Sui.Rpc.Models
{
    public class ObjectResponseErrorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectResponseError).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null || reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            ObjectResponseError error;

            string code = jsonObject["code"]?.ToString();

            switch (code)
            {
                case "notExists":
                    AccountAddress objectId = AccountAddress.FromHex(jsonObject["object_id"]?.ToString());
                    error = new NotExistsError(objectId);
                    break;
                case "dynamicFieldNotFound":
                    AccountAddress parentObjectId = AccountAddress.FromHex(jsonObject["parent_object_id"]?.ToString());
                    error = new DynamicFieldNotFoundError(parentObjectId);
                    break;
                case "deleted":
                    error = new DeletedError(
                        jsonObject["digest"]?.ToString(),
                        AccountAddress.FromHex(jsonObject["object_id"]?.ToString()),
                        jsonObject["version"]?.ToString()
                    );
                    break;
                case "unknown":
                    error = new UnknownError();
                    break;
                case "displayError":
                    error = new DisplayError(jsonObject["error"]?.ToString());
                    break;
                default:
                    throw new JsonSerializationException($"Unsupported type: {code}");
            }

            serializer.Populate(jsonObject.CreateReader(), error);

            return error;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}