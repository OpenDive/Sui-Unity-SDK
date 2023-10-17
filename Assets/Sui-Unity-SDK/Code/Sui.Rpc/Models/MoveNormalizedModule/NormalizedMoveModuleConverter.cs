using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;
using System;
using Newtonsoft.Json.Linq;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
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

                SuiMoveNormalizedModule moveModule = new SuiMoveNormalizedModule();
                Dictionary<string, SuiMoveNormalizedStruct> Structs = new Dictionary<string, SuiMoveNormalizedStruct>();

                if (item["fileFormatVersion"] != null) moveModule.FileFormatVersion = (int)item["fileFormatVersion"];
                if (item["address"] != null) moveModule.Address = AccountAddress.FromHex((string)item["address"]);
                if (item["name"] != null) moveModule.Name = (string)item["name"];
                // TODO: Check if this works. We might need to additional logic to convert array to list of Objects
                Friends[] Friends = JsonConvert.DeserializeObject<Friends[]>((string)item["friends"]);
                moveModule.Friends = Friends;

                //JArray structArr = (JArray)item["structs"];
                //foreach(JObject structObj in structArr)
                //{

                //}

                JObject structs = (JObject)item["structs"];
                //List<JProperty> props = (List<JProperty>)strutcs.Properties();
                foreach (JProperty property in structs.Properties())
                {
                    //Console.WriteLine(property.Name + " - " + property.Value);
                    JObject structObj = (JObject)structs[property.Name];
                    SuiMoveNormalizedStruct normalizedStruct = new SuiMoveNormalizedStruct();
                    SuiMoveAbilitySet abilitySet = JsonConvert.DeserializeObject<SuiMoveAbilitySet> ((string)structObj["abilities"]);
                    // TODO: Revisit typeParams and use corresponding implementation
                    SuiMoveStructTypeParameter typeParams = JsonConvert.DeserializeObject<SuiMoveStructTypeParameter>((string)structObj["typeParameters"]);

                    JArray fields = (JArray)structObj["fields"];
                    List<SuiMoveNormalizedField> normalizedFields = new List<SuiMoveNormalizedField>();

                    foreach (JObject field in fields)
                    {
                        SuiMoveNormalizedField normalizedField = new SuiMoveNormalizedField();
                        normalizedField.Name = (string)field["name"];

                        // TODO: Add logic to check type and create corresponding object
                        JObject typeObj = (JObject)field["type"];

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
                                case "Struct":
                                    //StructTag structTag = JsonConvert.DeserializeObject<StructTag>((string)typeObj[objType]);
                                    JObject structTypeObj = (JObject)typeObj[objType];
                                    AccountAddress address = AccountAddress.FromHex((string)structTypeObj["address"]);
                                    string module = (string)structTypeObj["module"];
                                    string name = (string)structTypeObj["name"];
                                    ISerializableTag[] typeArgs = new ISerializableTag[0]; // TODO: Implement this
                                    StructTag structTag = new StructTag(address, module, name, typeArgs);

                                    normalizedField.Type = structTag;
                                    break;
                                case "lorem":
                                    break;
                                default:
                                    break;
                            }
                                

                        }
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
}

