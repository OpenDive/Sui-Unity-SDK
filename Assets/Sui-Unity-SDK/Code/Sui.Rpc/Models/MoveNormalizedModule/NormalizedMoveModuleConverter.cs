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
                Debug.Log($"MARCUS:::: 0");
                JObject dictionaryObject = JObject.Load(reader);
                dictionaryObject = (JObject)dictionaryObject["result"];
                Debug.Log($"MARCUS:::: 1");
                Debug.Log($"MARCUS::::: {dictionaryObject}");
                Dictionary<string, SuiMoveNormalizedModule> dictionary = new Dictionary<string, SuiMoveNormalizedModule>();
                Debug.Log($"MARCUS:::: 2");
                foreach (JProperty dictObjectProp in dictionaryObject.Properties())
                {
                    Debug.Log($"MARCUS:::: 3");
                    string moduleName = dictObjectProp.Name;
                    Debug.Log($"MARCUS::::::: {dictionaryObject[moduleName]}");
                    NormalizedMoveModuleConverter converter = new NormalizedMoveModuleConverter();
                    JsonReader newReader = dictionaryObject[moduleName].CreateReader();
                    object result = converter.ReadJson(
                        newReader,
                        typeof(SuiMoveNormalizedModule),
                        null,
                        serializer
                    );
                    Debug.Log($"MARCUS:::: RESULT - {result.ToString()}");
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
            return objectType == typeof(List<ISuiMoveNormalizedType>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray typeObj = JArray.Load(reader);

            List<ISuiMoveNormalizedType> normalizedTypes = new List<ISuiMoveNormalizedType>();

            foreach (JToken typeObject in typeObj)
            {
                Debug.Log($"MARCUS::::: {typeObject.ToString()}");
                NormalizedTypeConverter jsonConverter = new NormalizedTypeConverter();
                ISuiMoveNormalizedType normalizedType = jsonConverter.ReadJson(
                    typeObject.CreateReader(),
                    typeof(ISuiMoveNormalizedType),
                    null,
                    serializer
                ) as ISuiMoveNormalizedType;
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
        public static string NormalizeSuiAddress(string address)
        {
            // If the address starts with "0x", remove it
            if (address.StartsWith("0x"))
            {
                address = address.Substring(2);
            }

            // Ensure the address is not longer than the desired length
            if (address.Length > 64)
            {
                throw new ArgumentException("The address is longer than expected.");
            }

            // Pad the address with zeros to reach 64 characters
            string paddedAddress = address.PadLeft(64, '0');

            // Return the normalized address with "0x" prefix
            return "0x" + paddedAddress;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ISuiMoveNormalizedType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                Debug.Log($"MARCUS::::: {reader}");
                JObject typeObj = JObject.Load(reader);

                if (reader.TokenType == JsonToken.StartObject)
                    typeObj = (JObject)typeObj["result"];

                ISuiMoveNormalizedType normalziedType;

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
                foreach (JProperty typeObjectProp in typeObj.Properties())
                {
                    string objType = typeObjectProp.Name;
                    Debug.Log($"MARCUS::::: {typeObjectProp}");
                    Debug.Log($"MARCUS::::: {typeObjectProp.Name}");
                    switch (objType)
                    {
                        // SuiMoveNormalizedTypeParameterType
                        case "TypeParameter":
                            JObject typeParameterTypeObj = (JObject)typeObj[objType];
                            U16 typeParameter = new U16((uint)typeParameterTypeObj["TypeParameter"]);
                            normalziedType = new SuiMoveNormalziedTypeParameterType(typeParameter);
                            break;
                        // SuiMoveNormalizedReferenceType
                        case "Reference":
                            JObject referenceTypeObj = (JObject)typeObj[objType];
                            var referenceTypeConverter = new NormalizedTypeConverter();
                            ISuiMoveNormalizedType reference = referenceTypeConverter.ReadJson(
                                referenceTypeObj.CreateReader(),
                                typeof(ISuiMoveNormalizedType),
                                null,
                                serializer
                            ) as ISuiMoveNormalizedType;
                            normalziedType = new SuiMoveNormalizedTypeReference(reference);
                            break;
                        // SuiMoveNormalizedMutableReferenceType
                        case "MutableReference":
                            JObject mutableReferenceTypeObj = (JObject)typeObj[objType];
                            var mutableReferenceTypeConverter = new NormalizedTypeConverter();
                            ISuiMoveNormalizedType mutableReference = mutableReferenceTypeConverter.ReadJson(
                                mutableReferenceTypeObj.CreateReader(),
                                typeof(ISuiMoveNormalizedType),
                                null,
                                serializer
                            ) as ISuiMoveNormalizedType;
                            normalziedType = new SuiMoveNormalizedTypeReference(mutableReference);
                            break;
                        // SuiMoveNormalizedVectorType
                        case "Vector":
                            JObject vectorTypeObj = (JObject)typeObj[objType];
                            var vectorTypeConverter = new NormalizedTypeConverter();
                            ISuiMoveNormalizedType vector = vectorTypeConverter.ReadJson(
                                vectorTypeObj.CreateReader(),
                                typeof(ISuiMoveNormalizedType),
                                null,
                                serializer
                            ) as ISuiMoveNormalizedType;
                            normalziedType = new SuiMoveNormalizedTypeReference(vector);
                            break;
                        // SuiMoveNormalizedStructType
                        case "Struct":
                            Debug.Log("MARCUS::: STRUCT");
                            JObject structTypeObj = (JObject)typeObj[objType];
                            AccountAddress address = AccountAddress.FromHex(NormalizeSuiAddress((string)structTypeObj["address"]));
                            string module = (string)structTypeObj["module"];
                            string name = (string)structTypeObj["name"];

                            JArray arguments = (JArray)structTypeObj["typeArguments"];
                            List<ISuiMoveNormalizedType> argumentsTypes = new List<ISuiMoveNormalizedType>();
                            foreach (JToken argument in arguments)
                            {
                                var argumentsTypeConverter = new NormalizedTypeConverter();
                                ISuiMoveNormalizedType argumentType = argumentsTypeConverter.ReadJson(
                                    argument.CreateReader(),
                                    typeof(ISuiMoveNormalizedType),
                                    null,
                                    serializer
                                ) as ISuiMoveNormalizedType;
                                argumentsTypes.Add(argumentType);
                            }

                            SuiStructTag structTag = new SuiStructTag(address, module, name, argumentsTypes.ToArray()); // TODO: Marcus: Implement Type Arguemtns parameter
                            normalziedType = new SuiMoveNormalziedTypeStruct(structTag);
                            Debug.Log($"MARCUS:::: STRUCT TAG NAME - {(normalziedType as SuiMoveNormalziedTypeStruct).Struct.name}");
                            break;
                        // String
                        default:
                            throw new ArgumentException();
                    }

                    if (normalziedType != null)
                        return normalziedType;
                }
            }
            catch (Exception e)
            {
                string objType = reader.ToString();
                Debug.Log($"MARCUS:::: {objType}");
                return new SuiMoveNormalizedTypeString(objType);
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
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
            JArray fields = (JArray)structObj["fields"];
            List<SuiMoveNormalizedField> normalizedFields = new List<SuiMoveNormalizedField>();
            foreach (JObject field in fields)
            {
                SuiMoveNormalizedField normalizedField = new SuiMoveNormalizedField();

                normalizedField.Name = (string)field["name"];
                JObject typeObj = (JObject)field["type"];

                NormalizedTypeConverter typeConverter = new NormalizedTypeConverter();
                normalizedField.Type = typeConverter.ReadJson(
                    typeObj.CreateReader(),
                    typeof(ISuiMoveNormalizedType),
                    null,
                    serializer
                ) as ISuiMoveNormalizedType;

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

            if (item["fileFormatVersion"] != null) moveModule.FileFormatVersion = (int)item["fileFormatVersion"];
            if (item["address"] != null) moveModule.Address = (string)item["address"];
            if (item["name"] != null) moveModule.Name = (string)item["name"];
            List<Friends> Friends = new List<Friends>();

            if (item["friends"] != null)
            {
                foreach (JProperty friend in item["friends"])
                    Friends.Add(JsonConvert.DeserializeObject<Friends>((string)friend));
            }

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
            return moveModule;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

