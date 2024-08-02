//
//  Sequence.cs
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
    /// Representation of an `ISerializable` sequence / list.
    /// </summary>
    public class Sequence : ReturnBase, ISerializable
    {
        /// <summary>
        /// The `ISerializable` value list.
        /// </summary>
        public ISerializable[] Values { get; set; }

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
                return this.SetError<bool, BcsError>(false, "Compared object is not a Sequence nor an ISerializable[].", other);

            ISerializable[] other_sequence;

            if (other is Sequence sequence)
                other_sequence = sequence.Values;
            else
                other_sequence = (ISerializable[])other;

            if (this.Length != other_sequence.Length)
                return false;

            return this.Values.SequenceEqual(other_sequence);
        }

        public override string ToString() => this.Values.ToReadableString();

        public override int GetHashCode() => base.GetHashCode();
    }
}