using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    public class ObjectReadConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ObjectRead);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                Newtonsoft.Json.Linq.JObject jobject_read = Newtonsoft.Json.Linq.JObject.Load(reader);

                ObjectRead object_read = new ObjectRead();

                switch (jobject_read["status"].ToString())
                {
                    case "VersionFound":
                        object_read.Type = ObjectReadType.VersionFound;
                        object_read.Object = new VersionFound(JsonConvert.DeserializeObject<ObjectData>(jobject_read["details"].ToString()));
                        break;
                    case "ObjectNotExists":
                        object_read.Type = ObjectReadType.ObjectNotExists;
                        object_read.Object = new ObjectNotExists(jobject_read["details"].ToString());
                        break;
                    case "ObjectDeleted":
                        object_read.Type = ObjectReadType.ObjectDeleted;
                        object_read.Object = new ObjectDeleted(JsonConvert.DeserializeObject<ObjectRef>(jobject_read["details"].ToString()));
                        break;
                    case "VersionNotFound":
                        object_read.Type = ObjectReadType.VersionNotFound;
                        JArray not_found_details = (JArray)jobject_read["details"];
                        object_read.Object = new VersionNotFound(
                            new Tuple<string, string>
                            (
                                not_found_details[0].ToString(),
                                not_found_details[1].ToString()
                            )
                        );
                        break;
                    case "VersionTooHigh":
                        object_read.Type = ObjectReadType.VersionTooHigh;
                        object_read.Object = new VersionTooHigh(JsonConvert.DeserializeObject<VersionTooHighDetails>(jobject_read["details"].ToString()));
                        break;
                }

                return object_read;
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public enum ObjectReadType
    {
        VersionFound,
        ObjectNotExists,
        ObjectDeleted,
        VersionNotFound,
        VersionTooHigh
    }

    [JsonConverter(typeof(ObjectReadConverter))]
    public class ObjectRead
    {
        public IObjectRead Object;
        public ObjectReadType Type;

        public ObjectRead(IObjectRead obj, ObjectReadType type)
        {
            this.Object = obj;
            this.Type = type;
        }

        public ObjectRead() { }
    }

    public interface IObjectRead { }

    public class VersionFound: IObjectRead
    {
        public ObjectData Value;

        public VersionFound(ObjectData value)
        {
            this.Value = value;
        }
    }

    public class ObjectNotExists: IObjectRead
    {
        public string Value;

        public ObjectNotExists(string value)
        {
            this.Value = value;
        }
    }

    public class ObjectDeleted : IObjectRead
    {
        public ObjectRef Value;

        public ObjectDeleted(ObjectRef value)
        {
            this.Value = value;
        }
    }

    public class VersionNotFound : IObjectRead
    {
        public Tuple<string, string> Value;

        public VersionNotFound(Tuple<string, string> value)
        {
            this.Value = value;
        }
    }

    public class VersionTooHigh : IObjectRead
    {
        public VersionTooHighDetails Value;

        public VersionTooHigh(VersionTooHighDetails value)
        {
            this.Value = value;
        }
    }

    [JsonObject]
    public class VersionTooHighDetails
    {
        [JsonProperty("askedVersion")]
        public string AskedVersion { get; set; }

        [JsonProperty("latestVersion")]
        public string LastestVersion { get; set; }

        [JsonProperty("objectId")]
        public string ObjectID { get; set; }
    }

    [JsonObject]
    public class PastObjectRequest
    {
        [JsonProperty("objectId")]
        public string ObjectID { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        public PastObjectRequest(string object_id, string version)
        {
            this.ObjectID = object_id;
            this.Version = version;
        }
    }
}