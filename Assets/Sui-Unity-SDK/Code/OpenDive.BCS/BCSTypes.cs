//
//  BCSTypes.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace OpenDive.BCS
{
    /// <summary>
    /// An enum value that represents a Move type.
    /// </summary>
    public enum TypeTag
    {
        BOOL,
        U8,
        U64,
        U128,
        ACCOUNT_ADDRESS,
        SIGNER,
        VECTOR,
        STRUCT,
        U16,
        U32,
        U256
    }

    public interface ISerializable
    {
        /// <summary>
        /// Serializes an output instance using the given Serializer.
        /// </summary>
        /// <param name="serializer">The Serializer instance used to serialize the data.</param>
        public void Serialize(Serialization serializer);

        /// <summary>
        /// Deserializes an output instance from a Deserializer.
        /// </summary>
        /// <param name="deserializer">The Deserializer instance used to deserialize the data.</param>
        /// <returns>A new `ISerializable` object that was deserialized from the input.</returns>
        public static ISerializable Deserialize(Deserialization deserializer) => new SuiError(0, "Deserialize is not implemented", null);
    }

    /// <summary>
    /// An object that contains the type tag associated with the BCS value.
    /// </summary>
    public class SerializableTypeTag: ISerializable
    {
        /// <summary>
        /// The Move type of the value.
        /// </summary>
        public TypeTag Type { get; set; }

        /// <summary>
        /// The serializable value.
        /// </summary>
        public ISerializable Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public RpcError Error { get; private set; }

        public SerializableTypeTag(ISerializable value)
        {
            this.Value = value;

            if (value.GetType() == typeof(SuiStructTag))
                this.Type = TypeTag.STRUCT;
            else if (value.GetType() == typeof(AccountAddress))
                this.Type = TypeTag.ACCOUNT_ADDRESS;
            else
                this.Error = new RpcError(0, "Unable to initialize SerializableTypeTag.", value);
        }

        public SerializableTypeTag(string string_value)
        {
            SuiStructTag struct_tag = SuiStructTag.FromStr(string_value);

            if (struct_tag != null)
            {
                this.Value = struct_tag;
                this.Type = TypeTag.STRUCT;
            }
            else
            {
                AccountAddress account_address = AccountAddress.FromHex(string_value);
                if (account_address != null)
                {
                    this.Value = account_address;
                    this.Type = TypeTag.ACCOUNT_ADDRESS;
                }
                else if (string_value == "bool")
                {
                    this.Value = null;
                    this.Type = TypeTag.BOOL;
                }
                else if (string_value == "u8")
                {
                    this.Value = null;
                    this.Type = TypeTag.U8;
                }
                else if (string_value == "u16")
                {
                    this.Value = null;
                    this.Type = TypeTag.U16;
                }
                else if (string_value == "u32")
                {
                    this.Value = null;
                    this.Type = TypeTag.U32;
                }
                else if (string_value == "u64")
                {
                    this.Value = null;
                    this.Type = TypeTag.U64;
                }
                else if (string_value == "u128")
                {
                    this.Value = null;
                    this.Type = TypeTag.U128;
                }
                else if (string_value == "u256")
                {
                    this.Value = null;
                    this.Type = TypeTag.U256;
                }
                else
                    this.Error = new RpcError(0, "Unable to initialize SerializableTypeTag.", string_value);
            }
        }

        public SerializableTypeTag(TypeTag type, ISerializable value = null)
        {
            this.Type = type;
            this.Value = value;
        }

        public override string ToString()
        {
            switch (this.Type)
            {
                case TypeTag.ACCOUNT_ADDRESS:
                    return "address";
                default:
                    return this.Type.ToString().ToLower();
            }
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            this.Value.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            TypeTag variant = (TypeTag)deserializer.DeserializeUleb128();
            ISerializable ser_value;

            if (variant == TypeTag.BOOL)
                ser_value = new Bool(deserializer.DeserializeBool());
            else if (variant == TypeTag.U8)
                ser_value = deserializer.DeserializeU8();
            else if (variant == TypeTag.U16)
                ser_value = deserializer.DeserializeU16();
            else if (variant == TypeTag.U32)
                ser_value = deserializer.DeserializeU32();
            else if (variant == TypeTag.U64)
                ser_value = deserializer.DeserializeU64();
            else if (variant == TypeTag.U128)
                ser_value = deserializer.DeserializeU128();
            else if (variant == TypeTag.U256)
                ser_value = deserializer.DeserializeU256();
            else if (variant == TypeTag.ACCOUNT_ADDRESS)
                ser_value = (AccountAddress)AccountAddress.Deserialize(deserializer);
            else if (variant == TypeTag.STRUCT)
                ser_value = (SuiStructTag)SuiStructTag.Deserialize(deserializer);
            // TODO: Implement below deserialization types.
            else if (variant == TypeTag.SIGNER)
                return new SuiError(0, "Unable to deserialize SerialziableTypeTag for Signer", null);
            else if (variant == TypeTag.VECTOR)
                return new SuiError(0, "Unable to deserialize SerialziableTypeTag for Vector", null);
            else
                return new SuiError(0, "Unable to deserialize SerialziableTypeTag", null);

            return new SerializableTypeTag(variant, ser_value);
        }
    }

    /// <summary>
    /// Representation of an `ISerializable` sequence / list.
    /// </summary>
    public class Sequence : ISerializable
    {
        /// <summary>
        /// The `ISerializable` value list.
        /// </summary>
        public ISerializable[] Values { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public Sequence(ISerializable[] serializable) => this.Values = serializable;

        /// <summary>
        /// The amount of items in the `Value` array.
        /// </summary>
        public int Length { get => this.Values.Length; }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)this.Length);

            foreach (ISerializable element in this.Values)
            {
                Type elementType = element.GetType();
                if (elementType == typeof(Sequence))
                {
                    Serialization seqSerializer = new Serialization();
                    Sequence seq = (Sequence)element;
                    seqSerializer.Serialize(seq);

                    byte[] elementsBytes = seqSerializer.GetBytes();
                    int sequenceLen = elementsBytes.Length;
                    serializer.SerializeU32AsUleb128((uint)sequenceLen);
                    serializer.SerializeFixedBytes(elementsBytes);
                }
                else
                {
                    Serialization s = new Serialization();
                    element.Serialize(s);
                    byte[] b = s.GetBytes();
                    serializer.SerializeFixedBytes(b);
                }
            }
        }

        public static ISerializable Deserialize(Deserialization deser)
        {
            int length = deser.DeserializeUleb128();
            List<ISerializable> values = new List<ISerializable>();

            while (values.Count < length)
                values.Add(new Bytes(deser.ToBytes()));

            return new Sequence(values.ToArray());
        }

        public override bool Equals(object other)
        {
            if (other is not Sequence && other is not ISerializable[])
            {
                this.Error = new SuiError(0, "Compared object is not a Sequence nor an ISerializable[].", other);
                return false;
            }

            ISerializable[] other_sequence;

            if (other is Sequence)
                other_sequence = ((Sequence)other).Values;
            else
                other_sequence = (ISerializable[])other;

            if (this.Length != other_sequence.Length)
                return false;

            return this.Values.SequenceEqual(other_sequence);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (var value in Values)
                result.Append(value.ToString());

            return result.ToString();
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of a `byte[]` sequence.
    /// </summary>
    public class BytesSequence : ISerializable
    {
        /// <summary>
        /// The `bytes[]` array value.
        /// </summary>
        public byte[][] Values { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public BytesSequence(byte[][] values) => this.Values = values;

        /// <summary>
        /// The amount of items in the `Value` array.
        /// </summary>
        public int Length { get => this.Values.Length; }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)this.Values.Length);

            foreach (byte[] element in this.Values)
                serializer.SerializeBytes(element);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            int length = deserializer.DeserializeUleb128();
            List<byte[]> bytesList = new List<byte[]>();

            while (bytesList.Count < length)
            {
                byte[] bytes = deserializer.ToBytes();
                bytesList.Add(bytes);
            }

            return new BytesSequence(bytesList.ToArray());
        }

        public override bool Equals(object other)
        {
            if (other is not BytesSequence && other is not byte[][])
            {
                this.Error = new SuiError(0, "Compared object is not a ByteSequence nor a byte[] array.", other);
                return false;
            }

            byte[][] other_byte_sequence;

            if (other is BytesSequence)
                other_byte_sequence = ((BytesSequence)other).Values;
            else
                other_byte_sequence = (byte[][])other;

            bool equal = true;

            if (this.Length != other_byte_sequence.Length)
                return false;

            for (int i = 0; i < this.Values.Length; i++)
                equal = equal && this.Values[i].SequenceEqual(other_byte_sequence[i]);

            return equal;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            string result = "[";

            for (int i = 0; i < this.Length; ++i)
                result += i == (this.Length - 1) ?
                    $"[{string.Join(", ", this.Values[i])}]" :
                    $"[{string.Join(", ", this.Values[i])}], ";

            result += "]";

            return result;
        }
    }

    /// <summary>
    /// Representation of a `Dictionary<BString, ISerializable>` in BCS.
    /// </summary>
    public class BCSMap : ISerializable
    {
        /// <summary>
        /// The dictionary of values, with the `BString` being the key, and an `ISerializable` object being the value.
        /// </summary>
        public Dictionary<BString, ISerializable> Values { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public BCSMap(Dictionary<BString, ISerializable> values) => this.Values = values;

        /// <summary>
        /// The amount of items in the `Value` array.
        /// </summary>
        public int Length { get => this.Values.Keys.Count(); }

        /// <summary>
        /// Maps (Key / Value Stores)
        /// Maps are represented as a variable-length, sorted sequence of(Key, Value) tuples.
        /// Keys must be unique and the tuples sorted by increasing lexicographical order on 
        /// the BCS bytes of each key.
        /// The representation is otherwise similar to that of a variable-length sequence.
        /// In particular, it is preceded by the number of tuples, encoded in ULEB128.
        /// </summary>
        /// <param name="serializer"></param>
        public void Serialize(Serialization serializer)
        {
            Serialization map_serializer = new Serialization();
            SortedDictionary<string, (byte[], byte[])> byte_map = new SortedDictionary<string, (byte[], byte[])>();

            foreach (KeyValuePair<BString, ISerializable> entry in this.Values)
            {
                Serialization key_serializer = new Serialization();
                entry.Key.Serialize(key_serializer);
                byte[] b_key = key_serializer.GetBytes();

                Serialization val_serializer = new Serialization();
                entry.Value.Serialize(val_serializer);
                byte[] b_value = val_serializer.GetBytes();

                byte_map.Add(entry.Key.Value, (b_key, b_value));
            }

            map_serializer.SerializeU32AsUleb128((uint)byte_map.Count);

            foreach (KeyValuePair<string, (byte[], byte[])> entry in byte_map)
            {
                map_serializer.SerializeFixedBytes(entry.Value.Item1);
                map_serializer.SerializeFixedBytes(entry.Value.Item2);
            }

            serializer.SerializeFixedBytes(map_serializer.GetBytes());
        }

        public override string ToString()
        {
            string result = "[";

            for (int i = 0; i < this.Length; ++i)
                result += i == (this.Length - 1) ?
                    $"({this.Values.Keys.ToArray()[i]}, {this.Values[this.Values.Keys.ToArray()[i]]})" :
                    $"({this.Values.Keys.ToArray()[i]}, {this.Values[this.Values.Keys.ToArray()[i]]}), ";

            result += "]";

            return result;
        }

        public override bool Equals(object other)
        {
            if (other is not BCSMap && other is not Dictionary<BString, ISerializable>)
            {
                this.Error = new SuiError(0, "Compared object is not a BCSMap nor a Dictionary<BString, ISerializable>.", other);
                return false;
            }

            Dictionary<BString, ISerializable> other_map_sequence;

            if (other is BCSMap)
                other_map_sequence = ((BCSMap)other).Values;
            else
                other_map_sequence = (Dictionary<BString, ISerializable>)other;

            bool equal = true;

            if (this.Values.Keys.Count != other_map_sequence.Keys.Count)
                return false;

            for (int i = 0; i < this.Length; i++)
                equal = equal &&
                    this.Values.Keys.ToArray()[i] == other_map_sequence.Keys.ToArray()[i] &&
                    this.Values[this.Values.Keys.ToArray()[i]] == other_map_sequence[other_map_sequence.Keys.ToArray()[i]];

            return equal;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `string` in BCS.
    /// </summary>
    public class BString : ISerializable
    {
        /// <summary>
        /// The `string` value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public BString(string value) => this.Value = value;

        /// <summary>
        /// The amount of items in the `Value` array.
        /// </summary>
        public int Length { get => this.Value.Length; }

        public void Serialize(Serialization serializer) => serializer.SerializeString(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeString();

        public override string ToString() => this.Value;

        public override bool Equals(object other)
        {
            if (other is not BString && other is not string)
            {
                this.Error = new SuiError(0, "Compared object is not a BString nor a string.", other);
                return false;
            }

            string other_string;

            if (other is BString)
                other_string = ((BString)other).Value;
            else
                other_string = (string)other;

            if (this.Value.Length != other_string.Length)
                return false;

            return this.Value == other_string;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `byte[]` in BCS.
    /// </summary>
    public class Bytes : ISerializable
    {
        /// <summary>
        /// The `byte` array value.
        /// </summary>
        public byte[] Values { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public Bytes(byte[] values) => this.Values = values;

        /// <summary>
        /// The amount of items in the `Value` array.
        /// </summary>
        public int Length { get => this.Values.Length; }

        public void Serialize(Serialization serializer) => serializer.SerializeBytes(this.Values);

        public ISerializable Deserialize(Deserialization deserializer) => new Bytes(deserializer.ToBytes());

        public override bool Equals(object other)
        {
            if (other is not Bytes && other is not byte[])
            {
                this.Error = new SuiError(0, "Compared object is not a Bytes nor a byte[].", other);
                return false;
            }

            byte[] other_bytes;

            if (other is Bytes)
                other_bytes = ((Bytes)other).Values;
            else
                other_bytes = (byte[])other;

            if (this.Length != other_bytes.Length)
                return false;

            return this.Values.SequenceEqual(other_bytes);
        }

        public override string ToString() => $"[{string.Join(", ", this.Values)}]";

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `bool` in BCS.
    /// </summary>
    public class Bool : ISerializable
    {
        /// <summary>
        /// The `bool` value.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public Bool(bool value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeBool(this.Value);

        public static Bool Deserialize(Deserialization deserializer) => new Bool(deserializer.DeserializeBool());

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not Bool || other is not bool)
            {
                this.Error = new SuiError(0, "Compared object is not a Bool nor a bool.", other);
                return false;
            }

            bool other_bool;

            if (other is Bool)
                other_bool = ((Bool)other).Value;
            else
                other_bool = (bool)other;

            return this.Value == other_bool;
        }


        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `byte` in BCS.
    /// </summary>
    public class U8 : ISerializable
    {
        /// <summary>
        /// The `byte` value.
        /// </summary>
        public byte Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public U8(byte value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU8(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU8();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U8 && other is not byte)
            {
                this.Error = new SuiError(0, "Compared object is not a U8 nor a byte.", other);
                return false;
            }

            byte other_byte;

            if (other is U8)
                other_byte = ((U8)other).Value;
            else
                other_byte = (byte)other;

            return this.Value == other_byte;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `ushort` in BCS.
    /// </summary>
    public class U16 : ISerializable
    {
        /// <summary>
        /// A `ushort` value.
        /// </summary>
        public ushort Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public U16(ushort value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU16(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU16();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U16 && other is not ushort)
            {
                this.Error = new SuiError(0, "Compared object is not a U16 nor a ushort.", other);
                return false;
            }

            ushort other_ushort;

            if (other is U16)
                other_ushort = ((U16)other).Value;
            else
                other_ushort = (ushort)other;

            return this.Value == other_ushort;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `uint` in BCS.
    /// </summary>
    public class U32 : ISerializable
    {
        /// <summary>
        /// A `uint` value.
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public U32(uint value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU32(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU32();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U32 && other is not uint)
            {
                this.Error = new SuiError(0, "Compared object is not a U32 nor a uint.", other);
                return false;
            }

            uint other_uint;

            if (other is U32)
                other_uint = ((U32)other).Value;
            else
                other_uint = (uint)other;

            return this.Value == other_uint;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of `ulong` in BCS.
    /// </summary>
    public class U64 : ISerializable
    {
        /// <summary>
        /// A `ulong` value.
        /// </summary>
        public ulong Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public U64(ulong value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU64(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU64();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U64 && other is not ulong)
            {
                this.Error = new SuiError(0, "Compared object is not a U64 nor a ulong.", other);
                return false;
            }

            ulong other_ulong;

            if (other is U64)
                other_ulong = ((U64)other).Value;
            else
                other_ulong = (ulong)other;

            return this.Value == other_ulong;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of a U128 BigInteger in BCS.
    /// </summary>
    public class U128 : ISerializable
    {
        /// <summary>
        /// A `BigInteger` unsigned 128-bit long integer value.
        /// </summary>
        public BigInteger Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public U128(BigInteger value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU128(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU128();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U128 && other is not BigInteger)
            {
                this.Error = new SuiError(0, "Compared object is not a U128 nor a BigInteger.", other);
                return false;
            }

            BigInteger other_u128_big_integer;

            if (other is U128)
                other_u128_big_integer = ((U128)other).Value;
            else
                other_u128_big_integer = (BigInteger)other;

            return this.Value == other_u128_big_integer;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of a U256 BigInteger in BCS.
    /// </summary>
    public class U256 : ISerializable
    {
        /// <summary>
        /// A `BigInteger` unsigned 256-bit long integer value.
        /// </summary>
        public BigInteger Value { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; private set; }

        public U256(BigInteger value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU256(this.Value);

        public static U256 Deserialize(Deserialization deserializer) => deserializer.DeserializeU256();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U256 && other is not BigInteger)
            {
                this.Error = new SuiError(0, "Compared object is not a U256 nor a BigInteger.", other);
                return false;
            }

            BigInteger other_u256_big_integer;

            if (other is U256)
                other_u256_big_integer = ((U256)other).Value;
            else
                other_u256_big_integer = (BigInteger)other;

            return this.Value == other_u256_big_integer;
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Representation of a Struct Tag in BCS
    /// </summary>
    /// <typeparam name="T">Value of Type Arguments used</typeparam>
    public abstract class StructTag<T>: ISerializable where T: ISerializable
    {
        /// <summary>
        /// The address of the struct tag.
        /// </summary>
        public AccountAddress Address { get; set; }

        /// <summary>
        /// The name of the contract module.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The name of the function called.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type fields for the given struct.
        /// </summary>
        public List<T> TypeArguments { get; set; }

        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; internal set; }

        internal static string[] FromStr_(string struct_tag) => struct_tag.Split("::");

        public virtual void Serialize(Serialization serializer) => this.Error = new SuiError(0, "Serialize function not implemented", null);

        public override string ToString()
        {
            string value = string.Format(
                "{0}::{1}::{2}",
                this.Address.ToHex(),
                this.Module.ToString(),
                this.Name.ToString()
            );

            if (this.TypeArguments != null && this.TypeArguments.Count > 0)
            {
                value += string.Format("<{0}", this.TypeArguments[0].ToString());

                foreach (T typeArg in this.TypeArguments.ToArray()[1..])
                    value += string.Format(", {0}", typeArg.ToString());

                value += ">";
            }

            return value;
        }

        public override bool Equals(object other)
        {
            if (other is not StructTag<T>)
            {
                this.Error = new SuiError(0, "Compared object is not a StructTag", other);
                return false;
            }

            StructTag<T> other_struct_tag = (StructTag<T>)other;

            return this.Address.AddressBytes.SequenceEqual(other_struct_tag.Address.AddressBytes) &&
                this.Module == other_struct_tag.Module &&
                this.Name == other_struct_tag.Name &&
                Enumerable.SequenceEqual(this.TypeArguments, other_struct_tag.TypeArguments);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class SuiStructTag : StructTag<SerializableTypeTag>
    {
        internal SuiStructTag(ErrorBase error) => this.Error = error;

        public SuiStructTag(AccountAddress address, string module, string name, List<SerializableTypeTag> type_arguments)
        {
            this.Address = address;
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public SuiStructTag(string address, string module, string name, List<SerializableTypeTag> type_arguments)
        {
            this.Address = AccountAddress.FromHex(address);
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public SuiStructTag(string suiStructTag)
        {
            SuiStructTag result = SuiStructTag.FromStr(suiStructTag);
            this.Address = result.Address;
            this.Module = result.Module;
            this.Name = result.Name;
            this.TypeArguments = result.TypeArguments;
        }

        public override void Serialize(Serialization serializer)
        {
            this.Address.Serialize(serializer);
            serializer.Serialize(this.Module);
            serializer.Serialize(this.Name);

            if (this.TypeArguments.Count != 0)
            {
                Sequence type_arguments = new Sequence(this.TypeArguments.ToArray());
                serializer.Serialize(type_arguments);
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiStructTag
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                deserializer.DeserializeString().Value,
                deserializer.DeserializeString().Value,
                new List<SerializableTypeTag>()  // TODO: Implement Deserializer for TypeArguments
            );
        }

        public static SuiStructTag FromStr(string type_tag)
        {
            try
            {
                string[] struct_tag_components = SuiStructTag.FromStr_(type_tag);

                string name = "";
                int index = 0;
                SerializableTypeTag nested_value = null;

                while (index < struct_tag_components[2].Length)
                {
                    char letter = struct_tag_components[2][index];
                    index += 1;

                    if (letter == '<')
                    {
                        nested_value = new SerializableTypeTag
                        (
                            struct_tag_components[2][index..struct_tag_components[2].Length]
                        );
                        break;
                    }
                    else if (letter == '>')
                        break;
                    else
                        name += letter;
                }

                AccountAddress address = AccountAddress.FromHex(struct_tag_components[0]);

                return new SuiStructTag(
                    address,
                    struct_tag_components[1],
                    name,
                    nested_value != null ?
                        new List<SerializableTypeTag> { nested_value } :
                        new List<SerializableTypeTag> { }
                );
            }
            catch
            {
                return new SuiStructTag(new SuiError(0, "Unable to initialize SuiStructTag from string", type_tag));
            }
        }
    }

    public class StructTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SuiMoveNormalizedStructType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject structTypeObj = (JObject)JToken.ReadFrom(reader);
                AccountAddress address = AccountAddress.FromHex
                (
                    NormalizedTypeConverter.NormalizeSuiAddress(structTypeObj["package"].Value<string>())
                );
                string module = structTypeObj["module"].Value<string>();
                string name = structTypeObj["function"].Value<string>();

                List<SuiMoveNormalizedType> normalizedTypes = new List<SuiMoveNormalizedType>();

                foreach (JToken typeObject in structTypeObj["typeArguments"])
                    normalizedTypes.Add(typeObject.ToObject<SuiMoveNormalizedType>());

                return new SuiMoveNormalizedStructType(address, module, name, normalizedTypes);
            }

            return new SuiMoveNormalizedStructType(new SuiError(0, "Unable to convert JSON to SuiMoveNormalizedStructType.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();
                SuiMoveNormalizedStructType struct_type = (SuiMoveNormalizedStructType)value;

                writer.WritePropertyName("package");
                writer.WriteValue(struct_type.Address.ToHex());

                writer.WritePropertyName("module");
                writer.WriteValue(struct_type.Module);

                writer.WritePropertyName("function");
                writer.WriteValue(struct_type.Name);

                writer.WritePropertyName("typeArguments");
                writer.WriteStartArray();

                foreach (SuiMoveNormalizedType normalized_type in struct_type.TypeArguments)
                    writer.WriteValue(normalized_type);

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }

    [JsonConverter(typeof(StructTypeConverter))]
    public class SuiMoveNormalizedStructType : StructTag<SuiMoveNormalizedType>
    {
        internal SuiMoveNormalizedStructType(ErrorBase error) => this.Error = error;

        public SuiMoveNormalizedStructType(string address, string module, string name, List<SuiMoveNormalizedType> type_arguments)
        {
            this.Address = AccountAddress.FromHex(address);
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public SuiMoveNormalizedStructType(AccountAddress address, string module, string name, List<SuiMoveNormalizedType> type_arguments)
        {
            this.Address = address;
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public static SuiMoveNormalizedStructType FromStr(string type_tag)
        {
            try
            {
                string[] struct_tag_components = SuiMoveNormalizedStructType.FromStr_(type_tag);

                string name = "";
                int index = 0;
                SuiMoveNormalizedType nested_value = null;

                while (index < struct_tag_components[2].Length)
                {
                    char letter = struct_tag_components[2][index];
                    index += 1;

                    if (letter == '<')
                    {
                        nested_value = SuiMoveNormalizedType.FromStr(struct_tag_components[2][index..struct_tag_components[2].Length]);
                        break;
                    }
                    else if (letter == '>')
                        break;
                    else
                        name += letter;
                }

                AccountAddress address = AccountAddress.FromHex(struct_tag_components[0]);

                return new SuiMoveNormalizedStructType(
                    address,
                    struct_tag_components[1],
                    name,
                    nested_value != null ?
                        new List<SuiMoveNormalizedType> { nested_value } :
                        new List<SuiMoveNormalizedType> { }
                );
            }
            catch
            {
                return new SuiMoveNormalizedStructType(new SuiError(0, "Unable to initialize SuiMoveNormalizedStructType from string", type_tag));
            }
        }

        public override void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Address);
            serializer.Serialize(this.Module);
            serializer.Serialize(this.Name);

            if (this.TypeArguments.Count != 0)
            {
                Sequence type_arguments = new Sequence(this.TypeArguments.ToArray());
                serializer.Serialize(type_arguments);
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalizedStructType
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                deserializer.DeserializeString().Value,
                deserializer.DeserializeString().Value,
                new List<SuiMoveNormalizedType> { }  // TODO: Implement deserializer for fetching Type Arguments.
            );
        }
    }
}