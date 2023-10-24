using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class PastObject
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("details")]
        public PastObjectDetails Details { get; set; }
    }

    [JsonObject]
    public class PastObjectDetails
    {
        [JsonProperty("obectId")]
        public string ObjectID { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("owner")]
        public OwnmerAddress Owner { get; set; }

        [JsonProperty("previousTransaction")]
        public string PreviousTransaction { get; set; }

        [JsonProperty("storageRebate")]
        public string StorageRebate { get; set; }

        [JsonProperty("content")]
        public PastObjectContent Content { get; set; }
    }

    [JsonObject]
    public class OwnmerAddress
    {
        [JsonProperty("AddressOwner")]
        public string AddressOwner { get; set; }
    }

    [JsonObject]
    public class PastObjectContent
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("hasPublicTransfer")]
        public bool HasPublicTransfer { get; set; }

        [JsonProperty("fields")]
        public JObject Fields { get; set; }
    }

    [JsonObject]
    public class PastObjectRequest
    {
        [JsonProperty("objectId")]
        public string ObjectID { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}