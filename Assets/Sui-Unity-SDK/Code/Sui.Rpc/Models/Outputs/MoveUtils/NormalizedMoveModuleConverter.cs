//
//  NormalizedTypeConverter.cs
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

using System.Collections.Generic;
using Newtonsoft.Json;
using Sui.Types;
using System;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using System.Linq;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class NormalizedTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SuiMoveNormalizedType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject type_object = JObject.Load(reader);

                if (reader.TokenType == JsonToken.StartObject)
                    type_object = (JObject)type_object["result"];

                string object_type = type_object.Properties().ToArray()[0].Name;
                switch (object_type)
                {
                    // SuiMoveNormalizedTypeParameterType
                    case "TypeParameter":
                        return new SuiMoveNormalizedType
                        (
                            new SuiMoveNormalziedTypeParameterType((ushort)type_object.GetValue(object_type)),
                            SuiMoveNormalizedTypeSerializationType.TypeParameter
                        );

                    // SuiMoveNormalizedReferenceType
                    case "Reference":
                        JToken reference_type_object = type_object[object_type];
                        NormalizedTypeConverter reference_type_converter = new NormalizedTypeConverter();
                        SuiMoveNormalizedType reference = reference_type_converter.ReadJson
                        (
                            reference_type_object.CreateReader(),
                            typeof(SuiMoveNormalizedType),
                            null,
                            serializer
                        ) as SuiMoveNormalizedType;
                        return new SuiMoveNormalizedType
                        (
                            new SuiMoveNormalizedTypeReference(reference),
                            SuiMoveNormalizedTypeSerializationType.Reference
                        );

                    // SuiMoveNormalizedMutableReferenceType
                    case "MutableReference":
                        JToken mutable_reference_type_object = type_object[object_type];
                        NormalizedTypeConverter mutable_reference_type_converter = new NormalizedTypeConverter();
                        SuiMoveNormalizedType mutable_reference = mutable_reference_type_converter.ReadJson
                        (
                            mutable_reference_type_object.CreateReader(),
                            typeof(SuiMoveNormalizedType),
                            null,
                            serializer
                        ) as SuiMoveNormalizedType;
                        return new SuiMoveNormalizedType
                        (
                            new SuiMoveNormalizedTypeMutableReference(mutable_reference),
                            SuiMoveNormalizedTypeSerializationType.MutableReference
                        );

                    // SuiMoveNormalizedVectorType
                    case "Vector":
                        JToken vector_type_object = type_object[object_type];
                        NormalizedTypeConverter vector_type_converter = new NormalizedTypeConverter();
                        SuiMoveNormalizedType vector = vector_type_converter.ReadJson
                        (
                            vector_type_object.CreateReader(),
                            typeof(SuiMoveNormalizedType),
                            null,
                            serializer
                        ) as SuiMoveNormalizedType;
                        return new SuiMoveNormalizedType
                        (
                            new SuiMoveNormalizedTypeVector(vector),
                            SuiMoveNormalizedTypeSerializationType.Vector
                        );

                    // SuiMoveNormalizedStructType
                    case "Struct":
                        JToken struct_type_object = type_object[object_type];
                        AccountAddress address = AccountAddress.FromHex((string)struct_type_object["address"]);
                        string module = (string)struct_type_object["module"];
                        string name = (string)struct_type_object["name"];

                        JArray arguments = (JArray)struct_type_object["typeArguments"];
                        List<SuiMoveNormalizedType> argument_types = new List<SuiMoveNormalizedType>();

                        foreach (JToken argument in arguments)
                        {
                            NormalizedTypeConverter argumentsTypeConverter = new NormalizedTypeConverter();
                            argument_types.Add
                            (
                                argumentsTypeConverter.ReadJson
                                (
                                    argument.CreateReader(),
                                    typeof(SuiMoveNormalizedType),
                                    null,
                                    serializer
                                ) as SuiMoveNormalizedType
                            );
                        }

                        SuiMoveNormalizedStructType struct_tag = new SuiMoveNormalizedStructType(address, module, name, argument_types);
                        return new SuiMoveNormalizedType
                        (
                            new SuiMoveNormalziedTypeStruct(struct_tag),
                            SuiMoveNormalizedTypeSerializationType.Struct
                        );

                    default:
                        return new SuiMoveNormalizedType
                        (
                            new SuiError
                            (
                                0,
                                $"Unable to convert {object_type} to SuiMoveNormalizedTypeSerializationType.",
                                reader
                            )
                        );
                }
            }
            catch
            {
                return new SuiMoveNormalizedType
                (
                    new SuiMoveNormalizedTypeString(reader.Value.ToString()),
                    SuiMoveNormalizedTypeSerializationType.String
                );
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                SuiMoveNormalizedType normalized_type = (SuiMoveNormalizedType)value;

                if (normalized_type.Type != SuiMoveNormalizedTypeSerializationType.String) writer.WriteStartObject();

                switch (normalized_type.Type)
                {
                    case SuiMoveNormalizedTypeSerializationType.String:
                        writer.WritePropertyName("String");
                        writer.WriteValue(((SuiMoveNormalizedTypeString)normalized_type.NormalizedType).Value);
                        break;
                    case SuiMoveNormalizedTypeSerializationType.TypeParameter:
                        writer.WritePropertyName("TypeParameter");
                        writer.WriteValue(((SuiMoveNormalziedTypeParameterType)normalized_type.NormalizedType).TypeParameter);
                        break;
                    case SuiMoveNormalizedTypeSerializationType.Reference:
                        writer.WritePropertyName("Reference");
                        writer.WriteRaw(JsonConvert.SerializeObject(((SuiMoveNormalizedTypeReference)normalized_type.NormalizedType).Reference));
                        break;
                    case SuiMoveNormalizedTypeSerializationType.MutableReference:
                        writer.WritePropertyName("MutableReference");
                        writer.WriteRaw(JsonConvert.SerializeObject(((SuiMoveNormalizedTypeMutableReference)normalized_type.NormalizedType).MutableReference));
                        break;
                    case SuiMoveNormalizedTypeSerializationType.Vector:
                        writer.WritePropertyName("Vector");
                        writer.WriteRaw(JsonConvert.SerializeObject(((SuiMoveNormalizedTypeVector)normalized_type.NormalizedType).Vector));
                        break;
                    case SuiMoveNormalizedTypeSerializationType.Struct:
                        writer.WritePropertyName("Struct");
                        writer.WriteRaw(JsonConvert.SerializeObject((SuiMoveNormalziedTypeStruct)normalized_type.NormalizedType));
                        break;
                }

                if (normalized_type.Type != SuiMoveNormalizedTypeSerializationType.String) writer.WriteEndObject();
            }
        }
    }
}

