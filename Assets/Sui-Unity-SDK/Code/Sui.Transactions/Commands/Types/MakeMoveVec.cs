//
//  MakeMoveVec.cs
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

using System.Linq;
using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions
{
    /// <summary>
    /// Build a vector of objects using the input arguments.
    /// It is impossible to export construct a `vector<T: key>` otherwise,
    /// so this call serves a utility function.
    /// </summary>
    public class MakeMoveVec : ICommand
    {
        /// <summary>
        /// The objects to make into a Move vector.
        /// </summary>
        public TransactionArgument[] Objects { get; set; }

        /// <summary>
        /// The type associated with the Move vector.
        /// </summary>
        public SuiStructTag Type { get; set; }

        public MakeMoveVec(TransactionArgument[] objects, SuiStructTag type = null)
        {
            this.Objects = objects;
            this.Type = type;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(0);

            serializer.Serialize(this.Objects);
            if (this.Type != null) serializer.Serialize(this.Type);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeU8();

            return new MakeMoveVec
            (
                deserializer.DeserializeSequence(typeof(TransactionArgument)).Values.Cast<TransactionArgument>().ToArray(),
                (SuiStructTag)SuiStructTag.Deserialize(deserializer)
            );
        }
    }
}