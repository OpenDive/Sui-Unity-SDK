using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class BalanceChange
    {
        [JsonProperty("amount")]
        public BigInteger Amount { get; set; }

        [JsonProperty("coinType")]
        public string CoinType { get; set; }

        [JsonProperty("owner")]
        public Owner Owner { get; set; }
    }
}