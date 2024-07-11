using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    public class TypeOrigin
    {
        [JsonProperty("module_name")]
        public string ModuleName { get; set; }

        [JsonProperty("package"), JsonConverter(typeof(AccountAddressConverter))]
        public AccountAddress Package { get; set; }

        [JsonProperty("struct_name")]
        public string StructName { get; set; }
    }
}