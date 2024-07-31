//
//  U32.cs
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
    /// Representation of `uint` in BCS.
    /// </summary>
    public class U32 : ReturnBase, ISerializable
    {
        /// <summary>
        /// A `uint` value.
        /// </summary>
        public uint Value { get; set; }

        public U32(uint value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU32(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU32();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U32 && other is not uint)
                return this.SetError<bool, BcsError>(false, "Compared object is not a U32 nor a uint.", other);

            uint other_uint;

            if (other is U32 u32)
                other_uint = u32.Value;
            else
                other_uint = (uint)other;

            return this.Value == other_uint;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}