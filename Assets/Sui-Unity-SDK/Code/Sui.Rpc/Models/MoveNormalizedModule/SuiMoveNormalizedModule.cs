using System.Collections.Generic;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;
using System;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a normalized Sui Move Module, containing various details about the module including its address,
    /// name, friend modules, structs, and exposed functions.
    /// </summary>
    [JsonObject]
    public class SuiMoveNormalizedModule
	{
        /// <summary>
        /// The address where the module is located.
        /// </summary>
        [JsonProperty("address")]
        public AccountAddress Address { get; internal set; }

        /// <summary>
        /// The name of the module.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Specifies the file format version of the module.
        /// </summary>
        [JsonProperty("fileFormatVersion")]
        public int FileFormatVersion { get; internal set; }

        /// <summary>
        /// A list of `Friend` representing the friend modules of this module.
        /// </summary>
        [JsonProperty("friends")]
        public Friend[] Friends { get; internal set; }

        /// <summary>
        /// A dictionary containing the structs present in the module, where the
        /// key is the name of the struct and the value is an instance of `SuiMoveNormalizedStruct`.
        /// </summary>
        [JsonProperty("structs")]
        public Dictionary<string, SuiMoveNormalizedStruct> Structs { get; internal set; }

        /// <summary>
        /// A dictionary containing the exposed functions of the module,
        /// where the key is the name of the function and the value is an instance of `SuiMoveNormalizedFunction`.
        /// </summary>
        [JsonProperty("exposedFunctions")]
        public Dictionary<string, NormalizedMoveFunctionResponse> ExposedFunctions { get; internal set; }
    }

    /// <summary>
    /// Represents the unique identifier for a Sui Move module.
    /// </summary>
    [JsonObject]
    public class Friend
    {
        /// <summary>
        /// Holds the address of the SuiMove module, serving as a part of its unique identifier.
        /// </summary>
        [JsonProperty("address")]
        public AccountAddress Address { get; internal set; }

        /// <summary>
        /// Holds the name of the Sui Move module, serving as a part of its unique identifier.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }
    }

    /// <summary>
    /// Represents a normalized Sui Move Struct, containing details about the struct including its abilities,
    /// type parameters, and fields.
    /// </summary>
    [JsonObject]
    public class SuiMoveNormalizedStruct
    {
        /// <summary>
        /// A `SuiMoveAbilitySet` representing the abilities of the struct.
        /// </summary>
        [JsonProperty("abilities")]
        public SuiMoveAbilitySet Abilities { get; internal set; }

        /// <summary>
        /// A list of `SuiMoveStructTypeParameter` representing the type parameters of the struct.
        /// </summary>
        [JsonProperty("typeParameters")]
        public SuiMoveStructTypeParameter[] TypeParameters { get; internal set; }

        /// <summary>
        /// A list of `SuiMoveNormalizedField` representing the fields within the struct.
        /// </summary>
        [JsonProperty("fields")]
        public SuiMoveNormalizedField[] Fields { get; internal set; }
    }

    /// <summary>
    /// Represents the set of abilities of a Sui Move in the ecosystem.
    /// </summary>
    [JsonObject]
    public class SuiMoveAbilitySet
    {
        /// <summary>
        /// Holds the abilities of the Sui Move.
        /// </summary>
        [JsonProperty("abilities")]
        public string[] Abilities { get; internal set; }
    }

    /// <summary>
    /// Represents a type parameter in a Sui Move structure, along with
    /// its constraints and whether it's a phantom type parameter.
    /// </summary>
    [JsonObject]
    public class SuiMoveStructTypeParameter
    {
        /// <summary>
        /// A `SuiMoveAbilitySet` representing the constraints on the
        /// type parameter in the Sui Move structure.
        /// </summary>
        [JsonProperty("constraints")]
        public SuiMoveAbilitySet Constraints { get; internal set; }

        /// <summary>
        /// A `Bool` indicating whether the type parameter is a
        /// phantom type parameter in the Sui Move structure.
        /// </summary>
        [JsonProperty("isPhantom")]
        public bool IsPhantom { get; internal set; }
    }

    /// <summary>
    /// Represents a normalized field within a Sui Move structure or contract.
    /// </summary>
    [JsonObject]
    public class SuiMoveNormalizedField
    {
        /// <summary>
        /// Holds the name of the field, serving as an identifier within its containing entity.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Represents the type of the field, defining what kind of data or structure it can hold.
        /// </summary>
        [JsonProperty("type")]
        public SuiMoveNormalizedType Type { get; internal set; }
    }

    /// <summary>
    /// <para>An abstract class representing the various types of Normalized Move Types</para>
    /// <para>- String: A type that can be an unsigned integer, boolean, or address.</para>
    /// <para>- Type Parameter: A type that nests its parameter as an index.</para>
    /// <para>- Struct: A type that represents a Move structure.</para>
    /// <para>- Vector: A type that represents a Move vector.</para>
    /// <para>- Referece: A type that contains a call-by value (read only) to another type.</para>
    /// <para>- Mutable Reference: A type that contains a call-by reference (read and write) to another type.</para>
    /// </summary>
    public abstract class ISuiMoveNormalizedType : ReturnBase, ISerializable
    {
        public virtual void Serialize(Serialization serializer) => throw new NotImplementedException();
    }

    /// <summary>
    /// The serialization type of the Normalized Type.
    /// </summary>
    public enum SuiMoveNormalizedTypeSerializationType
    {
        String,
        TypeParameter,
        Struct,
        Vector,
        Reference,
        MutableReference
    }

    /// <summary>
    /// A class representing the Normalized Type of the given parameter.
    /// </summary>
    [JsonConverter(typeof(NormalizedTypeConverter))]
    public class SuiMoveNormalizedType : ReturnBase, ISerializable
    {
        /// <summary>
        /// The Normalized Type's value.
        /// </summary>
        public ISuiMoveNormalizedType NormalizedType { get; internal set; }

        /// <summary>
        /// The type of Normalized Type.
        /// </summary>
        public SuiMoveNormalizedTypeSerializationType Type { get; internal set; }

        public SuiMoveNormalizedType
        (
            ISuiMoveNormalizedType normalized_type,
            SuiMoveNormalizedTypeSerializationType type
        )
        {
            this.NormalizedType = normalized_type;
            this.Type = type;
        }

        public SuiMoveNormalizedType(SuiError error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Converts a string value to a Normalized Type.
        /// </summary>
        /// <param name="value">The normalized value as a string.</param>
        /// <returns>A `SuiMoveNormalziedType` value, or null if it's not in the allowed types.</returns>
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
            byte type = deserializer.DeserializeU8().Value;

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
                    return new SuiError(0, "Unable to deserialize SuiMoveNormalizedType.", null);
            }

            return new SuiMoveNormalizedType(normalized_type, type_value);
        }
    }

    /// <summary>
    /// Represents an unsigned integer, boolean, or address.
    /// </summary>
    public class SuiMoveNormalizedTypeString : ISuiMoveNormalizedType
    {
        public string Value { get; internal set; }

        public SuiMoveNormalizedTypeString(string Value)
        {
            this.Value = Value;
        }

        public override void Serialize(Serialization serializer)
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
                    this.SetError<SuiError>("Invalid value", this.Value);
                    break;
            }
        }
    }

    /// <summary>
    /// Represents a Type Parameter.
    /// </summary>
    public class SuiMoveNormalziedTypeParameterType : ISuiMoveNormalizedType
    {
        public ushort TypeParameter { get; internal set; }

        public SuiMoveNormalziedTypeParameterType(ushort type_parameter)
        {
            this.TypeParameter = type_parameter;
        }

        public override void Serialize(Serialization serializer)
            => serializer.SerializeU16(this.TypeParameter);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SuiMoveNormalziedTypeParameterType(deserializer.DeserializeU16().Value);
    }

    /// <summary>
    /// Represents a Move structure.
    /// </summary>
    public class SuiMoveNormalziedTypeStruct : ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedStructType Struct { get; internal set; }

        public SuiMoveNormalziedTypeStruct(SuiMoveNormalizedStructType structure)
        {
            this.Struct = structure;
        }

        public override void Serialize(Serialization serializer)
            => serializer.Serialize(this.Struct);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SuiMoveNormalziedTypeStruct
               (
                   (SuiMoveNormalizedStructType)SuiMoveNormalizedStructType.Deserialize(deserializer)
               );
    }

    /// <summary>
    /// Represents a Move vector.
    /// </summary>
    public class SuiMoveNormalizedTypeVector : ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType Vector { get; internal set; }

        public SuiMoveNormalizedTypeVector(SuiMoveNormalizedType vector)
        {
            this.Vector = vector;
        }

        public override void Serialize(Serialization serializer)
            => serializer.Serialize(this.Vector);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SuiMoveNormalizedTypeVector
               (
                   (SuiMoveNormalizedType)SuiMoveNormalizedType.Deserialize(deserializer)
               );
    }

    /// <summary>
    /// Represents a Reference.
    /// </summary>
    public class SuiMoveNormalizedTypeReference : ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType Reference { get; internal set; }

        public SuiMoveNormalizedTypeReference(SuiMoveNormalizedType reference)
        {
            this.Reference = reference;
        }

        public override void Serialize(Serialization serializer)
            => serializer.Serialize(this.Reference);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SuiMoveNormalizedTypeVector
               (
                   (SuiMoveNormalizedType)SuiMoveNormalizedType.Deserialize(deserializer)
               );
    }

    /// <summary>
    /// Represents a Mutable Reference.
    /// </summary>
    public class SuiMoveNormalizedTypeMutableReference : ISuiMoveNormalizedType
    {
        public SuiMoveNormalizedType MutableReference { get; internal set; }

        public SuiMoveNormalizedTypeMutableReference(SuiMoveNormalizedType mutable_reference)
        {
            this.MutableReference = mutable_reference;
        }

        public override void Serialize(Serialization serializer)
            => serializer.Serialize(this.MutableReference);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SuiMoveNormalizedTypeMutableReference
               (
                   (SuiMoveNormalizedType)SuiMoveNormalizedType.Deserialize(deserializer)
               );
    }
}