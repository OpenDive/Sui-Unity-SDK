using System.Collections.Generic;
using Newtonsoft.Json;
using OpenDive.BCS;
using System;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Player;

namespace Sui.Rpc.Models
{
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
            if (reader.TokenType != JsonToken.StartObject)
            {
                JObject typeObj = JObject.Load(reader);
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
                            JObject structTypeObj = (JObject)typeObj[objType];
                            AccountAddress address = AccountAddress.FromHex(NormalizeSuiAddress((string)structTypeObj["address"]));
                            string module = (string)structTypeObj["module"];
                            string name = (string)structTypeObj["name"];
                            StructTag structTag = new StructTag(address, module, name, Array.Empty<ISerializableTag>()); // TODO: Marcus: Implement Type Arguemtns parameter
                            normalziedType = new SuiMoveNormalziedTypeStruct(structTag);
                            break;
                        // String
                        default:
                            throw new NotImplementedException();
                    }

                    if (normalziedType != null)
                    {
                        return normalziedType;
                    }
                }
            }
            throw new NotImplementedException();
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
            if(reader.TokenType == JsonToken.StartObject)
            {
                JObject item = JObject.Load(reader);
                item = (JObject)item["result"];
                Debug.Log(item.ToString());
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
                    SuiMoveNormalizedStruct normalizedStruct = new SuiMoveNormalizedStruct();
                    SuiMoveAbilitySet abilitySet = JsonConvert.DeserializeObject<SuiMoveAbilitySet> (structObj["abilities"].ToString());
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

                        // TODO: Marcus: Move this into the NormalizeType Converter, this object shouldn't have to know how to decode NormalizedTypeStrings itself.
                        try
                        {
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
                        catch
                        {
                            string type = (string)field["type"];
                            normalizedField.Type = new SuiMoveNormalizedTypeString(type);
                            normalizedFields.Add(normalizedField);
                        }
                    }
                    normalizedStruct.Abilities = abilitySet;
                    normalizedStruct.TypeParameters = typeParams.ToArray();
                    normalizedStruct.Fields = normalizedFields.ToArray();
                    Structs[property.Name] = normalizedStruct;
                }

                moveModule.Structs = Structs;
                return moveModule;
            }
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

