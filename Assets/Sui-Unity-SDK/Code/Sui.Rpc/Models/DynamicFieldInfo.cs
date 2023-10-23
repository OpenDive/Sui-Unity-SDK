using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class DynamicFieldPage
    {
        [JsonProperty("data")]
        public DynamicFieldInfo[] Data { get; set; }

        [JsonProperty("hasNextCursor", Required = Required.Default)]
        public string HasNextCursor { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }
    }

    [JsonObject]
    public class DynamicFieldInfo
    {
        [JsonProperty("name")]
        public DynamicFieldName Name { get; set; }

        [JsonProperty("bcsName")]
        public string BCSName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("objectId")]
        public string ObjectID { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }
    }

    [JsonObject]
    public class DynamicFieldName
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}