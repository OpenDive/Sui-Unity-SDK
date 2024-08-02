//
//  BString.cs
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
    /// Representation of `string` in BCS.
    /// </summary>
    public class BString : ReturnBase, ISerializable
    {
        /// <summary>
        /// The `string` value.
        /// </summary>
        public string Value { get; set; }

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
                return this.SetError<bool, BcsError>(false, "Compared object is not a BString nor a string.", other);

            string other_string;

            if (other is BString b_string)
                other_string = b_string.Value;
            else
                other_string = (string)other;

            if (this.Value.Length != other_string.Length)
                return false;

            return this.Value == other_string;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}