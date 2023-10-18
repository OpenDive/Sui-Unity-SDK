using Newtonsoft.Json;
using System.Numerics;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "value": "12023692"
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class TotalSupply
    {
        [JsonProperty("value")]
        public BigInteger Value;
    }
}