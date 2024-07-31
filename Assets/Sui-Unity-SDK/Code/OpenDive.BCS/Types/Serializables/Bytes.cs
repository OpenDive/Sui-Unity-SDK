//
//  Bytes.cs
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
using System.Linq;

namespace OpenDive.BCS
{
    /// <summary>
    /// Representation of `byte[]` in BCS.
    /// </summary>
    public class Bytes : ReturnBase, ISerializable
    {
        /// <summary>
        /// The `byte` array value.
        /// </summary>
        public byte[] Values { get; set; }

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
                return this.SetError<bool, BcsError>(false, "Compared object is not a Bytes nor a byte[].", other);

            byte[] other_bytes;

            if (other is Bytes bytes)
                other_bytes = bytes.Values;
            else
                other_bytes = (byte[])other;

            if (this.Length != other_bytes.Length)
                return false;

            return this.Values.SequenceEqual(other_bytes);
        }

        public override string ToString() => this.Values.ToReadableString();

        public override int GetHashCode() => base.GetHashCode();
    }
}