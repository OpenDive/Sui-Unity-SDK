//
//  U64.cs
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

namespace OpenDive.BCS
{
    /// <summary>
    /// Representation of `ulong` in BCS.
    /// </summary>
    public class U64 : ReturnBase, ISerializable
    {
        /// <summary>
        /// A `ulong` value.
        /// </summary>
        public ulong Value { get; set; }

        public U64(ulong value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU64(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU64();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U64 && other is not ulong)
                return this.SetError<bool, BcsError>(false, "Compared object is not a U64 nor a ulong.", other);

            ulong other_ulong;

            if (other is U64 u64)
                other_ulong = u64.Value;
            else
                other_ulong = (ulong)other;

            return this.Value == other_ulong;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}