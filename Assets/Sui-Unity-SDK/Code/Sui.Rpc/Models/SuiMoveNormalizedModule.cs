using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class SuiMoveNormalizedModule
	{
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fileFormatVersion")]
        public int FileFormatVersion { get; set; }

        [JsonProperty("friends")]
        public Friends[] Friends { get; set; }

        [JsonProperty("structs")]
        public Dictionary<string, SuiMoveNormalizedStruct> Structs { get; set; }
    }

    [JsonObject]
    public class Friends
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    [JsonObject]
    public class SuiMoveNormalizedStruct
    {
        [JsonProperty("abilities")]
        public SuiMoveAbilitySet Abilities { get; set; }

        [JsonProperty("typeParameters")]
        public SuiMoveStructTypeParameter[] TypeParameters { get; set; }

        [JsonProperty("fields")]
        public SuiMoveNormalizedField[] Fields { get; set; }
    }

    [JsonObject]
    public class SuiMoveAbilitySet
    {
        [JsonProperty("abilities")]
        public string[] Abilities { get; set; }
    }

    [JsonObject]
    public class SuiMoveStructTypeParameter
    {
        [JsonProperty("constraints")]
        public SuiMoveAbilitySet Constraints { get; set; }

        [JsonProperty("isPhantom")]
        public bool IsPhantom { get; set; }
    }

    [JsonObject]
    public class SuiMoveNormalizedField
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        // Types that are used:
        // - String
        // - SuiMoveNormalizedTypeParameterType
        // - Reference: SuiMoveNormalizedType
        // - MutableReference: SuiMoveNormalizedType
        // - Vector: SuiMoveNormalizedType
        // - SuiMoveNormalizedStructType
        [JsonProperty("type")]
        public ISerializable[] Type { get; set; }
    }
}