//
//  Serialization.cs
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

using Sui.Rpc.Client;
using System;
using System.IO;
using System.Numerics;
using System.Buffers.Binary;

namespace OpenDive.BCS
{
    /// <summary>
    /// The serialization value to use with BigIntegers.
    /// </summary>
    public enum BigIntegerSerialization
    {
        U128,
        U256
    }

    /// <summary>
    /// The Serializer class used for converting to BCS bytes.
    /// </summary>
    public class Serialization : ReturnBase
    {
        #region Integer Enum

        /// <summary>
        /// Used internally for determining serialization type.
        /// </summary>
        internal enum WriteIntegerCase
        {
            U8 = 1,
            U16 = 2,
            U32 = 4,
            U64 = 8,
            U128 = 16,
            U256 = 32
        }

        #endregion

        #region Serialization

        /// <summary>
        /// The output stream for the serialized stream.
        /// </summary>
        protected MemoryStream Output;

        public Serialization() => Output = new MemoryStream();

        /// <summary>
        /// Return the serialization buffer as a byte array.
        /// </summary>
        /// <returns>Serialization buffer as a byte array.</returns>
        public byte[] GetBytes() => Output.ToArray();

        /// <summary>
        /// Serialize a string value.
        /// </summary>
        /// <param name="value">String value to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(string value) => SerializeString(value);

        /// <summary>
        /// Serialize a byte array.
        /// </summary>
        /// <param name="value">Byte array to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(byte[] value) => SerializeBytes(value);

        /// <summary>
        /// Serialize a boolean value.
        /// </summary>
        /// <param name="value">Boolean value to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(bool value) => SerializeBool(value);

        /// <summary>
        /// Serialize a single byte
        /// </summary>
        /// <param name="num">Byte to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(byte num) => SerializeU8(num);

        /// <summary>
        /// Serialize an unsigned short value.
        /// </summary>
        /// <param name="num">The number to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(ushort num) => SerializeU16(num);

        /// <summary>
        /// Serialize an unsigned integer value.
        /// </summary>
        /// <param name="num">The unsigned integer to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(uint num) => SerializeU32(num);

        /// <summary>
        /// Serialize an unsigned long number.
        /// </summary>
        /// <param name="num">The unsigned long number to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(ulong num) => SerializeU64(num);

        /// <summary>
        /// Serialize a big integer number with either U128 or U256 serialization.
        /// </summary>
        /// <param name="num">The big integer number to serialize.</param>
        /// <param name="method">Which serialization method to use, defaults to U128.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(BigInteger num, BigIntegerSerialization method = BigIntegerSerialization.U128)
            => method == BigIntegerSerialization.U128 ? SerializeU128(num) : SerializeU256(num);

        /// <summary>
        /// Serializes an object using it's own serialization implementation.
        /// </summary>
        /// <param name="value">Value to serialize</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(ISerializable value)
        {
            value.Serialize(this);
            return this;
        }

        /// <summary>
        /// Serialize a plain sequence as a list of bytes.
        /// </summary>
        /// <param name="args">The sequence to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(Sequence args)
        {
            SerializeU32AsUleb128((uint)args.Length);

            foreach (ISerializable element in args.Values)
            {
                Serialization ser = new Serialization();
                element.Serialize(ser);
                SerializeFixedBytes(ser.GetBytes());
            }

            return this;
        }

        /// <summary>
        /// Serializes an array of serializable elements
        /// </summary>
        /// <param name="args">The `ISerializable` array of serializable values.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization Serialize(ISerializable[] args)
        {
            SerializeU32AsUleb128((uint)args.Length);

            foreach (ISerializable element in args)
            {
                Serialization ser = new Serialization();
                element.Serialize(ser);
                SerializeFixedBytes(ser.GetBytes());
            }

            return this;
        }

        /// <summary>
        /// Serializes a string. UTF8 string is supported. Serializes the string's bytes length "l" first,
        /// and then serializes "l" bytes of the string content.
        /// 
        /// BCS layout for "string": string_length | string_content. string_length is the bytes length of
        /// the string that is uleb128 encoded. string_length is a u32 integer.
        /// </summary>
        /// <param name="value">String value to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeString(string value) => SerializeBytes(System.Text.Encoding.UTF8.GetBytes(value));

        /// <summary>
        /// Serializes an array of bytes.
        /// BCS layout for "bytes": bytes_length | bytes. bytes_length is the length of the bytes array that is
        /// uleb128 encoded. bytes_length is a u32 integer.
        /// </summary>
        /// <param name="bytes">Byte array to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeBytes(byte[] bytes)
        {
            // Write the length of the bytes array
            SerializeU32AsUleb128((uint)bytes.Length);

            // Copy the bytes to the rest of the array
            Output.Write(bytes);

            return this;
        }

        /// <summary>
        /// Serializes a list of values represented in a byte array. 
        /// This can be a sequence or a value represented as a byte array.
        /// Note that for sequences we first add the length for the entire sequence array,
        /// not the length of the byte array.
        /// </summary>
        /// <param name="bytes">Byte array to be serialized.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeFixedBytes(byte[] bytes)
        {
            // Copy the bytes to the rest of the array
            Output.Write(bytes);

            return this;
        }

