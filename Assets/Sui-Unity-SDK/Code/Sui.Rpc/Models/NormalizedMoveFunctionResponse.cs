using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class NormalizedMoveFunctionResponse
    {
        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("isEntry")]
        public bool IsEntry { get; set; }

        [JsonProperty("typeParameters")]
        public List<TypeParameter> TypeParameters { get; set; }

        [JsonProperty("parameters")]
        public List<string> Parameters { get; set; }

        [JsonProperty("return")]
        public List<string> Return { get; set; }
    }

    [JsonObject]
    public class TypeParameter
    {
        [JsonProperty("abilities")]
        public List<string> Abilities { get; set; }
    }
}