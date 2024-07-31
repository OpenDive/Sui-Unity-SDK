//
//  BcsMap.cs
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
using System.Linq;

namespace OpenDive.BCS
{
    /// <summary>
    /// Representation of a `Dictionary<ISerializable, ISerializable>` in BCS.
    /// </summary>
    public class BcsMap : ReturnBase, ISerializable
    {
        /// <summary>
        /// The dictionary of values, with the `BString` being the key, and an `ISerializable` object being the value.
        /// </summary>
        public Dictionary<ISerializable, ISerializable> Values { get; set; }

        public BcsMap(Dictionary<ISerializable, ISerializable> values) => this.Values = values;

        /// <summary>
        /// The amount of items in the `Value` array.
        /// </summary>
        public int Length { get => this.Values.Keys.Count(); }

        public void Serialize(Serialization serializer)
        {
            Serialization map_serializer = new Serialization();
            List<Tuple<byte[], byte[]>> encoded_values = new List<Tuple<byte[], byte[]>>();

            foreach (KeyValuePair<ISerializable, ISerializable> entry in this.Values)
            {
                Serialization key_serializer = new Serialization();
                entry.Key.Serialize(key_serializer);

                Serialization val_serializer = new Serialization();
                entry.Value.Serialize(val_serializer);

                encoded_values.Add
                (
                    new Tuple<byte[], byte[]>
                    (
                        key_serializer.GetBytes(),
                        val_serializer.GetBytes()
                    )
                );
            }

            encoded_values.Sort(ByteArrayComparer.CompareTuple);

            map_serializer.SerializeU32AsUleb128((uint)encoded_values.Count);

            foreach (Tuple<byte[], byte[]> entry in encoded_values)
            {
                map_serializer.SerializeFixedBytes(entry.Item1);
                map_serializer.SerializeFixedBytes(entry.Item2);
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
            if (other is not BcsMap && other is not Dictionary<ISerializable, ISerializable>)
                return this.SetError<bool, BcsError>(false, "Compared object is not a BcsMap nor a Dictionary<ISerializable, ISerializable>.", other);

            Dictionary<ISerializable, ISerializable> other_map_sequence;

            if (other is BcsMap bcs_map)
                other_map_sequence = bcs_map.Values;
            else
                other_map_sequence = (Dictionary<ISerializable, ISerializable>)other;

            bool equal = true;

            if (this.Values.Keys.Count != other_map_sequence.Keys.Count)
                return false;

            for (int i = 0; i < this.Length; i++)
                equal = equal &&
                    this.Values.Keys.ToArray()[i].Equals(other_map_sequence.Keys.ToArray()[i]) &&
                    this.Values[this.Values.Keys.ToArray()[i]].Equals(other_map_sequence[other_map_sequence.Keys.ToArray()[i]]);

            return equal;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}