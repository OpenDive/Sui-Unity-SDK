//
//  U128.cs
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

using System.Numerics;
using Sui.Utilities;

namespace OpenDive.BCS
{
    /// <summary>
    /// Representation of a U128 BigInteger in BCS.
    /// </summary>
    public class U128 : ReturnBase, ISerializable
    {
        /// <summary>
        /// A `BigInteger` unsigned 128-bit long integer value.
        /// </summary>
        public BigInteger Value { get; set; }

        public U128(BigInteger value) => this.Value = value;

        public void Serialize(Serialization serializer) => serializer.SerializeU128(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer) => deserializer.DeserializeU128();

        public override string ToString() => this.Value.ToString();

        public override bool Equals(object other)
        {
            if (other is not U128 && other is not BigInteger)
                return this.SetError<bool, BcsError>(false, "Compared object is not a U128 nor a BigInteger.", other);

            BigInteger other_u128_big_integer;

            if (other is U128 u128)
                other_u128_big_integer = u128.Value;
            else
                other_u128_big_integer = (BigInteger)other;

            return this.Value.Equals(other_u128_big_integer);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}