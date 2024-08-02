//
//  Deserializer.cs
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

using Sui.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Buffers.Binary;

namespace OpenDive.BCS
{
    /// <summary>
    /// The Deserializer class used for converting from BCS bytes.
    /// </summary>
    public class Deserialization : ReturnBase
    {
        #region Constants

        private readonly int MAX_U8 = (int)(Math.Pow(2, 8) - 1);
        private readonly int MAX_U16 = (int)(Math.Pow(2, 16) - 1);
        private readonly uint MAX_U32 = (uint)(Math.Pow(2, 32) - 1);
        private readonly ulong MAX_U64 = (ulong)(Math.Pow(2, 64) - 1);
        private readonly BigInteger MAX_U128 = (BigInteger)(Math.Pow(2, 128) - 1);
        private readonly BigInteger MAX_U256 = (BigInteger)(Math.Pow(2, 256) - 1);

        private readonly string DeserializeFunctionName = "Deserialize";

        #endregion

        #region Deserialization

        protected MemoryStream Input;
        private readonly long Length;

        public Deserialization(byte[] data)
        {
            this.Length = data.Length;
            this.Input = new MemoryStream(data);
        }

        /// <summary>
        /// The remaining bytes in the memory stream deserializer.
        /// </summary>
        /// <returns>The length of the remaining bytes in `long`.</returns>
        public long Remaining() => this.Length - this.Input.Position;

        /// <summary>
        /// Deserialize a `bool` value.
        /// </summary>
        /// <returns>The deserialized `bool` value.</returns>
        public bool DeserializeBool()
            => this.PeekByte() > 1 ?
            this.SetError<bool, SuiError>(false, "Unexcpected value.", this.PeekByte()) :
            BitConverter.ToBoolean(this.Read(1));

        /// <summary>
        /// Deserializes a `byte` array with the length byte.
        /// </summary>
        /// <returns>The deserialized `byte` array.</returns>
        public byte[] ToBytes() => this.Read(this.DeserializeUleb128());

        /// <summary>
        /// Deserializes a `byte` array with a given length.
        /// </summary>
        /// <param name="length">The amount of `byte` values to return.</param>
        /// <returns>The deserialized `byte` array.</returns>
        public byte[] FixedBytes(int length) => this.Read(length);

        /// <summary>
        /// Deserializes a `BcsMap` dictionary, where the key is a string, and the value is a serializable value.
        /// </summary>
        /// <param name="key_type">The value type of the key.</param>
        /// <param name="value_type">The value type of the value.</param>
        /// <returns>A deserialized `BCSMap` object containing the `Dictionary<ISerializable, ISerializable>` value.</returns>
        public BcsMap DeserializeMap(Type key_type, Type value_type)
        {
            int length = DeserializeUleb128();
            Dictionary<ISerializable, ISerializable> sorted_map = new Dictionary<ISerializable, ISerializable>();

            while (sorted_map.Count < length)
            {
                // Calls the "Deserialize" method for the key and value.
                ISerializable key = this.DeserializeStruct(key_type);
                ISerializable value = this.DeserializeStruct(value_type);

                sorted_map.Add(key, value);
            }

            return new BcsMap(sorted_map);
        }

        /// <summary>
        /// Deserializes a `Sequence` array, where the elements are serializable objects.
        /// </summary>
        /// <param name="value_decoder_type">The value type of the elements.</param>
        /// <returns>A deserialized `Sequence` object containing the `ISerializable[]` value.</returns>
        public Sequence DeserializeSequence(Type value_decoder_type)
        {
            int length = DeserializeUleb128();
            List<ISerializable> values = new List<ISerializable>();

            while (values.Count < length)
                values.Add(this.DeserializeStruct(value_decoder_type));

            return new Sequence(values.ToArray());
        }

        /// <summary>
        /// Deserializes a `BString` value.
        /// </summary>
        /// <returns>A deserialized `BString` object containing the `string` value.</returns>
        public BString DeserializeString()
            => new BString(Encoding.UTF8.GetString(this.Read(this.DeserializeUleb128())));

        /// <summary>
        /// Deserializes an optional value.
        /// </summary>
        /// <param name="value_decoder_type">The value type of the optional.</param>
        /// <returns>Either the deserialized `ISerializable` value, or a null.</returns>
        public ISerializable DeserializeOptional(Type value_decoder_type)
            => this.PeekByte() != 0 ? this.DeserializeStruct(value_decoder_type) : null;

        /// <summary>
        /// Deserializes an `ISerializable` structure (class).
        /// </summary>
        /// <param name="struct_type">The value type of the struct object.</param>
        /// <returns>The deserialized `ISerializable` class object.</returns>
        public ISerializable DeserializeStruct(Type struct_type)
        {
            MethodInfo method = struct_type.GetMethod(this.DeserializeFunctionName, new Type[] { typeof(Deserialization) });
            return (ISerializable)method.Invoke(null, new[] { this });
        }

