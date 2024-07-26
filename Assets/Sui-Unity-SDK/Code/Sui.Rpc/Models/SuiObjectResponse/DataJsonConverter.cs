using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    public class DataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Data);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject parsed_data = JObject.Load(reader);

                string data_type = parsed_data["dataType"].Value<string>();

                switch (data_type)
                {
                    case "moveObject":
                        return new Data
                        (
                            DataType.MoveObject,
                            new ParsedMoveObject
                            (
                                parsed_data["fields"],
                                parsed_data["hasPublicTransfer"].Value<bool>(),
                                new SuiStructTag(parsed_data["type"].Value<string>())
                            )
                        );
                    case "movePackage":
                        return new Data
                        (
                            DataType.MovePackage,
                            new ParsedMovePackage
                            (
                                parsed_data["disassembled"].ToObject<Dictionary<string, object>>()
                            )
                        );
                    default:
                        return new Data(new SuiError(0, $"Unable to convert {data_type} to DataType.", null));
                }
            }

            return new Data(new SuiError(0, "Unable to convert JSON to Data.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();

                Data parsed_data = (Data)value;

                writer.WritePropertyName("dataType");
                writer.WriteValue(parsed_data.Type.ToString());

                switch (parsed_data.Type)
                {
                    case DataType.MoveObject:
                        ParsedMoveObject parsed_move_object = (ParsedMoveObject)parsed_data.ParsedData;

                        writer.WritePropertyName("fields");
                        writer.WriteValue(parsed_move_object.Fields);

                        writer.WritePropertyName("hasPublicTransfer");
                        writer.WriteValue(parsed_move_object.HasPublicTransfer);

                        writer.WritePropertyName("type");
                        writer.WriteValue(parsed_move_object.Type);

                        break;
                    case DataType.MovePackage:
                        ParsedMovePackage parsed_move_package = (ParsedMovePackage)parsed_data.ParsedData;

                        writer.WritePropertyName("disassembled");
                        writer.WriteValue(parsed_move_package.Disassembled);

                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}