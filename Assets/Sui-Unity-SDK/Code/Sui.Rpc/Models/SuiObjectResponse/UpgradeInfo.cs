using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    public class UpgradeInfo
    {
        [JsonProperty("upgraded_id")]
        public ObjectId UpgradedId { get; set; }

        [JsonProperty("upgraded_version")]
        public BigInteger UpgradedVersion { get; set; }
    }
}