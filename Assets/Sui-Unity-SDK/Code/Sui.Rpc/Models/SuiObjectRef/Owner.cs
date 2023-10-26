using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc.Models
{
    [JsonConverter(typeof(SuiOwnerConverter))]
    //[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy)), ]
    public class Owner
    {
        public SuiOwnerType Type { get; } // TODO: Fix this perhaps we should return enum

        [JsonProperty("AddressOwner", Required = Required.Default)] // TODO: Ask Mysten Labs about type
        public string Address { get; }

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
        [JsonProperty("initial_shared_version", Required = Required.Default)] // TODO: Ask Mysten Labs about type
        public int? InitialSharedVersion { get; }
    }
}