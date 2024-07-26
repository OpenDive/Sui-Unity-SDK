//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Sui.Rpc.Models
//{
//    public class MoveStructJsonConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType) => objectType == typeof(MoveStruct);

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            var jsonObject = JObject.Load(reader);
//            MoveStruct moveStruct;

//            if (jsonObject.Type == JTokenType.Array)
//            {
//                moveStruct = new ArrayMoveStruct
//                {
//                    Value = jsonObject.ToObject<List<MoveValue>>(serializer)
//                };
//            }
//            else if (jsonObject.Property("fields") != null && jsonObject.Property("type") != null)
//            {
//                moveStruct = new ObjectMoveStruct
//                {
//                    Fields = jsonObject["fields"].ToObject<Dictionary<string, MoveValue>>(serializer),
//                    Type = jsonObject["type"].ToString()
//                };
//            }
//            else
//            {
//                moveStruct = new AdditionalPropertiesMoveStruct
//                {
//                    AdditionalProperties = jsonObject.ToObject<Dictionary<string, MoveValue>>(serializer)
//                };
//            }

//            return moveStruct;
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            if (value is ArrayMoveStruct arrayMove)
//            {
//                serializer.Serialize(writer, arrayMove.Value);
//            }
//            else if (value is ObjectMoveStruct objectMove)
//            {
//                JObject jObject = new JObject
//        {
//            {"fields", JObject.FromObject(objectMove.Fields, serializer)},
//            {"type", JValue.FromObject(objectMove.Type)}
//        };
//                jObject.WriteTo(writer);
//            }
//            else if (value is AdditionalPropertiesMoveStruct additionalPropertiesMove)
//            {
//                serializer.Serialize(writer, additionalPropertiesMove.AdditionalProperties);
//            }
//            else
//            {
//                throw new JsonSerializationException($"Unsupported type: {value.GetType().FullName}");
//            }
//        }
//    }
//}