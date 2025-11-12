//
//  MoveCall.cs
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

using System.Collections.Generic;
using System.Linq;
using Sui.Types;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;

namespace Sui.Transactions
{
    /// <summary>
    /// A Move Call - any public Move function can be called via
    /// this transaction. The results can be used that instant to pass
    /// into the next transaction.
    /// </summary>
    public class MoveCall : ICommand
    {
        /// <summary>
        /// The target struct (e.g., 0x2::pay::split).
        /// </summary>
        public SuiMoveNormalizedStructType Target { get; set; }

        /// <summary>
        /// An array of type tags used as type arguments for the move call.
        /// </summary>
        public SerializableTypeTag[] TypeArguments { get; set; }

        /// <summary>
        /// An array of arguments used for executing the move call.
        /// </summary>
        public TransactionArgument[] Arguments { get; set; }

        public MoveCall
        (
            SuiMoveNormalizedStructType target,
            SerializableTypeTag[] type_arguments = null,
            TransactionArgument[] arguments = null)
        {
            this.Target = target;
            this.TypeArguments = type_arguments;
            this.Arguments = arguments;
        }

        public MoveCall(JToken input)
        {
            this.Target = input.ToObject<SuiMoveNormalizedStructType>();
            this.TypeArguments = new SerializableTypeTag[] { };

            List<TransactionArgument> arguments = new List<TransactionArgument>();
            foreach (JToken arg in (JArray)input["arguments"])
                arguments.Add(arg.ToObject<TransactionArgument>());

            this.Arguments = arguments.ToArray();
        }

        /// <summary>
        /// Adds to resolve list if needed, based on the arguments and provided inputs.
        /// </summary>
        /// <param name="list">A reference to an array of `MoveCall` to which the current instance may be appended.</param>
        /// <param name="inputs">An array of `TransactionBlockInput` used for resolving the arguments.</param>
        public void AddToResolve(List<MoveCall> list, List<TransactionBlockInput> inputs)
        {
            // Determine if any of the arguments require encoding.
            // - If they don't, then this is good to go.
            // - If they do, then we need to fetch the normalized move module.
            TransactionArgument[] arguments = this.Arguments;

            bool needsResolution = arguments.Any(arg => {
                if (arg.Kind == TransactionArgumentKind.Input)
                {
                    TransactionBlockInput argInput = (TransactionBlockInput)arg.Argument;

                    // Is it a PureCallArg or ObjectCallArg?
                    // If the argument is a `TransactionBlockInput`
                    // and the value of the input at `index` is NOT a BuilderArg (`CallArg`)
                    // then we need to resolve it.
                    return inputs[argInput.Index].Value.GetType() != typeof(CallArg);
                }
                return false;
            });

            // If any of the arguments in the MoveCall need to be resolved
            // This loop verifies that the move calls within the resolve list
            // match up with that in the current transaction within the outer loop
            // of blockDataBuilder's transactions
            if (needsResolution)
            {
                foreach (MoveCall move_call in list)
                {
                    foreach (var argument_outer in this.Arguments.Select((value, i) => new { i, value }))
                    {
                        if (argument_outer.value.Kind == TransactionArgumentKind.Input)
                        {
                            foreach (var argument_inner in move_call.Arguments.Select((value, i) => new { i, value }))
                            {
                                if
                                (
                                    argument_inner.value.Kind == TransactionArgumentKind.Input
                                )
                                {
                                    TransactionBlockInput outer_input =
                                        (TransactionBlockInput)argument_outer.value.Argument;
                                    TransactionBlockInput inner_input =
                                        (TransactionBlockInput)argument_inner.value.Argument;

                                    if
                                    (
                                        outer_input.Value.Equals(inner_input.Value) &&
                                        outer_input.Index != inner_input.Index
                                    )
                                    {
                                        this.Arguments[argument_outer.i].Equals(move_call.Arguments[argument_inner.i]);
                                    }
                                }
                            }
                        }
                    }
                }
                list.Add(this);
            }
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Target);
            serializer.Serialize(this.TypeArguments);
            serializer.Serialize(this.Arguments);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new MoveCall
               (
                   (SuiMoveNormalizedStructType)SuiMoveNormalizedStructType.Deserialize(deserializer),
                   deserializer.DeserializeSequence(typeof(SerializableTypeTag)).Values.Cast<SerializableTypeTag>().ToArray(),
                   deserializer.DeserializeSequence(typeof(TransactionArgument)).Values.Cast<TransactionArgument>().ToArray()
               );
    }
}