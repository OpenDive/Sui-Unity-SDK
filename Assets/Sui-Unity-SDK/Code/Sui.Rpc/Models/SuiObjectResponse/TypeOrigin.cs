using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    public class TypeOrigin
    {
        [JsonProperty("module_name")]
        public string ModuleName { get; set; }

        [JsonProperty("package")]
        public ObjectId Package { get; set; }

        [JsonProperty("struct_name")]
        public string StructName { get; set; }
    }
}