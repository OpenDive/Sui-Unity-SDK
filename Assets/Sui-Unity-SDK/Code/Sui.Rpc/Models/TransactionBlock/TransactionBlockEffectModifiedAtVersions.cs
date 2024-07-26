using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The version that the transaction effects took place.
    /// </summary>
    [JsonObject]
    public class TransactionBlockEffectsModifiedAtVersions
    {
        /// <summary>
        /// Represents the ID of the object.
        /// </summary>
        [JsonProperty("objectId")]
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// Represents the sequence number of the object.
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public BigInteger SequenceNumber { get; internal set; }
    }
}