//
//  BytesSequence.cs
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
using System.Collections.Generic;
using System.Linq;

namespace OpenDive.BCS
{
    /// <summary>
    /// Representation of a `byte[]` sequence.
    /// </summary>
    public class BytesSequence : ReturnBase, ISerializable
    {
        /// <summary>
        /// The `bytes[]` array value.
        /// </summary>
        public byte[][] Values { get; set; }

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
            if (other is not BytesSequence && other is not byte[])
                return this.SetError<bool, BcsError>(false, "Compared object is not a ByteSequence nor a byte[] array.", other);

            byte[][] other_byte_sequence;

            if (other is BytesSequence byte_sequence)
                other_byte_sequence = byte_sequence.Values;
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
                    this.Values[i].ToReadableString() :
                    $"{this.Values[i].ToReadableString()}, ";

            result += "]";

            return result;
        }
    }
}