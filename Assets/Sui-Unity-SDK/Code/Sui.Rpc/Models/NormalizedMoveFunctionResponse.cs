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

        [JsonProperty("parameters"), JsonConverter(typeof(NormalizedTypesConverter))]
        public List<ISuiMoveNormalizedType> Parameters { get; set; }

        [JsonProperty("return"), JsonConverter(typeof(NormalizedTypesConverter))]
        public List<ISuiMoveNormalizedType> Return { get; set; }
    }

    [JsonObject]
    public class TypeParameter
    {
        [JsonProperty("abilities")]
        public List<string> Abilities { get; set; }
    }

    [JsonObject]
    public class NormalizedTest
    {
        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("isEntry")]
        public bool IsEntry { get; set; }

        //[JsonProperty("typeParameters")]
        //public List<TypeParameter> TypeParameters { get; set; }

        //[JsonProperty("parameters"), JsonConverter(typeof(NormalizedTypesConverter))]
        //public List<ISuiMoveNormalizedType> Parameters { get; set; }

        //[JsonProperty("return"), JsonConverter(typeof(NormalizedTypesConverter))]
        //public List<ISuiMoveNormalizedType> Return { get; set; }
    }
}