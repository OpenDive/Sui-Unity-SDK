//
//  ProgrammableTransaction.cs
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
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions
{
    /// <summary>
    /// Represents a Sui programmable transaction that can execute
    /// different types of commands.
    /// </summary>
    public class ProgrammableTransaction : ITransactionKind
    {
        /// <summary>
        /// Can be a pure type (native BCS), or a Sui object (shared, or ImmutableOwned)
        /// Both type extend ISerialzable interface.
        /// </summary>
        [JsonProperty("inputs")]
        public CallArg[] Inputs { get; set; }

        /// <summary>
        /// Holds a set of transactions, e.g. MoveCallTransaction, TransferObjectsTransaction, etc.
        /// </summary>
        [JsonProperty("transactions")]
        public Command[] Transactions { get; set; }

        public ProgrammableTransaction(CallArg[] inputs, Command[] transactions)
        {
            this.Inputs = inputs;
            this.Transactions = transactions;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(new Sequence(this.Inputs));
            serializer.Serialize(new Sequence(this.Transactions));
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new ProgrammableTransaction
               (
                   deserializer.DeserializeSequence(typeof(CallArg)).Values.Cast<CallArg>().ToArray(),
                   deserializer.DeserializeSequence(typeof(Command)).Values.Cast<Command>().ToArray()
               );
    }
}