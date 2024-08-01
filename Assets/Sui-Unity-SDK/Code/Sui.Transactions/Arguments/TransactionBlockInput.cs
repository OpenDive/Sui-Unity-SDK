//
//  TransactionBlockInput.cs
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
using Sui.Types;

namespace Sui.Transactions
{
    /// <summary>
    /// <para>An abstraction of a Transaction block input.
    /// A transaction block input is a `TransactionInput`.</para>
    ///
    /// <para>In the TypeScript SDK the explicity define whether it's a pure or object,
    /// but in this SDK it automatically infers its own type, and also knows its "Type".</para>
    /// </summary>
    public class TransactionBlockInput : ITransactionArgument
    {
        /// <summary>
        /// The index within the programmable block transaction input list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The value of this transaction input.
        /// </summary>
        public ISerializable Value { get; set; }

        /// <summary>
        /// The type of Input being used.
        /// </summary>
        public CallArgumentType? Type { get; set; }

        public TransactionBlockInput(int index, ISerializable value, CallArgumentType? type)
        {
            this.Index = index;
            this.Value = value;
            this.Type = type;
        }

        public TransactionBlockInput(ushort index)
        {
            this.Index = index;
        }

        public void Serialize(Serialization serializer)
            => serializer.SerializeU16((ushort)Index);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new TransactionBlockInput(deserializer.DeserializeU16().Value);
    }
}