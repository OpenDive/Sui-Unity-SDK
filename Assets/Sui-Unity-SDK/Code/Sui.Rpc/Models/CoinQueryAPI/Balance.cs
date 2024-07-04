using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using OpenDive.BCS;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "coinType": "0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC",
    ///         "coinObjectCount": 15,
    ///         "totalBalance": "15",
    ///         "lockedBalance": {}
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class Balance
    {
        //[JsonProperty("coinType"), JsonConverter(typeof(SuiStructTagConverter))]  // TODO: Look into proper FromString() method for SuiStructTags
        [JsonProperty("coinType")]
        public string cointType;

        [JsonProperty("coinObjectCount")]
        public int CoinObjectCount;

        [JsonProperty("totalBalance")]
        public BigInteger TotalBalance;

        // TODO: Find examples wher "lockedBalance is populated
        [JsonProperty("lockedBalance")]
        public Dictionary<string, BigInteger> LockedBalance;
    }
}