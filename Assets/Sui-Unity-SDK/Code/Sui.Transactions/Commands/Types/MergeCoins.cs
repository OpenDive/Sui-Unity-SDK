//
//  MergeCoins.cs
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
    /// Merge Vector of Coins (`sources`) into a `destination`.
    /// </summary>
    public class MergeCoins : ICommand
    {
        /// <summary>
        /// The destination coin object to merge the sources into.
        /// </summary>
        public TransactionArgument Destination { get; set; }

        /// <summary>
        /// The sources as arguments.
        /// </summary>
        public TransactionArgument[] Sources { get; set; }

        public MergeCoins(TransactionArgument destination, TransactionArgument[] sources)
        {
            this.Destination = destination;
            this.Sources = sources;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Destination);
            serializer.Serialize(this.Sources);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new MergeCoins
               (
                   (TransactionArgument)TransactionArgument.Deserialize(deserializer),
                   deserializer.DeserializeSequence(typeof(TransactionArgument)).Values.Cast<TransactionArgument>().ToArray()
               );
    }
}
