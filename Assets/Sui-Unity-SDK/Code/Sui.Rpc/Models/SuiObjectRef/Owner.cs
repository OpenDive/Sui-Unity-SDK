using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(SuiOwnerConverter))]
    public class Owner
    {
        public SuiOwnerType Type { get; }

        public string Address { get; set; }

        [JsonProperty("Shared", Required = Required.Default)]
        public SharedOwner Shared { get; }

        public Owner(SuiOwnerType type)
        {
            Type = type;
        }

        public Owner(SuiOwnerType type, string address)
        {
            Type = type;
            Address = address;
        }
    }

    [JsonObject]
    public class SharedOwner
    {
        [JsonProperty("initial_shared_version", Required = Required.Default)]
        public int? InitialSharedVersion { get; }
    }
}