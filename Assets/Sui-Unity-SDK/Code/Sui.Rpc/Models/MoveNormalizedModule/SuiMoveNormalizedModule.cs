using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;
using Sui.Accounts;
using System;

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
        public SuiMoveNormalizedField(string name, ISuiMoveNormalizedType type)
        {
            this.Name = name;
            this.Type = type;
        }

        public SuiMoveNormalizedField()
        {
            this.Name = null;
            this.Type = null;
        }

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
        public ISuiMoveNormalizedType Type { get; set; }
    }

    public interface ISuiMoveNormalizedType { }

    public class SuiMoveNormalizedTypeString: ISuiMoveNormalizedType
    {
        public string Value { get; set; }

        public SuiMoveNormalizedTypeString(string Value)
        {
            this.Value = Value;
        }
    }

    public class SuiMoveNormalziedTypeParameterType : ISuiMoveNormalizedType
    {
        public U16 TypeParameter { get; set; }

        public SuiMoveNormalziedTypeParameterType(U16 TypeParameter)
        {
            this.TypeParameter = TypeParameter;
        }
    }

    public class SuiMoveNormalziedTypeStruct: ISuiMoveNormalizedType
    {
        public SuiStructTag Struct { get; set; }

        public SuiMoveNormalziedTypeStruct(SuiStructTag Struct)
        {
            this.Struct = Struct;
        }
    }

    public class SuiMoveNormalizedTypeVector: ISuiMoveNormalizedType
    {
        public ISuiMoveNormalizedType Vector { get; set; }

        public SuiMoveNormalizedTypeVector(ISuiMoveNormalizedType Vector)
        {
            this.Vector = Vector;
        }
    }

    public class SuiMoveNormalizedTypeReference : ISuiMoveNormalizedType
    {
        public ISuiMoveNormalizedType Reference { get; set; }

        public SuiMoveNormalizedTypeReference(ISuiMoveNormalizedType Reference)
        {
            this.Reference = Reference;
        }
    }

    public class SuiMoveNormalizedTypeMutableReference: ISuiMoveNormalizedType
    {
        public ISuiMoveNormalizedType MutableReference { get; set; }

        public SuiMoveNormalizedTypeMutableReference(ISuiMoveNormalizedType MutableReference)
        {
            this.MutableReference = MutableReference;
        }
    }
}