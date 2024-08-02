//
//  SuiMoveNormalizedStructTypeConverter.cs
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Rpc.Models;
using Sui.Utilities;
using System;
using System.Collections.Generic;

namespace Sui.Types
{
    public class SuiMoveNormalizedStructTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SuiMoveNormalizedStructType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject structTypeObj = (JObject)JToken.ReadFrom(reader);
                AccountAddress address = AccountAddress.FromHex
                (
                    Utils.NormalizeSuiAddress(structTypeObj["package"].Value<string>())
                );
                string module = structTypeObj["module"].Value<string>();
                string name = structTypeObj["function"].Value<string>();

                List<SuiMoveNormalizedType> normalizedTypes = new List<SuiMoveNormalizedType>();

                foreach (JToken typeObject in structTypeObj["typeArguments"])
                    normalizedTypes.Add(typeObject.ToObject<SuiMoveNormalizedType>());

                return new SuiMoveNormalizedStructType(address, module, name, normalizedTypes);
            }

            return new SuiMoveNormalizedStructType(new SuiError(0, "Unable to convert JSON to SuiMoveNormalizedStructType.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();
                SuiMoveNormalizedStructType struct_type = (SuiMoveNormalizedStructType)value;

                writer.WritePropertyName("package");
                writer.WriteValue(struct_type.Address.ToHex());

                writer.WritePropertyName("module");
                writer.WriteValue(struct_type.Module);

                writer.WritePropertyName("function");
                writer.WriteValue(struct_type.Name);

                writer.WritePropertyName("typeArguments");
                writer.WriteStartArray();

                foreach (SuiMoveNormalizedType normalized_type in struct_type.TypeArguments)
                    writer.WriteValue(normalized_type);

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }
}