        /// <summary>
        /// Deserializes a `U8` value.
        /// </summary>
        /// <returns>A deserialized `U8` object containing the `byte` value.</returns>
        public U8 DeserializeU8() => (U8)this.ReadInt(IntegerCase.U8);

        /// <summary>
        /// Deserializes a `U16` value.
        /// </summary>
        /// <returns>A deserialized `U16` object containing the `ushort` value.</returns>
        public U16 DeserializeU16() => (U16)this.ReadInt(IntegerCase.U16);

        /// <summary>
        /// Deserializes a `U32` value.
        /// </summary>
        /// <returns>A deserialized `U32` object containing the `uint` value.</returns>
        public U32 DeserializeU32() => (U32)this.ReadInt(IntegerCase.U32);

        /// <summary>
        /// Deserializes a `U64` value.
        /// </summary>
        /// <returns>A deserialized `U64` object containing the `ulong` value.</returns>
        public U64 DeserializeU64() => (U64)this.ReadInt(IntegerCase.U64);

        /// <summary>
        /// Deserializes a `U128` value.
        /// </summary>
        /// <returns>A deserialized `U128` object containing the `BigInteger` value.</returns>
        public U128 DeserializeU128() => (U128)this.ReadInt(IntegerCase.U128);

        /// <summary>
        /// Deserializes a `U256` value.
        /// </summary>
        /// <returns>A deserialized `U256` object containing the `BigInteger` value.</returns>
        public U256 DeserializeU256() => (U256)this.ReadInt(IntegerCase.U256);

        /// <summary>
        /// Deserializes an `int` value representing the Uleb128 serialized value.
        /// </summary>
        /// <returns>
        /// A deserialized `int` Uleb128 value, returns -1 if an error occured;
        /// and changes the `Error` member to reflect the error.
        /// </returns>
        public int DeserializeUleb128()
        {
            BigInteger value = 0;
            int shift = 0;

            while (value <= this.MAX_U32)
            {
                byte byte_read = this.Read(1)[0];
                value |= ((uint)byte_read & 0x7F) << shift;

                if ((byte_read & 0x80) == 0)
                    break;

                shift += 7;
            }

            if (value > this.MAX_U128)
                return this.SetError<int, SuiError>(-1, "Unexpectedly large uleb128 value.", value);

            return (int)value;
        }

        /// <summary>
        /// Reads in a given amount of bytes from the input memory stream.
        /// </summary>
        /// <param name="length">The number of bytes to read from the stream.</param>
        /// <returns>A `byte` array from the stream.</returns>
        internal byte[] Read(int length)
        {
            byte[] value = new byte[length];
            int total_read = this.Input.Read(value, 0, length);

            if (total_read < length)
                return this.SetError<byte[], SuiError>(new byte[] { }, $"Unexpected end of input. Requested: {length}, found: {total_read}");

            return value;
        }

        // TODO: Look into further benchmarking testing BinaryPrimitive vs low level bit shifting
        /// <summary>
        /// Reads in an integer value from the input memory stream.
        /// </summary>
        /// <param name="integer_type">The type of integer that will be read and returned.</param>
        /// <returns>An `ISerializable` object that represents an unsigned integer.</returns>
        internal ISerializable ReadInt(IntegerCase integer_type)
        {
            byte[] value = this.Read((int)integer_type);
            switch (integer_type)
            {
                case IntegerCase.U8:
                    return new U8(value[0]);
                case IntegerCase.U16:
                    return new U16(BinaryPrimitives.ReadUInt16LittleEndian(value));
                case IntegerCase.U32:
                    return new U32(BinaryPrimitives.ReadUInt32LittleEndian(value));
                case IntegerCase.U64:
                    return new U64(BinaryPrimitives.ReadUInt64LittleEndian(value));
                case IntegerCase.U128:
                    return new U128(new BigInteger(value, isUnsigned: true, isBigEndian: false));
                case IntegerCase.U256:
                    return new U256(new BigInteger(value, isUnsigned: true, isBigEndian: false));
                default:
                    return this.SetError<ISerializable, SuiError>(null, "Unexpected value read from deserializer.");
            }
        }

        /// <summary>
        /// Previews the next byte in the stream.
        /// </summary>
        /// <returns>The next byte in the stream.</returns>
        internal int PeekByte()
        {
            long original_position = this.Input.Position;
            int next_byte = this.Input.ReadByte();
            this.Input.Position = original_position;

            return next_byte;
        }

        #endregion
    }
}
