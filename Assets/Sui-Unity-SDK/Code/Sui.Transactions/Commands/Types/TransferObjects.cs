//
//  TransferObjects.cs
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

namespace Sui.Transactions
{
    /// <summary>
    /// Transfer vector of objects to a receiver.
    /// </summary>
    public class TransferObjects : ICommand
    {
        /// <summary>
        /// The objects to be transferred to the receiver.
        /// </summary>
        public TransactionArgument[] Objects { get; set; }

        /// <summary>
        /// The address of the receiver.
        /// </summary>
        public TransactionArgument Address { get; set; }

        public TransferObjects(TransactionArgument[] objects, TransactionArgument address)
        {
            this.Objects = objects;
            this.Address = address;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Objects);
            serializer.Serialize(this.Address);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new TransferObjects
               (
                   deserializer.DeserializeSequence(typeof(TransactionArgument)).Values.Cast<TransactionArgument>().ToArray(),
                   (TransactionArgument)ISerializable.Deserialize(deserializer)
               );
    }
}