#nullable enable
using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// An abstraction of a Transaction block input.
    /// A transaction block input is a:
    /// a TransactionInput,
    ///
    /// 
    /// In the TypeScript SDK the explicity define whether it's a pure or object,
    /// but in this SDK it automatically infers its own type, and also knows its "kind".
    /// 
    /// <code>
    ///     export const TransactionBlockInput = object({
	///         kind: literal('Input'),
    ///         index: integer(),
    ///         value: optional(any()),
    ///         type: optional(union([literal('pure'), literal('object')])),
    ///     });
    /// </code>
    /// </summary>
    public class TransactionBlockInput : ITransactionArgument
    {
        /// <summary>
        /// The index within the programmable block transaction input list.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The value of this transaction input.
        /// </summary>
        /// TODO: Look into this
        //public ICallArg Value { get; set; } // An object ref, or a core type like address or u8
        public ISerializable? Value { get; set; } // An object ref, or a core type like address or u8

        /// <summary>
        /// The type of Input being used.
        /// </summary>
        public CallArgumentType? Type { get; set; }

        /// <summary>
        /// Create a TransactionBlockInput object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        //public TransactionBlockInput(int index, ICallArg value) TODO: Look into this
        public TransactionBlockInput(int index, ISerializable? value, CallArgumentType? type)
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
        {
            serializer.SerializeU16((ushort)Index);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new TransactionBlockInput(deserializer.DeserializeU16());
        }
    }
}
#nullable disable