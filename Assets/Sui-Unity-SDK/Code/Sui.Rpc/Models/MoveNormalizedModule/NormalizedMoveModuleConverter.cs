using System.Collections.Generic;
using Newtonsoft.Json;
using OpenDive.BCS;
using System;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Player;
using static UnityEditor.Progress;
using NUnit.Framework;
using System.Linq;

namespace Sui.Rpc.Models
{
    public class NormalizedModulesByPackageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, SuiMoveNormalizedModule>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Marcus: Could probably use a better solution than to make a whole other converter just for the Dictionary<string, SuiMoveNormalizedModule> type.
            // Attempted to just use the types as is within the API earlier, was throwing errors.
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject dictionaryObject = JObject.Load(reader);
                dictionaryObject = (JObject)dictionaryObject["result"];
                Dictionary<string, SuiMoveNormalizedModule> dictionary = new Dictionary<string, SuiMoveNormalizedModule>();
                foreach (JProperty dictObjectProp in dictionaryObject.Properties())
                {
                    string moduleName = dictObjectProp.Name;
                    NormalizedMoveModuleConverter converter = new NormalizedMoveModuleConverter();
                    JsonReader newReader = dictionaryObject[moduleName].CreateReader();
                    object result = converter.ReadJson(
                        newReader,
                        typeof(SuiMoveNormalizedModule),
                        null,
                        serializer
                    );
                    dictionary[moduleName] = (Models.SuiMoveNormalizedModule)result;
                }

                return dictionary;
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class NormalizedTypesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<SuiMoveNormalizedType>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray typeObj = JArray.Load(reader);

            List<SuiMoveNormalizedType> normalizedTypes = new List<SuiMoveNormalizedType>();

            foreach (JToken typeObject in typeObj)
            {
                NormalizedTypeConverter jsonConverter = new NormalizedTypeConverter();
                SuiMoveNormalizedType normalizedType = jsonConverter.ReadJson(
                    typeObject.CreateReader(),
                    typeof(SuiMoveNormalizedType),
                    null,
                    serializer
                ) as SuiMoveNormalizedType;
                normalizedTypes.Add(normalizedType);
            }

