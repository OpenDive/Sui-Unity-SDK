using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(StringEnumConverter), converterParameters: typeof(CamelCaseNamingStrategy))]
    public enum ObjectChangeType
    {
        Published,
        Transferred,
        Mutated,
        Deleted,
        Wrapped,
        Created
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy)), JsonConverter(typeof(ObjectChangeConverter))]
    public abstract class ObjectChange
    {
        [JsonProperty("sender")]
        public string Digest { get; set; }
        [JsonProperty("type")]
        public ObjectChangeType Type { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class PublishedObjectChange : ObjectChange
    {
        public List<string> Modules { get; set; }
        public string PackageId { get; set; }

        public PublishedObjectChange()
        {
            Type = ObjectChangeType.Published;
        }
    }

    public class TransferredObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }

        public TransferredObjectChange()
        {
            Type = ObjectChangeType.Transferred;
        }
    }

    [JsonObject]
    public class MutatedObjectChange : ObjectChange
    {
        [JsonProperty("sender")]
        public string Sender { get; set; }
        [JsonProperty("owner")]
        public Owner Owner { get; set; }
        [JsonProperty("objectType")]
        public string ObjectType { get; set; }
        [JsonProperty("objectId")]
        public ObjectId ObjectId { get; set; }
        [JsonProperty("previousVersion")]
        public BigInteger PreviousVersion { get; set; }

        public MutatedObjectChange()
        {
            Type = ObjectChangeType.Mutated;
        }
    }

    public class DeletedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Sender { get; set; }

        public DeletedObjectChange()
        {
            Type = ObjectChangeType.Deleted;
        }
    }

    public class WrappedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Sender { get; set; }

        public WrappedObjectChange()
        {
            Type = ObjectChangeType.Wrapped;
        }
    }

    public class CreatedObjectChange : ObjectChange
    {
        public ObjectId ObjectId { get; set; }
        public string ObjectType { get; set; }
        public Owner Owner { get; set; }
        public string Sender { get; set; }

        public CreatedObjectChange()
        {
            Type = ObjectChangeType.Created;
        }
    }
}