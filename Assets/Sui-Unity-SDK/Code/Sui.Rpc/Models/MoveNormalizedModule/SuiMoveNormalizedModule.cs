using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;
using Sui.Accounts;
using System;

namespace Sui.Rpc.Models
{
    [JsonObject, JsonConverter(typeof(NormalizedMoveModuleConverter))]
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

    [JsonObject, JsonConverter(typeof(MoveStructConverter))]
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

    public enum SuiMoveNormalizedTypeSerializationType
    {
        String,
        TypeParameter,
        Struct,
        Vector,
        Reference,
        MutableReference
    }

    public class SuiMoveNormalizedType: ISerializable
    {
        public ISuiMoveNormalizedType NormalizedType;
        public SuiMoveNormalizedTypeSerializationType Type;

        public SuiMoveNormalizedType
        (
            ISuiMoveNormalizedType normalized_type,
            SuiMoveNormalizedTypeSerializationType type
        )
        {
            this.NormalizedType = normalized_type;
            this.Type = type;
        }

        public void Serialize(Serialization serializer)
        {
            switch(this.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.String:
                    switch(((SuiMoveNormalizedTypeString)this.NormalizedType).Value)
                    {
                        case "Bool":
                            serializer.SerializeU8(0);
                            break;
                        case "U8":
                            serializer.SerializeU8(1);
                            break;
                        case "U16":
                            serializer.SerializeU8(2);
                            break;
                        case "U32":
                            serializer.SerializeU8(3);
                            break;
                        case "U64":
                            serializer.SerializeU8(4);
                            break;
                        case "U128":
                            serializer.SerializeU8(5);
                            break;
                        case "U256":
                            serializer.SerializeU8(6);
                            break;
                        case "Address":
                            serializer.SerializeU8(7);
                            break;
                        case "Signer":
                            serializer.SerializeU8(8);
                            break;
                        default:
                            throw new Exception($"Unable to serialize type - {((SuiMoveNormalizedTypeString)this.NormalizedType).Value}");
                    }
                    break;
                case SuiMoveNormalizedTypeSerializationType.TypeParameter:
                    SuiMoveNormalziedTypeParameterType type_parameter = (SuiMoveNormalziedTypeParameterType)this.NormalizedType;
                    serializer.SerializeU8(9);
                    serializer.SerializeU16((ushort)type_parameter.TypeParameter.value);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Reference:
                    SuiMoveNormalizedTypeReference reference = (SuiMoveNormalizedTypeReference)this.NormalizedType;
                    serializer.SerializeU8(10);
                    serializer.Serialize(reference.Reference);
                    break;
                case SuiMoveNormalizedTypeSerializationType.MutableReference:
                    SuiMoveNormalizedTypeMutableReference mutable_reference = (SuiMoveNormalizedTypeMutableReference)this.NormalizedType;
                    serializer.SerializeU8(11);
                    serializer.Serialize(mutable_reference.MutableReference);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Vector:
                    SuiMoveNormalizedTypeVector vector = (SuiMoveNormalizedTypeVector)this.NormalizedType;
                    serializer.SerializeU8(12);
                    serializer.Serialize(vector.Vector);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Struct:
                    SuiMoveNormalziedTypeStruct type_struct = (SuiMoveNormalziedTypeStruct)this.NormalizedType;
                    serializer.SerializeU8(13);
                    serializer.Serialize(type_struct.Struct);
                    break;
            }
        }
    }

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
        public SuiMoveNormalizedStructType Struct { get; set; }

        public SuiMoveNormalziedTypeStruct(SuiMoveNormalizedStructType Struct)
        {
            this.Struct = Struct;
        }
    }

    public class SuiMoveNormalizedTypeVector: ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType Vector { get; set; }

        public SuiMoveNormalizedTypeVector(SuiMoveNormalizedType Vector)
        {
            this.Vector = Vector;
        }
    }

    public class SuiMoveNormalizedTypeReference : ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType Reference { get; set; }

        public SuiMoveNormalizedTypeReference(SuiMoveNormalizedType Reference)
        {
            this.Reference = Reference;
        }
    }

    public class SuiMoveNormalizedTypeMutableReference: ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType MutableReference { get; set; }

        public SuiMoveNormalizedTypeMutableReference(SuiMoveNormalizedType MutableReference)
        {
            this.MutableReference = MutableReference;
        }
    }
}