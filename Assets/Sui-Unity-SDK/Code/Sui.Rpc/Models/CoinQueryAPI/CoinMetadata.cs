
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "id": {
    ///             "id": "0x6d907beaa3a49db57bdfdb3557e6d405cbf01c293a53e01457d65e92b5d8dd68"
    ///         },
    ///         "decimals": 9,
    ///         "name": "Usdc",
    ///         "symbol": "USDC",
    ///         "description": "Stable coin.",
    ///         "icon_url": null
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class CoinMetadata
    {
        [JsonProperty("id")]
        public IdObj Id;

        [JsonProperty("decimals")]
        public BigInteger Decimals;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("symbol")]
        public string Symbol;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("icon_url", NullValueHandling = NullValueHandling.Include)]
        public string IconUrl;

        public class IdObj
        {
            public string Id;
        }
    }
}