            return normalizedTypes;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class NormalizedTypeConverter : JsonConverter
    {
        public static string NormalizeSuiAddress(string address, bool forceAdd0x = false)
        {
            // If the address starts with "0x", remove it
            if (address.StartsWith("0x") && !forceAdd0x)
                address = address[2..];

            // Ensure the address is not longer than the desired length
            if (address.Length > 64)
                return null;

            // Pad the address with zeros to reach 64 characters
            string paddedAddress = address.PadLeft(64, '0');

            // Return the normalized address with "0x" prefix
            return "0x" + paddedAddress;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SuiMoveNormalizedType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject typeObj = JObject.Load(reader);

                if (reader.TokenType == JsonToken.StartObject)
                    typeObj = (JObject)typeObj["result"];

                ISuiMoveNormalizedType normalziedType;
                SuiMoveNormalizedTypeSerializationType type;

                // NOTE: This only iterates once because there's only one object. For example,
                // below there's only one "Struct" object hence the property is only "Struct"
                // "type":{
                //    "Struct":{
                //      "address":"0x2",
                //      "module":"object",
                //      "name":"UID",
                //      "typeArguments":[
                //      ]
                //    }
                // }
                string objType = typeObj.Properties().ToArray()[0].Name;
                switch (objType)
                {
                    // SuiMoveNormalizedTypeParameterType
                    case "TypeParameter":
                        normalziedType = new SuiMoveNormalziedTypeParameterType((ushort)typeObj.GetValue(objType));
                        type = SuiMoveNormalizedTypeSerializationType.TypeParameter;
                        break;
                    // SuiMoveNormalizedReferenceType
                    case "Reference":
                        JToken referenceTypeObj = typeObj[objType];
                        NormalizedTypeConverter referenceTypeConverter = new NormalizedTypeConverter();
                        SuiMoveNormalizedType reference = referenceTypeConverter.ReadJson(
                            referenceTypeObj.CreateReader(),
                            typeof(SuiMoveNormalizedType),
                            null,
                            serializer
                        ) as SuiMoveNormalizedType;
                        normalziedType = new SuiMoveNormalizedTypeReference(reference);
                        type = SuiMoveNormalizedTypeSerializationType.Reference;
                        break;
                    // SuiMoveNormalizedMutableReferenceType
                    case "MutableReference":
                        JToken mutableReferenceTypeObj = typeObj[objType];
                        NormalizedTypeConverter mutableReferenceTypeConverter = new NormalizedTypeConverter();
                        SuiMoveNormalizedType mutableReference = mutableReferenceTypeConverter.ReadJson(
                            mutableReferenceTypeObj.CreateReader(),
                            typeof(SuiMoveNormalizedType),
                            null,
                            serializer
                        ) as SuiMoveNormalizedType;
                        normalziedType = new SuiMoveNormalizedTypeMutableReference(mutableReference);
                        type = SuiMoveNormalizedTypeSerializationType.MutableReference;
                        break;
                    // SuiMoveNormalizedVectorType
                    case "Vector":
                        JToken vectorTypeObj = typeObj[objType];
                        NormalizedTypeConverter vectorTypeConverter = new NormalizedTypeConverter();
                        SuiMoveNormalizedType vector = vectorTypeConverter.ReadJson(
                            vectorTypeObj.CreateReader(),
                            typeof(SuiMoveNormalizedType),
                            null,
                            serializer
                        ) as SuiMoveNormalizedType;
                        normalziedType = new SuiMoveNormalizedTypeVector(vector);
                        type = SuiMoveNormalizedTypeSerializationType.Vector;
                        break;
                    // SuiMoveNormalizedStructType
                    case "Struct":
                        JObject structTypeObj = (JObject)typeObj[objType];
                        AccountAddress address = AccountAddress.FromHex(NormalizeSuiAddress((string)structTypeObj["address"]));
                        string module = (string)structTypeObj["module"];
                        string name = (string)structTypeObj["name"];

                        JArray arguments = (JArray)structTypeObj["typeArguments"];
                        List<SuiMoveNormalizedType> argumentsTypes = new List<SuiMoveNormalizedType>();
                        foreach (JToken argument in arguments)
                        {
                            NormalizedTypeConverter argumentsTypeConverter = new NormalizedTypeConverter();
                            SuiMoveNormalizedType argumentType = argumentsTypeConverter.ReadJson(
                                argument.CreateReader(),
                                typeof(SuiMoveNormalizedType),
                                null,
                                serializer
                            ) as SuiMoveNormalizedType;
                            argumentsTypes.Add(argumentType);
                        }

                        SuiMoveNormalizedStructType structTag = new SuiMoveNormalizedStructType(address, module, name, argumentsTypes);
                        normalziedType = new SuiMoveNormalziedTypeStruct(structTag);
                        type = SuiMoveNormalizedTypeSerializationType.Struct;
                        break;
                    default:
                        throw new ArgumentException();
                }

                if (normalziedType != null)
                    return new SuiMoveNormalizedType(normalziedType, type);
            }
            catch
            {
                string objType = reader.Value.ToString();
                return new SuiMoveNormalizedType(new SuiMoveNormalizedTypeString(objType), SuiMoveNormalizedTypeSerializationType.String);
            }

            throw new NotImplementedException();
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
                        writer.WritePropertyName(((SuiMoveNormalizedTypeString)normalized_type.NormalizedType).Value);
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
                        writer.WriteRaw(JsonConvert.SerializeObject((SuiMoveNormalizedStructType)normalized_type.NormalizedType));
                        break;
                }

