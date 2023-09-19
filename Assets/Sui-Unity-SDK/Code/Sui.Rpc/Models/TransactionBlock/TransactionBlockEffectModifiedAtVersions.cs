using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// <code>
    /// {
    ///     "objectId":"0x7cb7bf705ad0edf9a93f993ba077a5dfd23052c1e059bd51e62a4bce4b3f8378",
    ///     "sequenceNumber":"1"
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class TransactionBlockEffectsModifiedAtVersions
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }
        [JsonProperty("sequenceNumber")]
        public BigInteger SequenceNumber { get; set; }
    }
}