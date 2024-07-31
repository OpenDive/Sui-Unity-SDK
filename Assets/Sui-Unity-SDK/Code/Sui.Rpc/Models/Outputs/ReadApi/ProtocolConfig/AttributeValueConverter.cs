//
//  AttributeValueConverter.cs
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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class AttributeValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(AttributeValue);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject attribute_value = JObject.Load(reader);

                string attribute_type = attribute_value.Properties().ToArray()[0].Name;

                return attribute_type switch
                {
                    "u64" => new AttributeValue
                    (
                        AttributeValueType.U64,
                        new U64AttributeValue(attribute_value[AttributeValueType.U64.ToString().ToLower()].Value<ulong>())
                    ),
                    "u32" => new AttributeValue
                     (
                        AttributeValueType.U32,
                        new U32AttributeValue(attribute_value[AttributeValueType.U32.ToString().ToLower()].Value<uint>())
                    ),
                    "f64" => new AttributeValue
                    (
                        AttributeValueType.F64,
                        new F64AttributeValue(attribute_value[AttributeValueType.F64.ToString().ToLower()].Value<double>())
                    ),
                    "u16" => new AttributeValue
                    (
                        AttributeValueType.U16,
                        new U16AttributeValue(attribute_value[AttributeValueType.U16.ToString().ToLower()].Value<ushort>())
                    ),
                    _ => new AttributeValue
                    (
                        new SuiError
                        (
                            0,
                            $"Unable to convert {attribute_type} to AttributeValueType.",
                            null
                        )
                    ),
                };
            }

            return new AttributeValue(new SuiError(0, "Unable to convert JSON to AttributeValue.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                AttributeValue attribute_value = (AttributeValue)value;

                writer.WriteStartObject();

                switch (attribute_value.Type)
                {
                    case AttributeValueType.U64:
                        U64AttributeValue u64 = (U64AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.U64.ToString().ToLower());
                        writer.WriteValue(u64.Value);
                        break;
                    case AttributeValueType.U32:
                        U32AttributeValue u32 = (U32AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.U32.ToString().ToLower());
                        writer.WriteValue(u32.Value);
                        break;
                    case AttributeValueType.F64:
                        F64AttributeValue f64 = (F64AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.F64.ToString().ToLower());
                        writer.WriteValue(f64.Value);
                        break;
                    case AttributeValueType.U16:
                        U16AttributeValue u16 = (U16AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.U16.ToString().ToLower());
                        writer.WriteValue(u16.Value);
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}