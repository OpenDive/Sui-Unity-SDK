using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class CoinPage
    {
        [JsonProperty("data")]
        public List<CoinDetails> Data { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }
    }

    [JsonObject]
    public class CoinDetails
    {
        [JsonProperty("coinType")]
        public string CoinType { get; set; }

        [JsonProperty("coinObjectId")]
        public string CoinObjectId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("previousTransaction")]
        public string PreviousTransaction { get; set; }
    }
}