        /// <summary>
        /// Write an array bytes directly to the serialization buffer.
        /// </summary>
        /// <param name="bytes">Byte array to write to the serialization buffer.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization WriteByte(byte value)
        {
            Output.WriteByte(value);
            return this;
        }

        /// <summary>
        /// Serialize an unsigned integer value. 
        /// Usually used to serialize the length of values.
        /// </summary>
        /// <param name="value">Unsigned integer to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU32AsUleb128(uint value)
        {
            while (value >= 0x80)
            {
                // Write 7 (lowest) bits of data and set the 8th bit to 1.
                byte b = (byte)(value & 0x7f);
                Output.WriteByte((byte)(b | 0x80));
                value >>= 7;
            }

            // Write the remaining bits of data and set the highest bit to 0
            Output.WriteByte((byte)(value & 0x7f));
            return this;
        }

        /// <summary>
        /// Serialize a boolean value
        /// </summary>
        /// <param name="value">Boolean value to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeBool(bool value)
            => value == true ? this.WriteByte(0x01) : this.WriteByte(0x00);

        /// <summary>
        /// Serialize an unsigned byte number.
        /// </summary>
        /// <param name="num">Byte to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU8(byte num)
            => this.Write(num, WriteIntegerCase.U8);

        /// <summary>
        /// Serialize an unsigned short number.
        /// </summary>
        /// <param name="num">Unsigned short number to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU16(ushort num)
            => this.Write(num, WriteIntegerCase.U16);

        /// <summary>
        /// Serialize an unsigned integer number.
        /// </summary>
        /// <param name="num">Unsigned integer number.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU32(uint num)
            => this.Write(num, WriteIntegerCase.U32);

        /// <summary>
        /// Serialize an unsigned long number.
        /// </summary>
        /// <param name="num">Unsigned long number to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU64(ulong num)
            => this.Write(num, WriteIntegerCase.U64);

        /// <summary>
        /// Serialize a unsigned 128 unsigned int (big integer) number.
        /// </summary>
        /// <param name="num">Big integer value to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU128(BigInteger num)
            => this.Write(num, WriteIntegerCase.U128);

        /// <summary>
        /// Serialize a unsigned 256 unsigned int (big integer) number.
        /// </summary>
        /// <param name="num">Big integer value to serialize.</param>
        /// <returns>The current Serialization object.</returns>
        public Serialization SerializeU256(BigInteger num)
            => this.Write(num, WriteIntegerCase.U256);

        /// <summary>
        /// Writes in a given amount of bytes to the output memory stream.
        /// </summary>
        /// <param name="value">The given unsigned integer value to serialize.</param>
        /// <param name="integer_type">
        /// A `WriteIntegerCase` enum that represents the type value is and how long it should be.
        /// </param>
        /// <returns>The current Serialization object.</returns>
        internal Serialization Write<U>(U value, WriteIntegerCase integer_type) where U : IComparable, IFormattable
        {
            if (this.IsSignedType<U>())
                return this.SetError<Serialization, SuiError>(this, "Value is a signed integer.", value);

            if (value is BigInteger big_integer)
            {
                if (big_integer.Sign == -1)
                    return this.SetError<Serialization, SuiError>(this, "Value cannot be negative.", value);

                byte[] big_integer_bytes = big_integer.ToByteArray(isUnsigned: true, isBigEndian: false);

                if (big_integer_bytes.Length > (int)integer_type)
                    return this.SetError<Serialization, SuiError>(this, $"Value cannot be serialized because it is more than {(int)integer_type} bytes long.", value);

                if (!(big_integer_bytes.Length <= (int)integer_type || big_integer_bytes[0] == 0))
                    return this.SetError<Serialization, SuiError>(this, "Value cannot be serialized because it is a negative value.", value);

                this.Output.Write(big_integer_bytes);

                if (big_integer_bytes.Length != (int)integer_type)
                    Output.Write(new byte[(int)integer_type - big_integer_bytes.Length]);

                return this;
            }

            Span<byte> writer = new Span<byte>(new byte[(int)integer_type]);

            switch (integer_type)
            {
                case WriteIntegerCase.U8:
                    writer = new Span<byte>(new byte[] { Convert.ToByte(value) });
                    break;
                case WriteIntegerCase.U16:
                    BinaryPrimitives.WriteUInt16LittleEndian(writer, Convert.ToUInt16(value));
                    break;
                case WriteIntegerCase.U32:
                    BinaryPrimitives.WriteUInt32LittleEndian(writer, Convert.ToUInt32(value));
                    break;
                case WriteIntegerCase.U64:
                    BinaryPrimitives.WriteUInt64LittleEndian(writer, Convert.ToUInt64(value));
                    break;
                default:
                    return this.SetError<Serialization, SuiError>(this, "The integer_type is not a U8, U16, U32, nor a U64.", new Tuple<U, WriteIntegerCase>(value, integer_type));
            }

            this.Output.Write(writer);

            return this;
        }

        /// <summary>
        /// Determines whether the inputted T type is a signed type or not.
        /// </summary>
        /// <typeparam name="T">A generic type that represents a number</typeparam>
        /// <returns>A `bool` that reflects whether T is signed or not.</returns>
        internal bool IsSignedType<T>()
        {
            return
                typeof(T) == typeof(sbyte) ||
                typeof(T) == typeof(short) ||
                typeof(T) == typeof(int) ||
                typeof(T) == typeof(long);
        }

        #endregion
    }
}