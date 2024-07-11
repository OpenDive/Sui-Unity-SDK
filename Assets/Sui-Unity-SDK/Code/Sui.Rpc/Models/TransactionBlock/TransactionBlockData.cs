using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class TransactionBlockData
    {
        [JsonProperty("messageVersion")]
        public string MessageVersion = "v1";

        [JsonConverter(typeof(TransactionBlockKindJsonConverter))]
        [JsonProperty("transaction")]
        public TransactionBlockKind Transaction { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("gasData")]
        public GasData GasData { get; set; }
    }
}