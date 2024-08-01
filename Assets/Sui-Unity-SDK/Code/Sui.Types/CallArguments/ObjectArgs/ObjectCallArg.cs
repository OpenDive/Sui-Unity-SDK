//
//  ObjectCallArg.cs
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

using OpenDive.BCS;

namespace Sui.Types
{
    /// <summary>
    /// Base interfaces that both a SuiObjectRef and a SharedObjectRef must implement.
    /// This is used to create an abstraction that will allow it to become an argument.
    /// In the Sui TypeScript SDK this is referred to as an `ObjectArg`.
    /// </summary>
    public class ObjectCallArg : ICallArg
    {
        /// <summary>
        /// The object argument's content.
        /// </summary>
        public ObjectArg ObjectArg { get; set; }

        public ObjectCallArg(ObjectArg objectArg)
        {
            ObjectArg = objectArg;
        }

        /// <summary>
        /// Returns whether the object argument is a mutable shared object input.
        /// </summary>
        /// <returns>A bool representing whether the object argument is a mutable shared object input.</returns>
        public bool IsMutableSharedObjectInput()
            => this.ObjectArg.Type == ObjectRefType.ImmOrOwned ? false : ((SharedObjectRef)this.ObjectArg.ObjectRef).Mutable;

        public void Serialize(Serialization serializer)
            => serializer.Serialize(this.ObjectArg);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new ObjectCallArg
               (
                   (ObjectArg)ObjectArg.Deserialize(deserializer)
               );
    }
}