using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    public class UpgradeInfo
    {
        [JsonProperty("upgraded_id"), JsonConverter(typeof(AccountAddressConverter))]
        public AccountAddress UpgradedId { get; set; }

        [JsonProperty("upgraded_version")]
        public BigInteger UpgradedVersion { get; set; }
    }
}