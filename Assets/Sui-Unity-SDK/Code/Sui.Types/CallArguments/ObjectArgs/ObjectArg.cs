//
//  ObjectArg.cs
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
using Sui.Utilities;

namespace Sui.Types
{
    /// <summary>
    /// Represents an object argument.
    /// </summary>
    public class ObjectArg : ISerializable
    {
        /// <summary>
        /// The object reference's type.
        /// </summary>
        public ObjectRefType Type { get; set; }

        /// <summary>
        /// The object reference's content.
        /// </summary>
        public IObjectRef ObjectRef { get; set; }

        public ObjectArg(ObjectRefType type, IObjectRef object_ref)
        {
            this.Type = type;
            this.ObjectRef = object_ref;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize((byte)Type);
            serializer.Serialize(ObjectRef);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;

            switch (type)
            {
                case 0:
                    SuiObjectRef object_ref = (SuiObjectRef)SuiObjectRef.Deserialize(deserializer);
                    return new ObjectArg(ObjectRefType.ImmOrOwned, object_ref);
                case 1:
                    SharedObjectRef shared_object_ref = (SharedObjectRef)SharedObjectRef.Deserialize(deserializer);
                    return new ObjectArg(ObjectRefType.Shared, shared_object_ref);
                default:
                    return new SuiError(0, "Unable to deserialize ObjectArg", null);
            }
        }
    }
}