using System.Collections.Generic;
using Newtonsoft.Json;
using OpenDive.BCS;
using System;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using System.Linq;
using Sui.Rpc.Client;
using UnityEngine;

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

    //public class MoveStructConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(SuiMoveNormalizedStruct);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        JObject structObj = JObject.Load(reader);

    //        if (reader.TokenType == JsonToken.StartObject)
    //            structObj = (JObject)structObj["result"];

    //        SuiMoveNormalizedStruct normalizedStruct = new SuiMoveNormalizedStruct();
    //        SuiMoveAbilitySet abilitySet = JsonConvert.DeserializeObject<SuiMoveAbilitySet>(structObj["abilities"].ToString());
    //        List<SuiMoveStructTypeParameter> typeParams = new List<SuiMoveStructTypeParameter>();

    //        if (structObj["TypeParameters"] != null)
    //        {
    //            foreach (JProperty type in structObj["TypeParameters"])
    //                typeParams.Add(JsonConvert.DeserializeObject<SuiMoveStructTypeParameter>((string)type));
    //        }
    //        List<SuiMoveNormalizedField> normalizedFields = new List<SuiMoveNormalizedField>();
    //        foreach (JObject field in (JArray)structObj["fields"])
    //        {
    //            SuiMoveNormalizedField normalizedField = new SuiMoveNormalizedField();

    //            normalizedField.Name = (string)field["name"];

    //            JToken typeObj = field["type"];

    //            NormalizedTypeConverter typeConverter = new NormalizedTypeConverter();
    //            normalizedField.Type = typeConverter.ReadJson(
    //                typeObj.CreateReader(),
    //                typeof(SuiMoveNormalizedType),
    //                null,
    //                serializer
    //            ) as SuiMoveNormalizedType;

    //            normalizedFields.Add(normalizedField);
    //        }
    //        normalizedStruct.Abilities = abilitySet;
    //        normalizedStruct.TypeParameters = typeParams.ToArray();
    //        normalizedStruct.Fields = normalizedFields.ToArray();
    //        return normalizedStruct;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class NormalizedMoveModuleConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(SuiMoveNormalizedModule);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        // Marcus: Temporary removed check for beginning token because NormalizedModuleFromPackage is a dictionary,
    //        // so the inputted reader wouldn't be a starting object.
    //        JObject item = JObject.Load(reader);

    //        if (reader.TokenType == JsonToken.StartObject)
    //            item = (JObject)item["result"];

    //        SuiMoveNormalizedModule moveModule = new SuiMoveNormalizedModule();
    //        Dictionary<string, SuiMoveNormalizedStruct> Structs = new Dictionary<string, SuiMoveNormalizedStruct>();
    //        Dictionary<string, NormalizedMoveFunctionResponse> ExposedFunctions = new Dictionary<string, NormalizedMoveFunctionResponse>();

    //        if (item["fileFormatVersion"] != null) moveModule.FileFormatVersion = (int)item["fileFormatVersion"];
    //        if (item["address"] != null) moveModule.Address = (string)item["address"];
    //        if (item["name"] != null) moveModule.Name = (string)item["name"];
    //        List<Friends> Friends = new List<Friends>();

    //        foreach (JObject friend in item["friends"])
    //            Friends.Add(JsonConvert.DeserializeObject<Friends>(friend.ToString()));

    //        moveModule.Friends = Friends.ToArray();

    //        JObject structs = (JObject)item["structs"];
    //        foreach (JProperty property in structs.Properties())
    //        {
    //            JObject structObj = (JObject)structs[property.Name];
    //            MoveStructConverter converter = new MoveStructConverter();

    //            SuiMoveNormalizedStruct normalizedStruct = converter.ReadJson(
    //                structObj.CreateReader(),
    //                typeof(SuiMoveNormalizedStruct),
    //                null,
    //                serializer
    //            ) as SuiMoveNormalizedStruct;

    //            Structs[property.Name] = normalizedStruct;
    //        }
    //        moveModule.Structs = Structs;

    //        JObject exposedFunctions = (JObject)item["exposedFunctions"];

    //        foreach(JProperty property in exposedFunctions.Properties())
    //        {
    //            JObject exposed_func_obj = (JObject)exposedFunctions[property.Name];
    //            ExposedFunctions[property.Name] = JsonConvert.DeserializeObject<NormalizedMoveFunctionResponse>(exposed_func_obj.ToString());
    //        }

    //        moveModule.ExposedFunctions = ExposedFunctions;

    //        return moveModule;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

