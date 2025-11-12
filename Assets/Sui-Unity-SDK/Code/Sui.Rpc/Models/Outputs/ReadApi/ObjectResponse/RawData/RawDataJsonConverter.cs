//
//  RawDataJsonConverter.cs
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
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class RawDataJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(RawData);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject parsed_data = JObject.Load(reader);
                string data_type = parsed_data["dataType"].Value<string>();

                return data_type switch
                {
                    "moveObject" => new RawData
                                        (
                                            RawDataType.MoveObject,
                                            new MoveObjectRawData
                                            (
                                                parsed_data["bcsBytes"].Value<string>(),
                                                parsed_data["hasPublicTransfer"].Value<bool>(),
                                                parsed_data["type"].Value<string>(),
                                                BigInteger.Parse(parsed_data["version"].Value<string>())
                                            )
                                        ),
                    "packageObject" => new RawData
                                       (
                                           RawDataType.Package,
                                           new PackageRawData
                                           (
                                               parsed_data["id"].ToObject<AccountAddress>(),
                                               parsed_data["linkageTable"].ToObject<Dictionary<string, UpgradeInfo>>(),
                                               parsed_data["moduleMap"].ToObject<Dictionary<string, string>>(),
                                               ((JArray)parsed_data["typeOriginTable"]).Select(item => item.ToObject<TypeOrigin>()).ToArray(),
                                               BigInteger.Parse(parsed_data["version"].Value<string>())
                                           )
                                       ),
                    _ => new RawData(new SuiError(0, $"Unable to convert {data_type} to RawDataType.", null)),
                };
            }

            return new RawData(new SuiError(0, "Unable to convert JSON to RawData.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();

                RawData raw_data = (RawData)value;

                writer.WritePropertyName("dataType");
                writer.WriteValue(raw_data.Type.ToString());

                writer.WritePropertyName("version");
                writer.WriteValue(raw_data.Data.Version.ToString());

                switch (raw_data.Type)
                {
                    case RawDataType.MoveObject:
                        MoveObjectRawData parsed_move_object = (MoveObjectRawData)raw_data.Data;

                        writer.WritePropertyName("bcsBytes");
                        writer.WriteValue(parsed_move_object.BCSBytes);

                        writer.WritePropertyName("hasPublicTransfer");
                        writer.WriteValue(parsed_move_object.HasPublicTransfer);

                        writer.WritePropertyName("type");
                        writer.WriteValue(parsed_move_object.ObjectType);

                        break;
                    case RawDataType.Package:
                        PackageRawData parsed_move_package = (PackageRawData)raw_data.Data;

                        writer.WritePropertyName("id");
                        writer.WriteValue(parsed_move_package.ID.KeyHex);

                        writer.WritePropertyName("linkageTable");
                        writer.WriteValue(parsed_move_package.LinkageTable.GetValuesAsString());

                        writer.WritePropertyName("moduleMap");
                        writer.WriteValue(parsed_move_package.ModuleMap.GetValuesAsString());

                        writer.WritePropertyName("typeOriginTable");
                        writer.WriteValue(parsed_move_package.TypeOriginTable.ToReadableString());

                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}