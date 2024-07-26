using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Rpc.Client;
using UnityEngine;

namespace Sui.Rpc.Models
{
    public class ObjectReadConverter : JsonConverter
    {
        public readonly string ObjectReadStatusProperty = "status";
        public readonly string ObjectReadDetailsProperty = "details";

        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectRead);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_read = JObject.Load(reader);

                switch (object_read[this.ObjectReadStatusProperty].Value<string>())
                {
                    case "VersionFound":
                        return new ObjectRead
                        (
                            new VersionFound(JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(object_read[this.ObjectReadDetailsProperty]))),
                            ObjectReadType.VersionFound
                        );
                    case "ObjectNotExists":
                        return new ObjectRead
                        (
                            new ObjectNotExists(object_read[this.ObjectReadDetailsProperty].Value<string>()),
                            ObjectReadType.ObjectNotExists
                        );
                    case "ObjectDeleted":
                        return new ObjectRead
                        (
                            new ObjectDeleted(JsonConvert.DeserializeObject<Sui.Types.SuiObjectRef>(JsonConvert.SerializeObject(object_read[this.ObjectReadDetailsProperty]))),
                            ObjectReadType.ObjectDeleted
                        );
                    case "VersionNotFound":
                        JArray not_found_details = (JArray)object_read[this.ObjectReadDetailsProperty];
                        return new ObjectRead
                        (
                            new VersionNotFound
                            (
                                new Tuple<string, string>
                                (
                                    not_found_details[0].ToString(),
                                    not_found_details[1].ToString()
                                )
                            ),
                            ObjectReadType.VersionNotFound
                        );
                    case "VersionTooHigh":
                        return new ObjectRead
                        (
                            new VersionTooHigh(JsonConvert.DeserializeObject<VersionTooHighDetails>(JsonConvert.SerializeObject(object_read[this.ObjectReadDetailsProperty]))),
                            ObjectReadType.VersionTooHigh
                        );
                    default:
                        return new ObjectRead
                        (
                            new SuiError
                            (
                                0,
                                $"Unable to convert {object_read[this.ObjectReadStatusProperty].Value<string>()} to ObjectReadType.",
                                null
                            )
                        );
                }
            }

            return new ObjectRead(new SuiError(0, "Unable to convert JSON to ObjectRead.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                ObjectRead object_read = (ObjectRead)value;

                writer.WriteStartObject();

                writer.WritePropertyName(this.ObjectReadStatusProperty);
                writer.WriteValue(object_read.Type.ToString());

                writer.WritePropertyName(this.ObjectReadDetailsProperty);

                switch (object_read.Type)
                {
                    case ObjectReadType.VersionFound:
                        VersionFound version_found = (VersionFound)object_read.Object;
                        writer.WriteRawValue(JsonConvert.SerializeObject(version_found.Value));
                        break;
                    case ObjectReadType.ObjectNotExists:
                        ObjectNotExists object_not_exists = (ObjectNotExists)object_read.Object;
                        writer.WriteValue(JsonConvert.SerializeObject(object_not_exists.Value));
                        break;
                    case ObjectReadType.ObjectDeleted:
                        ObjectDeleted object_deleted = (ObjectDeleted)object_read.Object;
                        writer.WriteRawValue(JsonConvert.SerializeObject(object_deleted.Value));
                        break;
                    case ObjectReadType.VersionNotFound:
                        VersionNotFound version_not_found = (VersionNotFound)object_read.Object;

                        writer.WriteStartArray();

                        writer.WriteValue(version_not_found.Value.Item1);
                        writer.WriteValue(version_not_found.Value.Item2);

                        writer.WriteEndArray();
                        break;
                    case ObjectReadType.VersionTooHigh:
                        VersionTooHigh version_too_high = (VersionTooHigh)object_read.Object;
                        writer.WriteRawValue(JsonConvert.SerializeObject(version_too_high.Value));
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }

    /// <summary>
    /// The associated value types of an object read request.
    /// </summary>
    public enum ObjectReadType
    {
        VersionFound,
        ObjectNotExists,
        ObjectDeleted,
        VersionNotFound,
        VersionTooHigh
    }

    /// <summary>
    /// `ObjectRead` describes the result of attempting to read an object in the Sui Network.
    /// </summary>
    [JsonConverter(typeof(ObjectReadConverter))]
    public class ObjectRead : ReturnBase
    {
        public IObjectRead Object { get; internal set; }

        public ObjectReadType Type { get; internal set; }

        public ObjectRead(IObjectRead obj, ObjectReadType type)
        {
            this.Object = obj;
            this.Type = type;
        }

        public ObjectRead(SuiError error)
        {
            this.Error = error;
        }
    }

    public interface IObjectRead { }

    /// <summary>
    /// Indicates that the object's version was successfully found.
    /// The associated value is the found `ObjectData`.
    /// </summary>
    public class VersionFound: IObjectRead
    {
        public ObjectData Value { get; internal set; }

        public VersionFound(ObjectData value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Indicates that the object does not exist.
    /// The associated value is a `string` containing the object's identifier.
    /// </summary>
    public class ObjectNotExists: IObjectRead
    {
        public string Value { get; internal set; }

        public ObjectNotExists(string value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Indicates that the object has been deleted.
    /// The associated value is a `SuiObjectRef` reference to the deleted object.
    /// </summary>
    public class ObjectDeleted : IObjectRead
    {
        public Sui.Types.SuiObjectRef Value { get; internal set; }

        public ObjectDeleted(Sui.Types.SuiObjectRef value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Indicates that the requested version of the object was not found.
    /// The associated value is a `string, string` tuple containing the
    /// object ID and the requested version.
    /// </summary>
    public class VersionNotFound : IObjectRead
    {
        public Tuple<string, string> Value { get; internal set; }

        public VersionNotFound(Tuple<string, string> value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Indicates that the requested version is higher than the latest version.
    /// The associated values are `string`s containing the asked version,
    /// the latest version, and the object ID.
    /// </summary>
    public class VersionTooHigh : IObjectRead
    {
        public VersionTooHighDetails Value { get; internal set; }

        public VersionTooHigh(VersionTooHighDetails value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// The details pertraining to a Version Too High return value.
    /// </summary>
    [JsonObject]
    public class VersionTooHighDetails
    {
        [JsonProperty("askedVersion")]
        public string AskedVersion { get; internal set; }

        [JsonProperty("latestVersion")]
        public string LastestVersion { get; internal set; }

        [JsonProperty("objectId")]
        public string ObjectID { get; internal set; }
    }

    /// <summary>
    /// A class representing the query of past object.
    /// </summary>
    [JsonObject]
    public class PastObjects
    {
        /// <summary>
        /// The object ID of the past object.
        /// </summary>
        [JsonProperty("objectId")]
        public string ObjectID { get; internal set; }

        /// <summary>
        /// The version of the past object.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; internal set; }

        internal PastObjects
        (
            string object_id,
            string version
        )
        {
            this.ObjectID = object_id;
            this.Version = version;
        }
    }

    /// <summary>
    /// A class representing the query of past object input.
    /// </summary>
    public class PastObjectsInput
    {
        /// <summary>
        /// The object ID of the past object.
        /// </summary>
        public AccountAddress ObjectID { get; set; }

        /// <summary>
        /// The version of the past object.
        /// </summary>
        public BigInteger Version { get; set; }

        public PastObjectsInput
        (
            AccountAddress object_id,
            BigInteger version
        )
        {
            this.ObjectID = object_id;
            this.Version = version;
        }

        public PastObjects ToQueryObject()
            => new PastObjects(this.ObjectID.KeyHex, this.Version.ToString());
    }
}