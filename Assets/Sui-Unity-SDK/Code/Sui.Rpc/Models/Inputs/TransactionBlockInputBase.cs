using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public abstract class TransactionBlockInputBase
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}