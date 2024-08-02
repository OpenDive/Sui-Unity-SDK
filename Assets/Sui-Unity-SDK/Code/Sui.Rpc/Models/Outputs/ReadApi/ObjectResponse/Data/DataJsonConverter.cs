//
//  DataJsonConverter.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Types;
using Sui.Utilities;

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
                                parsed_data["disassembled"].ToObject<Dictionary<string, string>>()
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