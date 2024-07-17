using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;
using Sui.Accounts;
using System;
using Sui.Rpc.Client;

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

        [JsonProperty("exposedFunctions")]
        public Dictionary<string, NormalizedMoveFunctionResponse> ExposedFunctions { get; set; }
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
        public SuiMoveNormalizedField(string name, SuiMoveNormalizedType type)
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
        public SuiMoveNormalizedType Type { get; set; }
    }

    public interface ISuiMoveNormalizedType: ISerializable { }

    public enum SuiMoveNormalizedTypeSerializationType
    {
        String,
        TypeParameter,
        Struct,
        Vector,
        Reference,
        MutableReference
    }

    [JsonConverter(typeof(NormalizedTypeConverter))]
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

        public static SuiMoveNormalizedType FromStr(string value)
        {
            List<string> allowed_types = new List<string>() { "Address", "Bool", "U8", "U16", "U32", "U64", "U128", "U256", "Signer" };

            if (allowed_types.Exists(str => str == value) == false)
                return null;

            return new SuiMoveNormalizedType
            (
                new SuiMoveNormalizedTypeString(value),
                SuiMoveNormalizedTypeSerializationType.String
            );
        }

        public void Serialize(Serialization serializer)
        {
            switch(this.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.String:
                    ((SuiMoveNormalizedTypeString)this.NormalizedType).Serialize(serializer);
                    break;
                case SuiMoveNormalizedTypeSerializationType.TypeParameter:
                    serializer.SerializeU8(9);
                    serializer.Serialize((SuiMoveNormalziedTypeParameterType)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Reference:
                    serializer.SerializeU8(10);
                    serializer.Serialize((SuiMoveNormalizedTypeReference)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.MutableReference:
                    serializer.SerializeU8(11);
                    serializer.Serialize((SuiMoveNormalizedTypeMutableReference)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Vector:
                    serializer.SerializeU8(12);
                    serializer.Serialize((SuiMoveNormalizedTypeVector)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Struct:
                    serializer.SerializeU8(13);
                    serializer.Serialize((SuiMoveNormalziedTypeStruct)this.NormalizedType);
                    break;
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8();
            ISuiMoveNormalizedType normalized_type;
            SuiMoveNormalizedTypeSerializationType type_value;

            switch (type)
            {
                case 0:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("Bool");
                    break;
                case 1:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U8");
                    break;
                case 2:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U16");
                    break;
                case 3:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U32");
                    break;
                case 4:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U64");
                    break;
                case 5:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U128");
                    break;
                case 6:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U256");
                    break;
                case 7:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("Address");
                    break;
                case 8:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("Signer");
                    break;
                case 9:
                    type_value = SuiMoveNormalizedTypeSerializationType.TypeParameter;
                    normalized_type = (SuiMoveNormalziedTypeParameterType)SuiMoveNormalziedTypeParameterType.Deserialize(deserializer);
                    break;
                case 10:
                    type_value = SuiMoveNormalizedTypeSerializationType.Reference;
                    normalized_type = (SuiMoveNormalizedTypeReference)SuiMoveNormalizedTypeReference.Deserialize(deserializer);
                    break;
                case 11:
                    type_value = SuiMoveNormalizedTypeSerializationType.MutableReference;
                    normalized_type = (SuiMoveNormalizedTypeMutableReference)SuiMoveNormalizedTypeMutableReference.Deserialize(deserializer);
                    break;
                case 12:
                    type_value = SuiMoveNormalizedTypeSerializationType.Vector;
                    normalized_type = (SuiMoveNormalizedTypeVector)SuiMoveNormalizedTypeVector.Deserialize(deserializer);
                    break;
                case 13:
                    type_value = SuiMoveNormalizedTypeSerializationType.Struct;
                    normalized_type = (SuiMoveNormalziedTypeStruct)SuiMoveNormalziedTypeStruct.Deserialize(deserializer);
                    break;
                default:
                    return new SuiError(0, "Unable to deserialize SuiMoveNormalizedType", null);
            }

            return new SuiMoveNormalizedType(normalized_type, type_value);
        }
    }

    public class SuiMoveNormalizedTypeString: ISuiMoveNormalizedType
    {
        public string Value { get; set; }

        public SuiMoveNormalizedTypeString(string Value)
        {
            this.Value = Value;
        }

        public void Serialize(Serialization serializer)
        {
            switch (this.Value)
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
                    throw new Exception($"Unable to serialize type - {this.Value}");
            }
        }
    }

    public class SuiMoveNormalziedTypeParameterType : ISuiMoveNormalizedType
    {
        public ushort TypeParameter { get; set; }

        public SuiMoveNormalziedTypeParameterType(ushort TypeParameter)
        {
            this.TypeParameter = TypeParameter;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU16(this.TypeParameter);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalziedTypeParameterType(deserializer.DeserializeU16());
        }
    }

    public class SuiMoveNormalziedTypeStruct: ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedStructType Struct { get; set; }

        public SuiMoveNormalziedTypeStruct(SuiMoveNormalizedStructType Struct)
        {
            this.Struct = Struct;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Struct);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalziedTypeStruct((SuiMoveNormalizedStructType)SuiMoveNormalizedStructType.Deserialize(deserializer));
        }
    }

    public class SuiMoveNormalizedTypeVector: ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType Vector { get; set; }

        public SuiMoveNormalizedTypeVector(SuiMoveNormalizedType vector)
        {
            this.Vector = vector;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Vector);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalizedTypeVector((SuiMoveNormalizedType)SuiMoveNormalizedType.Deserialize(deserializer));
        }
    }

    public class SuiMoveNormalizedTypeReference : ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType Reference { get; set; }

        public SuiMoveNormalizedTypeReference(SuiMoveNormalizedType reference)
        {
            this.Reference = reference;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Reference);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalizedTypeVector((SuiMoveNormalizedType)SuiMoveNormalizedType.Deserialize(deserializer));
        }
    }

    public class SuiMoveNormalizedTypeMutableReference: ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType MutableReference { get; set; }

        public SuiMoveNormalizedTypeMutableReference(SuiMoveNormalizedType mutable_reference)
        {
            this.MutableReference = mutable_reference;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.MutableReference);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalizedTypeMutableReference((SuiMoveNormalizedType)SuiMoveNormalizedType.Deserialize(deserializer));
        }
    }
}