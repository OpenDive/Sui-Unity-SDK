//
//  TransactionArgument.cs
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

using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Utilities;

namespace Sui.Transactions
{
    /// <summary>
    /// <para>There are 4 types of transaction arguments:</para>
    /// <para>1) GasCoin</para>
    /// <para>2) TransactionBlockInput</para>
    /// <para>3) Result</para>
    /// <para>4) NestedResult</para>
    /// 
    /// <para>A TransactionArgument is an abstraction over these 4 types, and does
    /// not have a concrete implementation or serialization strategy.
    /// Each concrete type has it's own serialization strategy, particularly
    /// add a byte that represents it's type.</para>
    /// </summary>
    [JsonConverter(typeof(TransactionArgumentConverter))]
    public class TransactionArgument : ReturnBase, ISerializable
    {
        /// <summary>
        /// The kind of argument being represented.
        /// </summary>
        public TransactionArgumentKind Kind { get; private set; }

        /// <summary>
        /// The internal representation of the transaction argument.
        /// </summary>
        private ITransactionArgument transaction_argument;

        /// <summary>
        /// The public implementation of the transaction argument.
        /// Adds in safety set for changing the `Kind` value.
        /// </summary>
        public ITransactionArgument Argument
        {
            get => this.transaction_argument;
            set
            {
                if (value == null)
                    this.Kind = TransactionArgumentKind.GasCoin;
                else if (value.GetType() == typeof(TransactionBlockInput))
                    this.Kind = TransactionArgumentKind.Input;
                else if (value.GetType() == typeof(Result))
                    this.Kind = TransactionArgumentKind.Result;
                else if (value.GetType() == typeof(NestedResult))
                    this.Kind = TransactionArgumentKind.NestedResult;
                else
                {
                    this.SetError<SuiError>("Unable to set up TransactionArgument");
                    return;
                }

                this.transaction_argument = value;
            }
        }

        public TransactionArgument(TransactionArgumentKind kind, ITransactionArgument transactionArgument)
        {
            this.Kind = kind;
            this.Argument = transactionArgument;
        }

        public void Serialize(Serialization serializer)
        {
            switch (this.Kind)
            {
                case TransactionArgumentKind.GasCoin:
                    serializer.SerializeU8(0);
                    break;
                case TransactionArgumentKind.Input:
                    serializer.SerializeU8(1);
                    break;
                case TransactionArgumentKind.Result:
                    serializer.SerializeU8(2);
                    break;
                case TransactionArgumentKind.NestedResult:
                    serializer.SerializeU8(3);
                    break;
            }

            if (this.Argument != null)
                serializer.Serialize(this.Argument);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte value = deserializer.DeserializeU8().Value;

            switch (value)
            {
                case 0:
                    return new TransactionArgument
                    (
                        TransactionArgumentKind.GasCoin,
                        null
                    );
                case 1:
                    return new TransactionArgument(
                        TransactionArgumentKind.Input,
                        (TransactionBlockInput)TransactionBlockInput.Deserialize(deserializer)
                    );
                case 2:
                    return new TransactionArgument(
                        TransactionArgumentKind.Result,
                        (Result)Result.Deserialize(deserializer)
                    );
                case 3:
                    return new TransactionArgument(
                        TransactionArgumentKind.NestedResult,
                        (NestedResult)NestedResult.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize TransactionArgument", null);
            }
        }
    }
}