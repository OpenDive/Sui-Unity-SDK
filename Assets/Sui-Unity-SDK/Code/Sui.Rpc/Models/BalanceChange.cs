using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class BalanceChange
    {
        public string Amount { get; set; }

        public string CoinType { get; set; }

        public Owner Owner { get; set; }
    }
}