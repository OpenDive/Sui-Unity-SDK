using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(SuiOwnerConverter))]
    public class Owner
    {
        public SuiOwnerType Type { get; }

        [JsonProperty("AddressOwner", Required = Required.Default)]
        public string Address { get; set; }

        [JsonProperty("Shared", Required = Required.Default)]
        public SharedOwner Shared { get; }

        public Owner()
        {
            Type = SuiOwnerType.Immutable;
        }

        public Owner(SuiOwnerType type, string address)
        {
            Type = type;
            Address = address;
        }

        public Owner(int initial_shared_version)
        {
            Type = SuiOwnerType.Shared;
            Shared = new SharedOwner(initial_shared_version);
        }
    }

    [JsonObject]
    public class SharedOwner
    {
        [JsonProperty("initial_shared_version", Required = Required.Default)]
        public int? InitialSharedVersion { get; }

        public SharedOwner(int initial_shared_version)
        {
            this.InitialSharedVersion = initial_shared_version;
        }
    }
}