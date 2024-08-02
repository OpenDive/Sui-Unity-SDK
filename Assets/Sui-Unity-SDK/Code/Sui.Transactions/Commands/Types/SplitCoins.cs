//
//  SplitCoins.cs
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
    /// Split `amount` from a `coin`.
    /// </summary>
    public class SplitCoins : ICommand
    {
        /// <summary>
        /// The coin to be split.
        /// </summary>
        public TransactionArgument Coin { get; set; }

        /// <summary>
        /// Amounts of the associated splits.
        /// </summary>
        public TransactionArgument[] Amounts { get; set; }

        public SplitCoins(TransactionArgument coin, TransactionArgument[] amounts)
        {
            this.Coin = coin;
            this.Amounts = amounts;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Coin);
            serializer.Serialize(this.Amounts);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SplitCoins
               (
                   (TransactionArgument)TransactionArgument.Deserialize(deserializer),
                   deserializer.DeserializeSequence(typeof(TransactionArgument)).Values.Cast<TransactionArgument>().ToArray()
               );
    }
}