                if (normalized_type.Type != SuiMoveNormalizedTypeSerializationType.String) writer.WriteEndObject();
            }
        }
    }

    public class MoveStructConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SuiMoveNormalizedStruct);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject structObj = JObject.Load(reader);

            if (reader.TokenType == JsonToken.StartObject)
                structObj = (JObject)structObj["result"];

            SuiMoveNormalizedStruct normalizedStruct = new SuiMoveNormalizedStruct();
            SuiMoveAbilitySet abilitySet = JsonConvert.DeserializeObject<SuiMoveAbilitySet>(structObj["abilities"].ToString());
            List<SuiMoveStructTypeParameter> typeParams = new List<SuiMoveStructTypeParameter>();

            if (structObj["TypeParameters"] != null)
            {
                foreach (JProperty type in structObj["TypeParameters"])
                    typeParams.Add(JsonConvert.DeserializeObject<SuiMoveStructTypeParameter>((string)type));
            }
            List<SuiMoveNormalizedField> normalizedFields = new List<SuiMoveNormalizedField>();
            foreach (JObject field in (JArray)structObj["fields"])
            {
                SuiMoveNormalizedField normalizedField = new SuiMoveNormalizedField();

                normalizedField.Name = (string)field["name"];

                JToken typeObj = field["type"];

                NormalizedTypeConverter typeConverter = new NormalizedTypeConverter();
                normalizedField.Type = typeConverter.ReadJson(
                    typeObj.CreateReader(),
                    typeof(SuiMoveNormalizedType),
                    null,
                    serializer
                ) as SuiMoveNormalizedType;

                normalizedFields.Add(normalizedField);
            }
            normalizedStruct.Abilities = abilitySet;
            normalizedStruct.TypeParameters = typeParams.ToArray();
            normalizedStruct.Fields = normalizedFields.ToArray();
            return normalizedStruct;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class NormalizedMoveModuleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SuiMoveNormalizedModule);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Marcus: Temporary removed check for beginning token because NormalizedModuleFromPackage is a dictionary,
            // so the inputted reader wouldn't be a starting object.
            JObject item = JObject.Load(reader);

            if (reader.TokenType == JsonToken.StartObject)
                item = (JObject)item["result"];

            SuiMoveNormalizedModule moveModule = new SuiMoveNormalizedModule();
            Dictionary<string, SuiMoveNormalizedStruct> Structs = new Dictionary<string, SuiMoveNormalizedStruct>();
            Dictionary<string, NormalizedMoveFunctionResponse> ExposedFunctions = new Dictionary<string, NormalizedMoveFunctionResponse>();

            if (item["fileFormatVersion"] != null) moveModule.FileFormatVersion = (int)item["fileFormatVersion"];
            if (item["address"] != null) moveModule.Address = (string)item["address"];
            if (item["name"] != null) moveModule.Name = (string)item["name"];
            List<Friends> Friends = new List<Friends>();

            foreach (JObject friend in item["friends"])
                Friends.Add(JsonConvert.DeserializeObject<Friends>(friend.ToString()));

            moveModule.Friends = Friends.ToArray();

            JObject structs = (JObject)item["structs"];
            foreach (JProperty property in structs.Properties())
            {
                JObject structObj = (JObject)structs[property.Name];
                MoveStructConverter converter = new MoveStructConverter();

                SuiMoveNormalizedStruct normalizedStruct = converter.ReadJson(
                    structObj.CreateReader(),
                    typeof(SuiMoveNormalizedStruct),
                    null,
                    serializer
                ) as SuiMoveNormalizedStruct;

                Structs[property.Name] = normalizedStruct;
            }
            moveModule.Structs = Structs;

            JObject exposedFunctions = (JObject)item["exposedFunctions"];

            foreach(JProperty property in exposedFunctions.Properties())
            {
                JObject exposed_func_obj = (JObject)exposedFunctions[property.Name];
                ExposedFunctions[property.Name] = JsonConvert.DeserializeObject<NormalizedMoveFunctionResponse>(exposed_func_obj.ToString());
            }

            moveModule.ExposedFunctions = ExposedFunctions;

            return moveModule;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

