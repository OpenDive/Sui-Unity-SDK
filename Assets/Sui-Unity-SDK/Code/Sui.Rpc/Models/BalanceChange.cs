using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenDive.BCS;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class BalanceChange
    {
        [JsonProperty("amount")]
        public BigInteger Amount { get; internal set; }

        [JsonProperty("coinType")]
        public SuiStructTag CoinType { get; internal set; }

        [JsonProperty("owner")]
        public Owner Owner { get; internal set; }
    }
}