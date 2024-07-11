using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Sui.Rpc.Models
{
    public class ObjectChangeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ObjectChange));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            // TODO: Needs to be cleaned up, too much repeated code
            Debug.Log("OBJECT TYPE: " + jo["type"]?.ToObject<ObjectChangeType>());
            Debug.Log($"MARCUS::: OBJECT VALUE - {JsonConvert.SerializeObject(jo)}");
            switch (jo["type"]?.ToObject<ObjectChangeType>())
            {
                case ObjectChangeType.Published:
                    return new PublishedObjectChange
                    {
                        Digest = jo["digest"].Value<string>(),
                        Type = ObjectChangeType.Published,
                        Version = jo["version"].Value<string>(),
                        Modules = jo["modules"]?.ToObject<List<string>>(serializer),
                        PackageId = jo["packageId"].Value<string>()
                    };
                case ObjectChangeType.Transferred:
                    return new TransferredObjectChange
                    {
                        Digest = jo["digest"].Value<string>(),
                        Type = ObjectChangeType.Transferred,
                        Version = jo["version"].Value<string>(),
                        ObjectId = jo["objectId"].ToObject<ObjectId>(serializer),
                        ObjectType = jo["objectType"].Value<string>(),
                        Recipient = jo["recipient"].Value<string>(),
                        Sender = jo["sender"].Value<string>()
                    };
                case ObjectChangeType.Mutated:
                    Debug.Log("MUTATED CASE TRIGGERED: " + jo["sender"].Value<string>());
                    return new MutatedObjectChange
                    {
                        Digest = jo["digest"].Value<string>(),
                        Type = ObjectChangeType.Mutated,
                        Version = jo["version"].Value<string>(),
                        ObjectId = jo["objectId"].ToObject<ObjectId>(serializer),
                        ObjectType = jo["objectType"].Value<string>(),
                        Owner = jo["owner"].ToObject<Owner>(serializer),
                        PreviousVersion = BigInteger.Parse(jo["previousVersion"].Value<string>()),
                        Sender = jo["sender"].Value<string>()
                    };
                case ObjectChangeType.Deleted:
                    return new DeletedObjectChange
                    {
                        Type = ObjectChangeType.Deleted,
                        Version = jo["version"].Value<string>(),
                        ObjectId = new ObjectId(jo["objectId"].Value<string>()),
                        ObjectType = jo["objectType"].Value<string>(),
                        Sender = jo["sender"].Value<string>()
                    };
                case ObjectChangeType.Wrapped:
                    return new WrappedObjectChange
                    {
                        Digest = jo["digest"].Value<string>(),
                        Type = ObjectChangeType.Wrapped,
                        Version = jo["version"].Value<string>(),
                        ObjectId = jo["objectId"].ToObject<ObjectId>(serializer),
                        ObjectType = jo["objectType"].Value<string>(),
                        Sender = jo["sender"].Value<string>()
                    };
                case ObjectChangeType.Created:
                    return new CreatedObjectChange
                    {
                        Digest = jo["digest"].Value<string>(),
                        Type = ObjectChangeType.Created,
                        Version = jo["version"].Value<string>(),
                        ObjectId = jo["objectId"].ToObject<ObjectId>(serializer),
                        ObjectType = jo["objectType"].Value<string>(),
                        Owner = jo["owner"].ToObject<Owner>(serializer),
                        Sender = jo["sender"].Value<string>()
                    };
                default:
                    throw new InvalidOperationException("Type field not found.");
            }
        }

        // TODO: This is Irvin's implementation
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            Type type = value.GetType();

            foreach(PropertyInfo prop in type.GetProperties())
            {
                object propVal = prop.GetValue(value, null);
                if (propVal != null)
                {
                    // Create CamelCase
                    string camelCaseProp = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);
                    jo.Add(camelCaseProp, JToken.FromObject(propVal, serializer));
                }
            }


            jo.WriteTo(writer);

            //DefaultContractResolver contractResolver = new DefaultContractResolver
            //{
            //    NamingStrategy = new CamelCaseNamingStrategy()
            //};

            //var jsonSerializerSettings = new JsonSerializerSettings
            //{
            //    //ContractResolver = contractResolver
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};

            //string serialized = JsonConvert.SerializeObject(jo, jsonSerializerSettings);
            //Debug.Log("TEST :::: " + serialized);

            //writer.WriteRaw(serialized);

            //throw new NotImplementedException();
        }
    }
}