//
//  ISerializable.cs
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

namespace OpenDive.BCS
{
    public interface ISerializable
    {
        /// <summary>
        /// Serializes an output instance using the given Serializer.
        /// </summary>
        /// <param name="serializer">The Serializer instance used to serialize the data.</param>
        public void Serialize(Serialization serializer);

        /// <summary>
        /// Deserializes an output instance from a Deserializer.
        /// </summary>
        /// <param name="deserializer">The Deserializer instance used to deserialize the data.</param>
        /// <returns>A new `ISerializable` object that was deserialized from the input.</returns>
        public static ISerializable Deserialize(Deserialization deserializer) => new BcsError(0, "Deserialize is not implemented", null);
    }
}