//
//  CallArg.cs
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
using OpenDive.BCS;
using Newtonsoft.Json;

namespace Sui.Types
{
    /// <summary>
    /// <para>A Sui type that represents a transaction call arguments.
    /// A call arg can be a:</para>
    /// 
    /// <para>(1) A vector / list of byte (e.g., not a struct nor object).</para>
    /// <para>(2) An ObjectRef (also known as object arg).</para>
    /// <para>(3) Or a vector / list of ObjectRef.</para>
    /// </summary>
    [JsonConverter(typeof(CallArgumentConverter))]
    public class CallArg : ReturnBase, ISerializable
    {
        /// <summary>
        /// The call argument's type.
        /// </summary>
        public CallArgumentType Type { get; set; }

        /// <summary>
        /// The call argument's content.
        /// </summary>
        public ICallArg CallArgument { get; set; }

        public CallArg(CallArgumentType type, ICallArg call_argument)
        {
            this.Type = type;
            this.CallArgument = call_argument;
        }

        public CallArg(SuiError error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Create a shared object reference.
        /// </summary>
        /// <returns>A `SharedObjectRef` object.</returns>
        public SharedObjectRef GetSharedObectInput()
        {
            return this.Type switch
            {
                CallArgumentType.Object => ((ObjectCallArg)this.CallArgument).ObjectArg.Type switch
                {
                    ObjectRefType.Shared => (SharedObjectRef)((ObjectCallArg)this.CallArgument).ObjectArg.ObjectRef,
                    _ => null,
                },
                _ => null,
            };
        }

        /// <summary>
        /// Returns whether the call argument is a mutable shared object input.
        /// </summary>
        /// <returns>A bool representing whether the call argument is a mutable shared object input.</returns>
        public bool IsMutableSharedObjectInput()
            => this.GetSharedObectInput() != null ? this.GetSharedObectInput().Mutable : false;

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            serializer.Serialize(this.CallArgument);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;
            switch (type)
            {
                case 0:
                    return new CallArg(CallArgumentType.Pure, (PureCallArg)PureCallArg.Deserialize(deserializer));
                case 1:
                    return new CallArg(CallArgumentType.Object, (ObjectCallArg)ObjectCallArg.Deserialize(deserializer));
                default:
                    return new SuiError(0, "Unable to deserialize CallArg.", null);
            }
        }
    }
}